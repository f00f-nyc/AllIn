using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    internal class TakeLoanApply : ActionApply<TakeLoanAction>
    {
        public override BoardState Apply(BoardState state, TakeLoanAction action)
        {
            if (state.State == GameState.Started)
            {
                if (action.LoanProposalId < state.Proposals.Count)
                {
                    LoanProposal propose = state.Proposals[action.LoanProposalId];
                    if (propose.PlayerId != action.PlayerId)
                    {
                        Player to = state.Players[action.PlayerId];
                        to.Cash += propose.LoanAmount;
                        state.Players[action.PlayerId] = to;

                        if (propose.PlayerId > -1)
                        {
                            Player from = state.Players[propose.PlayerId];
                            from.Cash -= propose.LoanAmount;
                            state.Players[propose.PlayerId] = from;
                        }

                        state.Loans.Add(new Loan
                        {
                            Amount = propose.LoanAmount,
                            Period = propose.Period,
                            Payment = propose.PayBack,
                            LenderId = propose.PlayerId,
                            BorrowerId = action.PlayerId,
                            Start = DateTime.Now
                        });
                    }
                }
            }

            return state;
        }
    }
}
