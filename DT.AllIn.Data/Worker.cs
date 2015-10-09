using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Data
{
    public enum Job
    {
        Unemployed,
        Factory,
        Research
    }

    public struct Worker
    {
        public decimal Wage { get; set; }
        public Job Job { get; set; }
    }
}
