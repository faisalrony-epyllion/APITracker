﻿using APITracker.Infrastructure.Data;
using Dapper.Contrib.Extensions;

namespace APITracker.Application.Entities.ReportAPI
{
    [Table("ReportAPIUserHistory")]
    public class ReportAPIUserHistory : DapperBaseEntity
    {
        [ExplicitKey]
        public int ReportID { get; set; }
        [ExplicitKey]
        public int UserCode { get; set; }
        [ExplicitKey]
        public DateTime CallingStartTime { get; set; }
        public DateTime? CallingEndTime { get; set; }

        #region Additional Properties
        [Write(false)]
        public override bool IsModified => EntityState == System.Data.Entity.EntityState.Modified || UserCode == 0;
        #endregion Additional Properties
    }
}
