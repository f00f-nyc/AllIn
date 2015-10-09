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

namespace DT.AllIn.Loader
{
    class Program
    {
        private static T Get<T>(string name)
        {
            string value = ConfigurationManager.AppSettings[name];
            return (T)Convert.ChangeType(value, typeof(T));
        }

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
                    YearLength = Program.Get<TimeSpan>("YearLength")
                }
            };
        }

        private static string PostAddress = Program.Get<string>("PostStateAddress");
        private static string GetAddress = Program.Get<string>("GetActionsAddress");
        private static BoardState game = Program.DefaultGame();

        static async void Main(string[] args)
        {
            Console.WriteLine("Starting game...");
            Console.WriteLine("Will read user actions from: ", Program.GetAddress);
            Console.WriteLine("Attempting to post state to: {0}", Program.PostAddress);

            while (true)
            {
                Program.PostState();
                Thread.Sleep(990);
                var actions = await Program.GetActionsString();
                Program.game = Tick.Apply(Program.game, actions);
            }
        }

        private async static Task<List<GameAction>> GetActionsString()
        {
            using (var client = new HttpClient())
            {
                string json = await client.GetStringAsync(Program.GetAddress);
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
    }
}
