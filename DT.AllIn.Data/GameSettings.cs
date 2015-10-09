using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Data
{
    public struct GameSettings
    {
        public string Name { get; set; }
        public int NumPlayers { get; set; }
        public decimal StartMoney { get; set; }
        public int WokersPerPlayer { get; set; }
        public int Factories { get; set; }

        public decimal WorkerInitialWage { get; set; }
        public decimal WorkerLureWageMultiplier { get; set; }

        public TimeSpan YearLength { get; set; }

        public decimal DefaultLoanAmountMultiplier { get; set; }
        public int DefaultLoanYears { get; set; }

        public decimal WinCashReservers { get; set; }
        public decimal WinIncome { get; set; }

        public decimal ResearchLevelIncrease { get; set; }
    }
}
