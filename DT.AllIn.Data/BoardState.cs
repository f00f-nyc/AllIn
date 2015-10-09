using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Data
{
    public enum GameState
    {
        GameNotStarted,
        Started,
        Finish
    }

    public struct BoardState
    {
        public GameSettings Settings { get; set; }
        public GameState State { get; set; }
        public List<Player> Players { get; set; }
        public Worker[] Workers { get; set; }
        public List<Loan> Loans { get; set; }
        public List<LoanProposal> Proposals { get; set; }

        public long GameTick { get; set; }
        public DateTime LastTick { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static BoardState FromJson(string json)
        {
            return JsonConvert.DeserializeObject<BoardState>(json);
        }
    }
}
