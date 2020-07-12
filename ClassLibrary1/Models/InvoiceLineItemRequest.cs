using System;

namespace ClassLibrary1.Models
{
    public class InvoiceLineItemRequest
    {
        public int InvoiceId { get; set; }

        public int IsisServicePlanId { get; set; }

        public int ProviderId { get; set; }

        public DateTime ServiceSpanBeginDate { get; set; }

        public DateTime ServiceSpanEndDate { get; set; }

        public string Wcode { get; set; }

        public string Modifier { get; set; }

        public int Units { get; set; }

        public decimal UnitCost { get; set; }

        public decimal TotalCost { get; set; }

        public decimal CpAmount { get; set; }

        public decimal Fees { get; set; }

        public decimal NetCost { get; set; }

        public decimal Credits { get; set; }

        public int Exception { get; set; }

        public int DeleteInvoiceLineItemInd { get; set; }

        public long SessionId { get; set; }
    }
}
