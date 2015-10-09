using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    internal class ProposeLoanApply : ActionApply<ProposeLoanAction>
    {
        public override BoardState Apply(BoardState state, ProposeLoanAction action)
        {
            Console.WriteLine("  -> Player {0} proposing a loan for amount {1}", action.PlayerId, action.LoanAmount);

            state.Proposals.Add(
                new LoanProposal
                {
                    PlayerId = action.PlayerId,
                    LoanAmount = action.LoanAmount,
                    Period = action.Period,
                    PayBack = action.PayBack
                }
            );

            return state;
        }
    }
}
