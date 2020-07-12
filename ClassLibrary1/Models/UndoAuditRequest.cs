
namespace ClassLibrary1.Models
{
    public class UndoAuditRequest
    {
        public int ProgramRequestId { get; set; }
        public int MilestoneId { get; set; }
        public string MilestoneType { get; set; }
        public int WorkflowProcessId { get; set; }
        public int ResponseId { get; set; }
        public string CustomData { get; set; }
        public string Note { get; set; }
        public int WorkerId { get; set; }
    }
}
