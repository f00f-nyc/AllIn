using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    internal class WithdrawProposalApply : ActionApply<WithdrawProposalAction>
    {
        public override BoardState Apply(BoardState state, WithdrawProposalAction action)
        {
            try
            {
                Console.WriteLine("  -> Player {0} withdrawing a loan proposal", action.PlayerId);

                LoanProposal proposal = state.Proposals[action.ProposalId];
                if (proposal.PlayerId == action.PlayerId)
                {
                    state.Proposals.RemoveAt(action.ProposalId);
                }
                return state;
            }
            catch
            {
                return state;
            }
        }
    }
}
