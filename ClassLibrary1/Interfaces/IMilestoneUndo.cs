using ClassLibrary1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Interfaces
{
    interface IMilestonesUndo
    {
        void Undo(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoAllRolesAssigned(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoARODiagLPHA(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0, long progranRequestId = 0, string lphaCertDate = "01/01/1900");
        void UndoCancelDecision(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0, long progranRequestId = 0);
        void UndoCauseofDeath(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoClosureEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoClosureEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0, long progranRequestId = 0);
        void UndoCOLS(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoCOLSArbitration(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoDateEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0, long progranRequestId = 0);
        void UndoDeleteRoles(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoDiagEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0, long progranRequestId = 0);
        void UndoFacilityEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoFinalAuth(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoInterventionStrategy(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoLOC(long workflowPointProcessId = 0, long sessionId = 0, long workerId = 0, long oldServicePlanId = 0, long archivePlanId = 0, long oldLevelCareId = 0, string oldLevelCareEffectiveDate = "01/01/1900", string oldCSRDate = "01/01/1900", string oldOrigLOCEffectiveDate = "01/01/1900", long splitServicePlanId = 0);
        void UndoMCareLOC(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoMDSQ(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoMDSQReasons(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoMedAcceptDeny(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoMembersResponse(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoOutofStateSkilled(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0, long programRequestId = 0, long oldMedicalServicesWorkerID = 0);
        void UndoPace(long workflowPointProcessId = 0, long sessionId = 0);
        void UndoPACEWithDrawal(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        //void UndoPASRRExemption(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0, long progranRequestId = 0);
        void UndoRBSCLApproval(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0, long progranRequestId = 0);
        void UndoRSFinalAuth(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoTCMServiceAuth(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoTierEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0, long progranRequestId = 0);
        void UndoUnNaturalDeath(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void UndoWhatTriggered(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0);
        void SaveUndoAudit(UndoAuditRequest request);
    }
}
