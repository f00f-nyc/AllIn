using DT.AllIn.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DT.AllIn.Test
{
    [TestFixture]
    public class DataTest
    {
        public static GameSettings Settings
        {
            get
            {
                return new GameSettings
                {
                    Name = "Sample Game",
                    Factories = 1,
                    NumPlayers = 2,
                    StartMoney = 20m,
                    WokersPerPlayer = 3,
                    WorkerInitialWage = 1m,
                    WorkerLureWageMultiplier = 1.1m,
                    YearLength = TimeSpan.FromMilliseconds(100),
                    DefaultLoanAmountMultiplier = 1.5m,
                    DefaultLoanYears = 1,
                    WinCashReservers = 100000m,
                    WinIncome = 10000m
                };
            }
        }

        public static Player Player1
        {
            get
            {
                return new Player
                         {
                             PlayerId = 0,
                             Cash = 14.2m,
                             Income = .1m,
                             Name = "First Player",
                             ResearchLevel = 1.1m,
                             WorkerIds = new List<int> { 0 },
                             RecentWorkers = new List<int> { },
                             NumFactories = 2,
                             NextFactory = 15
                         };
            }
        }

        public static Player Player2
        {
            get
            {
                return new Player
                            {
                                PlayerId = 1,
                                Cash = 21.2m,
                                Income = 2m,
                                Name = "Second Player",
                                ResearchLevel = 1m,
                                WorkerIds = new List<int> { 1, 2, 3 },
                                RecentWorkers = new List<int> { },
                                NumFactories = 5,
                                NextFactory = 994
                            };
            }
        }

        public static BoardState SampleGame
        {
            get
            {
                Loan loan = new Loan { Amount = 1, BorrowerId = 0, LenderId = 1, Payment = 2, Period = TimeSpan.FromMinutes(1), Start = DateTime.Now };
                List<Loan> loans = new List<Loan> { loan };
                List<Player> players = new List<Player> { DataTest.Player1, DataTest.Player2 };
                long yearTicks = DataTest.Settings.YearLength.Ticks;

                return new BoardState
                {
                    Settings = DataTest.Settings,
                    State = GameState.Started,
                    GameTick = 1000,
                    LastTick = DateTime.Now,
                    Loans = loans,
                    Players = players,
                    Workers = new[]
                    {
                         new Worker { Job = Job.Factory, Wage = 1m },
                         new Worker { Job = Job.Factory, Wage = 1.2m },
                         new Worker { Job = Job.Factory, Wage = 1.4m },
                         new Worker { Job = Job.Research, Wage = 1.1m },
                         new Worker { Job = Job.Unemployed, Wage = 0m },
                         new Worker { Job = Job.Unemployed, Wage = 0m }
                     },
                    Proposals = new List<LoanProposal>
                    {
                        new LoanProposal { PlayerId = -1, LoanAmount = 20, Period = new TimeSpan(yearTicks * 3), PayBack = 30 },
                        new LoanProposal { PlayerId = 1, LoanAmount = 15, Period = new TimeSpan(yearTicks * 2), PayBack = 20 }
                    }
                };
            }
        }

        public static TestAction SampleAction
        {
            get { return new TestAction { TestValue = "abc", PlayerId = 1 }; }
        }

        [Test]
        public void GameSerializes()
        {
            string json = DataTest.SampleGame.ToJson();
            Assert.IsNotNullOrEmpty(json);
        }

        [Test]
        public void GameDeserializes()
        {
            BoardState sample = DataTest.SampleGame;
            string json = sample.ToJson();
            BoardState fromJson = BoardState.FromJson(json);

            Assert.AreEqual(sample.Workers.Length, fromJson.Workers.Length);
            Assert.AreEqual(sample.Settings.Name, fromJson.Settings.Name);
            Assert.AreEqual(sample.GameTick, fromJson.GameTick);
        }

        [Test]
        public void ActionSerializes()
        {
            TestAction act = DataTest.SampleAction;
            string json = act.ToJson();
            Assert.IsNotNullOrEmpty(json);
            Assert.Greater(json.IndexOf(act.TestValue), 0);
        }

        [Test]
        public void ActionDeserializes()
        {
            string json = DataTest.SampleAction.ToJson();
            TestAction act = (TestAction)GameAction.FromJson(json);
            Assert.AreEqual(act.TestValue, DataTest.SampleAction.TestValue);
        }


        [Test]
        public void SerializeItAll()
        {
            JoinAction join = new JoinAction { PlayerId = 1, PlayerName = "Test Player" };
            HireWorkerAction hire = new HireWorkerAction { PlayerId = 2 };
            FireWorkerAction fire = new FireWorkerAction { PlayerId = 3 };
            ProposeLoanAction propose = new ProposeLoanAction { PlayerId = 4, Period = TimeSpan.FromMinutes(1.0), PayBack = 20m, LoanAmount = 15m };
            WithdrawProposalAction withdraw = new WithdrawProposalAction { PlayerId = 5, ProposalId = 2 };
            GiveGiftAction gift = new GiveGiftAction { PlayerId = 6, ToPlayerId = 2, GiftAmount = 10.2m };
            AssignWorkerAction research = new AssignWorkerAction { PlayerId = 7, Work = Job.Research, WorkerId = 1 };
            AssignWorkerAction factory = new AssignWorkerAction { PlayerId = 8, Work = Job.Factory, WorkerId = 2 };
            TakeLoanAction loan = new TakeLoanAction { PlayerId = 9, LoanProposalId = 1 };

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"gameJson\": ");
            sb.Append(DataTest.SampleGame.ToJson());
            sb.Append("\n, \"NoOp\": ");
            sb.Append(new NoOpAction().ToJson());
            sb.Append("\n \"JoinGame\": ");
            sb.Append(join.ToJson());
            sb.Append("\n \"HireWorker\": ");
            sb.Append(hire.ToJson());
            sb.Append("\n \"FireWorker\": ");
            sb.Append(fire.ToJson());
            sb.Append("\n \"ProposeLoan\": ");
            sb.Append(propose.ToJson());
            sb.Append("\n \"WithdrawProposal\": ");
            sb.Append(withdraw.ToJson());
            sb.Append("\n \"GiveGift\": ");
            sb.Append(gift.ToJson());
            sb.Append("\n \"AssignWorkerResearch\": ");
            sb.Append(research.ToJson());
            sb.Append("\n \"AssignWorkerFactory\": ");
            sb.Append(factory.ToJson());
            sb.Append("\n \"TakeLoan\": ");
            sb.Append(loan.ToJson());

            List<GameAction> actions = new List<GameAction> { hire, fire, loan };
            string json = GameAction.SerializeList(actions);
            List<GameAction> fromJson = GameAction.DeserializeList(json);

            List<GameAction> empty = new List<GameAction>();
            json = GameAction.SerializeList(empty);
            fromJson = GameAction.DeserializeList(json);
        }
    }
}
