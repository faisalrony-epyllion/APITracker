using APITracker.Application.Entities;



namespace APITracker.Application.Interfaces
{
    public interface IReportAPISetupService : ICommonService<ReportAPISetup>
    {
        //Task<List<ReportAPISetup>> GetAllAPIReport();
        Task<ReportAPISetup> GetAPIReportByReportName(string reportName);
        //Task<ReportAPISetup> AddAPIReport(ReportAPISetup reportAPISetup);
        //Task<string> DeleteAPIReport(string reportName);
        Task<dynamic> GetDynamicReportAPIDataAsync(string username, string token, string reportName, string values);
    }
}
