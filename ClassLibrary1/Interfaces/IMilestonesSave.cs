using ClassLibrary1.Models;

namespace ClassLibrary1.Interfaces
{
    interface IMilestonesSave
    {
        void SaveAllRolesAssigned(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", string data = "", int responseType = 0, long sessionId = 0);
        void SaveARODiagLPHA(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long programRequestId = 0, long workerID = 0, string diagnosisCodes = "", string comments = "", string lphaCertDate = "01/01/1900", string oldLPHACertDate = "01/01/1900", long sessionId = 0);
        void SaveCancelDecision(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, bool accept = true, long sessionId = 0);
        string SaveCapIncrease(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, decimal monthlyCap = 0, string beginDate = "01/01/1900", long sessionId = 0);
        void SaveCauseofDeath(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, int primaryCauseofDeath = 0, int secondaryCauseofDeath = 0, long sessionId = 0);
        string SaveDeleteRoles(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long sessionId = 0);
        void SaveClosureEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long programRequestId = 0, long workerID = 0, int closureReasonID = 0, string closureReason = "", string comments = "", long sessionId = 0);
        string SavecMCareLOC(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long levelCareID = 0, long sessionId = 0);
        void SaveCOLS(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", int countyOfLegalSettlementID = 0, long sessionId = 0);
        void SaveCOLSArbitration(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", int countyOfLegalSettlementID = 0, long sessionId = 0);
        void SaveDateEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestID = 0, int dateType = 0, string captureDate = "01/01/1900", long sessionId = 0);
        void SaveDCNEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestID = 0, long dcn = 0, long sessionId = 0);
        void SaveDiagEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long programRequestId = 0, long workerID = 0, string diagnosisCodes = "", string comments = "", long sessionId = 0);
        string SaveFacilityEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, string providerNumber = "", int workflowPointType = 0, long sessionId = 0);
        void SaveFinalAuth(long workflowPointProcessID = 0, long workflowPointResponseID = 0, int workflowApproveDeny = 0, long workerID = 0, string comments = "", string servicePlanIds = "", long sessionId = 0, int denialReasonCode = 0, string assesmentDate = "01/01/1900");
        void SaveIntervStrategy(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestID = 0, int askMembertoStopBehavior = 0, int encourageMemberExpression = 0, int attemptDistraction = 0, int offeredOtherChoices = 0, int changedEnvironment = 0, int mediatedConflict = 0, string otherIntervention = "", long sessionId = 0);
        //string SaveISPASRRPresent(long programRequestId = 0, long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", string data = " ", int workflowApproveDeny = 0, long sessionId = 0);
        void SaveLOC(LOCMilestoneRequest locRequest);
        void SaveMDSQ(long workflowPointProcessID = 0, long workflowPointResponseID = 0, string response = "", long workerID = 0, string comments = "", string data = "", long sessionId = 0);
        void SaveMDSQReasons(MDSQReasonsMilestoneRequest mdsqMilestone);
        void SaveMedAcceptDeny(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", string data = " ", int workflowApproveDeny = 0, long sessionId = 0, string beginCode = "");
        void SaveMembersResponse(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, int gradualincreaseinAgitation = 0, int explosiveAggressionWithStress = 0, int explosiveAggressionWithoutProvocation = 0, string otherResponse = "", long sessionId = 0);
        void SaveMilestone(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", string data = "", long sessionId = 0, string beginEndCode = "");
        void SaveOutofStateSkilled(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, int outofStateInd = 0, long sessionId = 0);
        void SavePACEWithDrawal(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, int withDrawalInd = 0, long sessionId = 0);
        //void SavePASRRExemption(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, int categoryId = 0, string comments = "", long programRequestId = 0, string expirationDate = "", long sessionId = 0);
        void SaveRBSCLApproval(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, int rbsclInd = 0, long sessionId = 0);
        void SaveRSFinalAuth(long serviceSpanID = 0, int approveDenyInd = 0, long denialReasonCode = 0, long sessionId = 0);
        void SaveTCMServiceAuth(TCMMilestoneRequest tcmRequest);
        void SaveTierEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, string tier = "", string tierEffectiveDate = "01/01/1900", long sessionId = 0);
        void SaveUnnaturalDeath(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, string riskFactors = "", string actionsTaken = "", string actionsTakenforSafetyofOthers = "", long sessionId = 0);
        void SaveWhatTriggered(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "", long programRequestId = 0, int environmentalFactors = 0, int personalCharacteristics = 0, int consequencesOfEvent = 0, long sessionId = 0);
        void UpdateComment(int milestoneId, string comment, long isisSession);
    }
}
