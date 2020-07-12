
using System;

namespace ClassLibrary1.Models
{
    public class TCMMilestoneRequest
    {
        public long WorkflowPointProcessID { get; set; }
        public long WorkflowPointResponseID { get; set; }
        public long WorkerID { get; set; }
        public string Comments { get; set; }
        public long ProgramRequestID { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string PrimaryDisabilityID { get; set; }
        public long PrimaryPsychDiagCode { get; set; }
        public DateTime WrittenDocumentationofDisabilityDate { get; set; }
        public int ConsumerinWaiverInd { get; set; }
        public int ConsumerNeedsHelpWithSchoolingEtcInd { get; set; }
        public int ConsumerNeedsRespiteInd { get; set; }
        public int ConsumerNeedsHealthServicesInd { get; set; }
        public int ConsumerNeedsDailyLivingSkillsSLCInd { get; set; }
        public int ConsumerNeedsDailyLivingSkillsPayeeInd { get; set; }
        public int ConsumerNeedsDailyLivingSkillsMealsonWheelsInd { get; set; }
        public int ConsumerNeedsDailyLivingSkillsTransInd { get; set; }
        public int ConsumerNeedsDailyLivingSkillsHomemakerInd { get; set; }
        public int ConsumerNeedsSupportToEngageMHTreatmentInd { get; set; }
        public int ConsumerNeedsAssistInMedsManagementInd { get; set; }
        public int ConsumerNeedsAssistInHousingInd { get; set; }
        public int ConsumerNeedsAssisttoAccessEligServicesInd { get; set; }
        public int AssessmentDemonstrationImpairmentInd { get; set; }
        public int DocumentofLackofAbilitytoAccessServicesInd { get; set; }
        public int DocumentofLackofAbilitytoSustainServicesInd { get; set; }
        public int ConsumerNeedsDailyLivingSkillsInd { get; set; }
        public int ConsumerNeedsSocialFunctioningInd { get; set; }
        public int ConsumerNeedsVocationalorPreVocationalServicesInd { get; set; }
        public int ConsumerNeedsNonVocationalServicesInd { get; set; }
        public int ConsumerResidesinMINoDischargePlan30DaysInd { get; set; }
        public int ConsumerCurrServicedInAssertiveProgramInd { get; set; }
        public long SessionID { get; set; }
        public string UserAcknowledgement { get; set; }
        public int DenyReason { get; set; }
        public int VersionNumber { get; set; }
    }
}
