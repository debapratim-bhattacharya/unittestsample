using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class SaveCodeTableProgramRequest
    {
        public int ProgramId { get; internal set; }
        public string ProgramName { get; internal set; }
        public string ProgramType { get; internal set; }
        public int MonthlyUnitCap { get; internal set; }
        public long MonthlyCap { get; internal set; }
        public int YearlyUnitCap { get; internal set; }
        public long YearlyCap { get; internal set; }
        public string ProgramDisplayName { get; internal set; }
        public string StatePlanEnhancedInd { get; internal set; }
        public string ProgramManager { get; internal set; }
        public int SessionId { get; internal set; }
    }
}
