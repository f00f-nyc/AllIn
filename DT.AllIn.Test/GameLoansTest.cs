using DT.AllIn.Data;
using DT.AllIn.Game;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DT.AllIn.Test
{
    [TestFixture]
    public class GameLoansTest
    {
        [Test]
        public void ProposeLoanShowsUp()
        {
            BoardState state = DataTest.SampleGame;
            ProposeLoanAction loan = new ProposeLoanAction { PlayerId = 0, LoanAmount = 123m, PayBack = 124m, Period = state.Settings.YearLength };
            state = Tick.Apply(state, new List<GameAction> { loan });

            LoanProposal prop = state.Proposals.First(l => l.LoanAmount == 123m);

            Assert.AreEqual(0, prop.PlayerId);
            Assert.AreEqual(123m, prop.LoanAmount);
            Assert.AreEqual(124m, prop.PayBack);
            Assert.AreEqual(state.Settings.YearLength, prop.Period);
        }

        [Test]
        public void WithdrawLoanProposalWorks()
        {
            BoardState state = DataTest.SampleGame;
            ProposeLoanAction loan = new ProposeLoanAction { PlayerId = 0, LoanAmount = 123m, PayBack = 124m, Period = state.Settings.YearLength };
            state = Tick.Apply(state, new List<GameAction> { loan });

            int withdrawId = state.Proposals.FindIndex(p => p.LoanAmount == 123m);
            WithdrawProposalAction withdraw = new WithdrawProposalAction { PlayerId = 0, ProposalId = withdrawId };
            state = Tick.Apply(state, new List<GameAction> { withdraw });

            Assert.AreEqual(0, state.Proposals.Count(p => p.LoanAmount == 123m));
        }

        [Test]
        public void DefaultLoanAlwaysShowsUpFirst()
        {
            BoardState state = DataTest.SampleGame;
            state.Proposals = new List<LoanProposal>();
        
            ProposeLoanAction loan = new ProposeLoanAction { PlayerId = 0, LoanAmount = 100m, PayBack = 101m, Period = state.Settings.YearLength };
            state = Tick.Apply(state, new List<GameAction> { loan });

            Assert.Greater(state.Proposals.Count, 1);
            Assert.AreEqual(-1, state.Proposals[0].PlayerId);
            Assert.AreEqual(state.Settings.YearLength.Ticks * state.Settings.DefaultLoanYears, state.Proposals[0].Period.Ticks);
        }

        [Test]
        public void CannotWithdrawSomeoneElsesProposal()
        {
            BoardState state = DataTest.SampleGame;
            ProposeLoanAction loan = new ProposeLoanAction { PlayerId = 0, LoanAmount = 123m, PayBack = 124m, Period = state.Settings.YearLength };
            state = Tick.Apply(state, new List<GameAction> { loan });

            int withdrawId = state.Proposals.FindIndex(p => p.LoanAmount == 123m);
            WithdrawProposalAction withdraw = new WithdrawProposalAction { PlayerId = 1, ProposalId = withdrawId };
            state = Tick.Apply(state, new List<GameAction> { withdraw });

            Assert.AreEqual(1, state.Proposals.Count(p => p.LoanAmount == 123m));
        }

        [Test]
        public void GiveGiftHonored()
        {
            BoardState state = DataTest.SampleGame;
            decimal giftGiverCashBefore = state.Players[1].Cash;
            decimal giftReceiverCashBefore = state.Players[0].Cash;
            GiveGiftAction gift = new GiveGiftAction { PlayerId = 1, GiftAmount = 100m, ToPlayerId = 0 };
            state = Tick.Apply(state, new List<GameAction> { gift });

            Assert.Greater(0, state.Players[1].Cash);
            Assert.Less(90, state.Players[0].Cash);
        }

        [Test]
        public void TakeLoanActionHonored()
        {
            BoardState state = DataTest.SampleGame;
            ProposeLoanAction propose = new ProposeLoanAction { LoanAmount = 123m, PayBack = 120m, Period = state.Settings.YearLength, PlayerId = 0 };
            state = Tick.Apply(state, new List<GameAction> { propose });

            int loanId = state.Proposals.FindIndex(l => l.LoanAmount == 123m);
            TakeLoanAction loan = new TakeLoanAction { LoanProposalId = loanId, PlayerId = 1 };
            state = Tick.Apply(state, new List<GameAction> { loan });

            Assert.Greater(-90, state.Players[0].Cash);
            Assert.Less(90, state.Players[1].Cash);
        }

        [Test]
        public void LoansAreHonored()
        {
            BoardState state = DataTest.SampleGame;
            ProposeLoanAction propose = new ProposeLoanAction { LoanAmount = 123m, PayBack = 999m, Period = state.Settings.YearLength, PlayerId = 0 };
            state = Tick.Apply(state, new List<GameAction> { propose });

            int loanId = state.Proposals.FindIndex(l => l.LoanAmount == 123m);
            TakeLoanAction loan = new TakeLoanAction { LoanProposalId = loanId, PlayerId = 1 };
            state = Tick.Apply(state, new List<GameAction> { loan });

            Thread.Sleep(state.Settings.YearLength);
            state = Tick.Apply(state, new List<GameAction> { });

            Assert.AreEqual(-1, state.Loans.FindIndex(l => l.Amount == 123m));
            Assert.That(500m < state.Players[0].Cash);
        }
    }
}
