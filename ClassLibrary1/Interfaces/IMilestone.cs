using System.Data;

namespace ClassLibrary1.Interfaces
{
    public interface IMilestone
    {
        void BuildMilestone(int workflowPointTemplateId, int mileStoneType, int roleId, int questionId, int daystoRespond);
        void BuildPredecessor(int workflowPredecessorId, int responseTemplateId, int workflowPointTemplateId, int andOrInd);
        void BuildResponse(int workflowResponseId, int responseTemplateId, string response);
        void CloseMilestones(long isisSession, int programRequestId, int closureReason);
        string DeleteWorkflow(int workflowNumber);
        DataTable GetCausesofDeath();
        DataTable GetMilestoneInfo(int workflowPointId);
        DataTable GetMilestoneQuestion(int milestoneNumber = 0);
        DataTable GetMilestoneQuestions();
        DataTable GetMilestones(int workflowTemplateId);
        DataTable GetMilestoneType(int milestoneTypeNumber = 0);
        DataTable GetMilestoneTypes();
        DataTable GetPredecessors(int workflowTemplateId);
        DataTable GetResponses(int workflowTemplateId);
        DataTable GetStatus(int programRequestId);
        void LockMilestone(int workflowPointProcessId, int workerId);
        void RaiseChangeEvent(string changeEvent, long sessionId, int data, string customData = "");
        string SaveMilestoneQuestion(int milestoneNumber, string milestoneWording);
        string SaveMilestoneType(string milestoneType, string processingPage);
        void SavePredecessors(int workflowTemplateId, int predecessorCount);
        void SaveResponses(int workflowTemplateId, int responseCount);
        string SaveWorkflow(int workflowNumber, string workflowCode, int milestoneCount, string workflowName);
        void UnlockMilestone(int workflowPointProcessId);
        void UnlockMilestones(int programRequestId, long isisSession);
    }
}
