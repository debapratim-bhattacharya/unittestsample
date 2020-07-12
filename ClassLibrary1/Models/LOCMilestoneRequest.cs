using System;

namespace ClassLibrary1.Models
{
    public class LOCMilestoneRequest
    {
        public long WorkflowPointProcessID { get; set; }
        public long WorkflowPointResponseID { get; set; }
        public long WorkerID { get; set; }
        public string Comments { get; set; }
        public long OldLevelCareID { get; set; }
        public DateTime OldLevelCareEffectiveDate { get; set; }
        public DateTime OldCSRDate { get; set; }
        public DateTime OldOrigLOCEffectiveDate { get; set; }
        public int LevelCareID { get; set; }
        public DateTime LevelCareEffectiveDate { get; set; }
        public DateTime CsrDate { get; set; }
        public bool ForceServicePlanSplit { get; set; }
        public long SessionId { get; set; }
    }
}
