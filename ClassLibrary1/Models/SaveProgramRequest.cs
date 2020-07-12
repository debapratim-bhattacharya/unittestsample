using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class SaveProgramRequest
    {
        public int IsisSession { get; internal set; }
        public int ProgramRequestId { get; internal set; }
        public string StateId { get; internal set; }
        public DateTime BeginDate { get; internal set; }
        public string BeginCode { get; internal set; }
        public string StatusCode { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public string EndCode { get; internal set; }
        public string AidType { get; internal set; }
        public int LevelCareId { get; internal set; }
        public int ProgramId { get; internal set; }
        public int CountyId { get; internal set; }
        public string ProviderNum { get; internal set; }
        public long CurCpFirstMonth { get; internal set; }
        public long CurCpOngoing { get; internal set; }
        public DateTime AssessmentDate { get; internal set; }
        public DateTime ApplicationDate { get; internal set; }
        public string ExceptionNbr { get; internal set; }
        public string ExceptionComment { get; internal set; }
        public string CaseId { get; internal set; }
        public DateTime CsrDate { get; internal set; }
        public int Approved { get; internal set; }
        public DateTime OrigAssessmentDate { get; internal set; }
        public int RequestorId { get; internal set; }
        public string TermCode { get; internal set; }
        public string TermDate { get; internal set; }
        public int OutofStateSkilledInd { get; internal set; }
        public string HospiceNfProviderNum { get; internal set; }
        public int Rbscl { get; internal set; }
        public int InaFacility { get; internal set; }
        public string Tier { get; internal set; }
        public DateTime TierEffectiveDate { get; internal set; }
        public int WorkerId { get; internal set; }
    }
}
