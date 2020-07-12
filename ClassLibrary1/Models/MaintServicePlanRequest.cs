using System;
using System.Data;

namespace ClassLibrary1.Models
{
    public class MaintServicePlanRequest
    {
        public int ProgramId { get; internal set; }
        public int ServiceId { get; internal set; }
        public long CurRate { get; internal set; }
        public long ServicePlanId { get; internal set; }
        public long ServiceSpanId { get; internal set; }
        public DateTime BeginDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public string SiteNum { get; internal set; }
        public object ProvNum { get; internal set; }
        public int Units { get; internal set; }
        public long Billable { get; internal set; }
        public long CurCpFirstMo { get; internal set; }
        public long CurCpOngoing { get; internal set; }
        public string ExceptionNbr { get; internal set; }
        public string ExceptionComment { get; internal set; }
        public int Exception { get; internal set; }
        public int RequestorId { get; internal set; }
        public int IsisSession { get; internal set; }
        public int Approved { get; internal set; }
        public int ProgramRequestId { get; internal set; }
        public string LevelCareCode { get; internal set; }
        public DateTime LocEffDate { get; internal set; }
        public DateTime PlanRevDate { get; internal set; }
        public DateTime ApprovedDate { get; internal set; }
        public string ExceptionComments { get; internal set; }
        public decimal MonthlyCap { get; internal set; }
        public decimal YearlyCap { get; internal set; }
        public DateTime OrigLocDate { get; internal set; }
    }
}
