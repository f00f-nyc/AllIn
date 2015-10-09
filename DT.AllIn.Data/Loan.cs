using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Data
{
    public struct Loan
    {
        public int LenderId { get; set; }
        public int BorrowerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Start { get; set; }
        public TimeSpan Period { get; set; }
        public decimal Payment { get; set; }
    }

    public struct LoanProposal
    {
        public int PlayerId { get; set; }
        public TimeSpan Period { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal PayBack { get; set; }
    }
}
