using System;

namespace ClassLibrary1.Models
{
    public class InvoiceRequest
    {
        public int InvoiceId { get; set; }

        public int IsisServicePlanId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime ScheduleDate { get; set; }

        public int VoidInd { get; set; }

        public string StateId { get; set; }

        public string SsN { get; set; }

        public string MemberCustomerId { get; set; }

        public string FiscalYear { get; set; }

        public long SessionId { get; set; }
    }
}
