using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class MDSQReasonsMilestoneRequest
    {
        public long WorkflowPointProcessID { get; set; }
        public long WorkflowPointResponseID { get; set; }
        public long WorkerID { get; set; }
        public string Comments { get; set; }
        public long ProgranRequestId { get; set; }
        public int ChangeMindInd { get; set; }
        public int ServiceUnavailableInd { get; set; }
        public int NeedTooHighInd { get; set; }
        public int LackingSupportInd { get; set; }
        public int NoFundingInd { get; set; }
        public int PrevAttemptsFailedInd { get; set; }
        public int NoGuardianPOAInd { get; set; }
        public int CourtCommittedInd { get; set; }
        public int OtherInd { get; set; }
        public long SessionId { get; set; }
    }
}
