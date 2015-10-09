using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game
{
    public class Create
    {
        public static BoardState FromSettings(GameSettings settings)
        {
            BoardState ret = new BoardState { Settings = settings, State = GameState.GameNotStarted };
            ret.Players = new List<Player>();
            ret.Loans = new List<Loan>();

            int numWorkers = settings.WokersPerPlayer * settings.NumPlayers;
            ret.Workers = new Worker[numWorkers];

            for (int i = 0; i < numWorkers; i++)
            {
                ret.Workers[i] = new Worker { Job = Job.Unemployed, Wage = 0m };
            }

            return ret;
        }
    }
}
