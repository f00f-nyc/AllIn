using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DT.AllIn.Data;
using DT.AllIn.Game.Actions;

namespace DT.AllIn.Game
{
    public class Tick
    {
        private static Dictionary<ActionType, IActionApply> actionMap = new Dictionary<ActionType, IActionApply>()
        {
            { ActionType.NoOp, new NoOpApply() },
            { ActionType.JoinGame, new JoinGameApply() },
            { ActionType.HireWorker, new HireWorkerApply() },
            { ActionType.FireWorker, new FireWorkerApply() },
            { ActionType.ProposeLoan, new ProposeLoanApply() },
            { ActionType.WithdrawProposal, new WithdrawProposalApply() },
            { ActionType.GiveGift, new GiveGiftApply() },
            { ActionType.AssignWorker, new AssignWorkerApply() },
            { ActionType.TakeLoan, new TakeLoanApply() }
        };

        public static BoardState Apply(BoardState game, IEnumerable<GameAction> list)
        {
            TimeSpan tick = DateTime.Now - game.LastTick;

            foreach (GameAction action in list)
            {
                game = Tick.actionMap[action.ActionType].Apply(game, action);
            }

            game = Tick.TickWages(game, tick);
            game = Tick.TickLoans(game, tick);
            game = Tick.NormalizeMoney(game);

            game.GameTick++;
            game.LastTick = DateTime.Now;

            return Tick.CheckWin(game);
        }

        private static BoardState NormalizeMoney(BoardState game)
        {
            for (int w=0; w<game.Workers.Length; w++)
            {
                Worker worker = game.Workers[w];
                int money = (int)(worker.Wage * 100);
                game.Workers[w] = new Worker { Job = worker.Job, Wage = (decimal)money / 100m };
            }

            for (int p = 0; p < game.Players.Count; p++)
            {
                Player player = game.Players[p];
                int cash = (int)(player.Cash * 100);
                int income = (int)(player.Income * 100);

                player.Cash = (decimal)cash / 100m;
                player.Income = (decimal)income / 100m;
                game.Players[p] = player;
            }

            return game;
        }

        private static BoardState CheckWin(BoardState game)
        {
            foreach (Player player in game.Players)
            {
                if (player.Cash > game.Settings.WinCashReservers)
                {
                    game.State = GameState.Finish;
                }
                else if (player.Income > game.Settings.WinIncome && player.Cash > 0)
                {
                    game.State = GameState.Finish;
                }
            }

            return game;
        }

        private static decimal PercentOfYear(BoardState game, TimeSpan tick)
        {
            return (decimal)tick.Ticks / game.Settings.YearLength.Ticks;
        }

        private static decimal PercentOfPeriod(TimeSpan tick, TimeSpan period)
        {
            return (decimal)tick.Ticks / period.Ticks;
        }

        private static BoardState TickWages(BoardState game, TimeSpan tick)
        {
            if (game.State != GameState.Started)
            {
                return game;
            }

            decimal percent = Tick.PercentOfYear(game, tick);

            for (int i=0; i<game.Players.Count; i++)
            {
                Player player = game.Players[i];
                foreach (int workerId in player.WorkerIds)
                {
                    Worker worker = game.Workers[workerId];
                    player.Cash -= percent * worker.Wage;

                    if (worker.Job == Job.Factory)
                    {
                        player.Cash += percent * player.NumFactories * player.ResearchLevel;
                    }
                    else if (worker.Job == Job.Research)
                    {
                        player.NextResearchLevel += (int)(percent * 1000);
                        if (player.NextResearchLevel > 1000)
                        {
                            player.ResearchLevel += game.Settings.ResearchLevelIncrease;
                            player.NextResearchLevel = player.NextResearchLevel - 1000;
                        }
                    }
                }
                game.Players[i] = player;
            }

            return game;
        }

        private static BoardState TickLoans(BoardState game, TimeSpan tick)
        {
            if (game.Proposals[0].PlayerId != -1)
            {
                game.Proposals.Insert(0, new LoanProposal
                {
                    PlayerId = -1,
                    LoanAmount = game.Settings.StartMoney,
                    PayBack = game.Settings.StartMoney * game.Settings.DefaultLoanAmountMultiplier,
                    Period = TimeSpan.FromTicks(game.Settings.YearLength.Ticks * game.Settings.DefaultLoanYears)
                });
            }

            List<int> removeLoans = new List<int>();

            for (int i = 0; i < game.Loans.Count; i++)
            {
                Loan loan = game.Loans[i];
                TimeSpan period = tick;

                if (loan.Start + loan.Period < DateTime.Now)
                {
                    period = (loan.Start + loan.Period) - game.LastTick;
                    removeLoans.Add(i);
                }

                decimal percent = Tick.PercentOfPeriod(period, loan.Period);
                Player borrower = game.Players[loan.BorrowerId];
                borrower.Cash -= percent * loan.Payment;
                game.Players[loan.BorrowerId] = borrower;

                if (loan.LenderId > -1)
                {
                    Player lender = game.Players[loan.LenderId];
                    lender.Cash += percent * loan.Payment;
                    game.Players[loan.LenderId] = lender;
                }  
            }

            for (int i = removeLoans.Count - 1; i >= 0; i--)
            {
                int removeId = removeLoans[i];
                game.Loans.RemoveAt(removeId);
            }

            return game;
        }
    }
}
