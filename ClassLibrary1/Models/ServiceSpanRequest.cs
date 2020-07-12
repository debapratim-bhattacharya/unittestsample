using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class ServiceSpanRequest
    {
        public int ProgramId { get; internal set; }
        public int ServiceId { get; internal set; }
        public long CurRate { get; internal set; }
        public int ServicePlanId { get; internal set; }
        public int ServiceSpanId { get; internal set; }
        public DateTime BeginDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public string SiteNum { get; internal set; }
        public int ProvNum { get; internal set; }
        public int Units { get; internal set; }
        public long Billable { get; internal set; }
        public long CurCpFirstMo { get; internal set; }
        public long CurCpOngoing { get; internal set; }
        public int IsisSession { get; internal set; }
        public int ProgramRequestId { get; set; }
        public int SentToFiscalAgent { get; set; }
    }
}
