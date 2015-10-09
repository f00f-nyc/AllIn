using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    internal class JoinGameApply : ActionApply<JoinAction>
    {
        public override BoardState Apply(BoardState state, JoinAction action)
        {
            if (state.Players.Count != state.Settings.NumPlayers)
            {
                Player player = new Player
                {
                    Cash = state.Settings.StartMoney,
                    Income = 0m,
                    Name = action.PlayerName,
                    NextFactory = 0,
                    NumFactories = state.Settings.Factories,
                    PlayerId = action.PlayerId,
                    RecentWorkers = new List<int> { },
                    ResearchLevel = 0,
                    WorkerIds = new List<int> { }
                };

                state.Players.Add(player);

                if (state.Players.Count == state.Settings.NumPlayers)
                {
                    state.State = GameState.Started;
                }
            }

            return state;
        }
    }
}
