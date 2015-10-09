using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DT.AllIn.Game;
using DT.AllIn.Data;
using System.Net.Http;
using System.Configuration;
using System.Threading;
using System.Net.Http.Headers;
using System.Net;

namespace DT.AllIn.Loader
{
    class Program
    {
        private static T Get<T>(string name)
        {
            string value = ConfigurationManager.AppSettings[name];
            return (T)Convert.ChangeType(value, typeof(T));
        }

        private static int TickWait = Program.Get<int>("TickMilliseconds");

        private static BoardState DefaultGame()
        {
            return new BoardState
            {
                GameTick = 0,
                LastTick = DateTime.Now,
                Loans = new List<Loan>(),
                Proposals = new List<LoanProposal>(),
                Players = new List<Player>(),
                Settings = new GameSettings
                {
                    NumPlayers = Program.Get<int>("NumPlayers"),
                    Name = Program.Get<string>("DefaultGameName"),
                    Factories = 1,
                    DefaultLoanYears = Program.Get<int>("DefaultLoanYears"),
                    DefaultLoanAmountMultiplier = Program.Get<decimal>("DefaultLoanMultiplier"),
                    ResearchLevelIncrease = Program.Get<decimal>("ResearchLevelIncrease"),
                    StartMoney = Program.Get<decimal>("StartMoney"),
                    WinCashReservers = Program.Get<decimal>("WinCashReservers"),
                    WinIncome = Program.Get<decimal>("WinIncome"),
                    WokersPerPlayer = Program.Get<int>("WorkersPerPlayer"),
                    WorkerInitialWage = Program.Get<decimal>("WorkerInitialWage"),
                    WorkerLureWageMultiplier = Program.Get<decimal>("WorkerLureWageMultiplier"),
                    YearLength = TimeSpan.Parse(Program.Get<string>("YearLength"))
                },
                Workers = new Worker[Program.Get<int>("NumPlayers") * Program.Get<int>("WorkersPerPlayer")],
                State = GameState.GameNotStarted
            };
        }

        private static void Wait()
        {
            Thread.Sleep(Program.TickWait);
        }

#if DEBUG
        private static List<GameAction>[] Moves = new List<GameAction>[]
        {
            new List<GameAction> { new JoinAction { PlayerId = 0, PlayerName = "Player 1" } },
            new List<GameAction> { },
            new List<GameAction> { },
            new List<GameAction> { new JoinAction { PlayerId = 1, PlayerName = "Player 2" } },
            new List<GameAction> { new JoinAction { PlayerId = 2, PlayerName = "Player 3" } },
            new List<GameAction> { new HireWorkerAction { PlayerId = 0 } },
            new List<GameAction> { new HireWorkerAction { PlayerId = 1 }, new HireWorkerAction { PlayerId = 0 }, new HireWorkerAction { PlayerId = 2 } },
            new List<GameAction> { new HireWorkerAction { PlayerId = 1 } },
            new List<GameAction> { },
            new List<GameAction> { },
            new List<GameAction> { new AssignWorkerAction { PlayerId = 0, WorkerId = 0, Work = Job.Factory} },
            new List<GameAction> { },
            new List<GameAction> { },
            new List<GameAction> { },
            new List<GameAction> { },
            new List<GameAction> { },
            new List<GameAction> { },
            new List<GameAction> { new ProposeLoanAction { LoanAmount = 15m, PayBack = 18m, Period = TimeSpan.FromMinutes(1), PlayerId = 1 } },
            new List<GameAction> { },
            new List<GameAction> { new HireWorkerAction { PlayerId = 0 } },
            new List<GameAction> { },
            new List<GameAction> { new TakeLoanAction { PlayerId = 0, LoanProposalId = 1 } },
            new List<GameAction> { new WithdrawProposalAction { PlayerId = 1, ProposalId = 1 } }
        };
#else
        private static string PostAddress = Program.Get<string>("PostStateAddress");
        private static string GetAddress = Program.Get<string>("GetActionsAddress");
#endif
        private static BoardState game = Program.DefaultGame();

        static void Main(string[] args)
        {
            Console.WriteLine("Starting game...");
#if DEBUG
            Console.WriteLine("Debug mode");
            
            while (true)
            {
                Program.PostState();
                Program.Wait();
                var actions = Program.GetActions(Program.game);
                Program.game = Tick.Apply(Program.game, actions);
                Console.WriteLine("Processed tick {0} at {1}", Program.game.GameTick, Program.game.LastTick);

                if (Program.game.State != GameState.Started)
                {
                    Console.WriteLine("! Game not started");
                }
                else
                {
                    Program.PrintState();
                }

                Console.WriteLine("");
            }
#else
            Console.WriteLine("Will read user actions from: ", Program.GetAddress);
            Console.WriteLine("Attempting to post state to: {0}", Program.PostAddress);

            while (true)
            {
                Program.PostState();
                Program.Wait()
                Program.game = Tick.Apply(Program.game, Program.GetActions());
            }
#endif
        }

#if DEBUG
        private static void PostState()
        {
            Console.WriteLine("Posting state to server, waiting for actions");
        }

        private static List<GameAction> GetActions(BoardState state)
        {
            List<GameAction> ret = (state.GameTick < Program.Moves.Length) ? Program.Moves[state.GameTick] : new List<GameAction>();
            Console.WriteLine("  Retrieved {0} actions: {1}", ret.Count, string.Join(",", ret.Select(m => m.ActionType)));
            return ret;
        }
#else
        private static List<GameAction> GetActions()
        {
            using (var client = new WebClient())
            {
                string json = client.DownloadString(Program.GetAddress);
                return GameAction.DeserializeList(json);
            }
        }

        private async static void PostState()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                await client.PostAsync(Program.PostAddress, new StringContent(Program.game.ToJson(), Encoding.UTF8, "application/json"));
            }
        }
#endif

        private static void PrintState()
        {
            string[] players = Program.game.Players.Select(p => string.Format("Player {0}: {1} (+{2})", p.Name, p.Cash, p.Income)).ToArray();
            Console.WriteLine(string.Join(", ", players));
        }
    }
}
