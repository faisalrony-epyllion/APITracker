#region Using
using APITracker.API.CustomMiddlwares;
using APITracker.Application.Interfaces;
using APITracker.Application.Services;
using APITracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Reflection;
using System.Text;
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache(opt => { ////for in memory caching
    opt.SizeLimit = 100; ///// Set the caching key limit
});

#region Swagger config
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(opt =>
//{
//    opt.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Version = "v1",
//        Title = "Epyllion Group Expense Management System API",
//        Description = "RESTful API using C# .NET 8, Dapper, AutoMapper, NLog for\r\nlogging, FluentValidation for input validation",
//        TermsOfService = new Uri("https://www.linkedin.com/in/anupam-das-09102131/"),
//        Contact = new OpenApiContact
//        {
//            Name = "Contact Me",
//            Url = new Uri("https://www.linkedin.com/in/anupam-das-09102131/")
//        },
//        License = new OpenApiLicense
//        {
//            Name = "License, Source Code",
//            Url = new Uri("https://www.linkedin.com/in/anupam-das-09102131/")
//        }
//    });

//    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//    opt.IncludeXmlComments(xmlPath);

//    opt.EnableAnnotations();

//    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Scheme = "Bearer",
//        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer uYrweyJhbGci321GrewqOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
//    });

//    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });

//});
#endregion

#region Services LifeTime
//builder.Services.AddSingleton<DapperDBContext>();

// Register IDapperCRUDService<T> with DapperCRUDService<T>
builder.Services.AddScoped(typeof(IDapperCRUDService<>), typeof(DapperCRUDService<>)); 

builder.Services.AddTransient<IReportAPISetupService, ReportAPISetupService>();
#endregion

#region AutoMapper
//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
#endregion

#region NLogConfig
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    logging.AddNLog(builder.Configuration.GetSection("v"));
});
builder.Host.UseNLog();
#endregion

#region Authentification
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var validateJwt = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentException("security key can not be null"))),
};


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{

    jwt.SaveToken = true;
    jwt.TokenValidationParameters = validateJwt;
});
builder.Services.AddSingleton(validateJwt);
#endregion

#region Cors
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("RequestPipeline",
//        builder =>
//        {
//            if (builder == null) throw new ArgumentNullException(nameof(builder));
//            builder.WithOrigins()
//                   .AllowAnyHeader()
//                   .AllowAnyMethod();
//        });
//});
#endregion

var app = builder.Build();

#region Use Swagger
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Epyllion Group Expense Management System API v1");
    });
}
#endregion


app.UseHttpsRedirection();
//app.UseAuthentication();
app.UseAuthorization();

#region Custom Middlwares
app.UseMiddleware<GlobalExceptionHandler>();
//app.UseMiddleware<LoggingMiddleware>();
//app.UseMiddleware<RateLimitingMiddleware>();
#endregion

app.MapControllers();
//app.UseCors("RequestPipeline");
app.Run();
