namespace ClassLibrary1.Models
{
    public class RsDiagnosisRequest
    {
        public int ServicePlanId { get; internal set; }
        public int Axis1DiagId { get; internal set; }
        public int Axis2DiagId { get; internal set; }
        public string Axis3 { get; internal set; }
        public string Axis4 { get; internal set; }
        public string Axis5PastYear { get; internal set; }
        public string Axis5Session { get; internal set; }
        public string Axis5Current { get; internal set; }
        public string Lpha { get; internal set; }
    }
}
