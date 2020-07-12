namespace ClassLibrary1.Models
{
    public class SaveProgramServiceRequest
    {
        public int ProgramsServiceId { get; internal set; }
        public int ProgramId { get; internal set; }
        public int ServiceId { get; internal set; }
        public long CurRate { get; internal set; }
        public long CapType { get; internal set; }
        public string AgeEdit { get; internal set; }
        public string CashOut { get; internal set; }
        public long CurCashOutAmount { get; internal set; }
        public long CurAverageRate { get; internal set; }
        public int DiscountPercent { get; internal set; }
        public string RateAutoFill { get; internal set; }
        public string PaMeasureType { get; internal set; }
        public int PaMeasureValue { get; internal set; }
        public string PaMeasureActive { get; internal set; }
        public string PaMeasureBegDate { get; internal set; }
        public string PaMeasureEndDate { get; internal set; }
        public int SessionId { get; internal set; }
    }
}
