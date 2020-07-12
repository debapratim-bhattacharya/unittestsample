using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class ServicePlanRequest
    {
        public int ServicePlanId { get;  set; }
        public int ProgramRequestId { get;  set; }
        public string LevelCareCode { get;  set; }
        public DateTime BeginDate { get;  set; }
        public DateTime EndDate { get;  set; }
        public DateTime LocEffDate { get;  set; }
        public DateTime OrigLocDate { get;  set; }
        public DateTime PlanRevDate { get;  set; }
        public DateTime ApprovedDate { get;  set; }
        public long MonthlyCap { get;  set; }
        public long YearlyCap { get;  set; }
        public string ExceptionNbr { get;  set; }
        public string ExceptionComments { get;  set; }
        public int Exception { get;  set; }
        public int RequestorId { get;  set; }
        public int IsisSession { get;  set; }
        public int Approved { get;  set; }
        public decimal CpFirstMonth { get;  set; }
        public decimal CpOngoing { get;  set; }
        public int SupportBrokerId { get;  set; }
    }
}
