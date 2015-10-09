using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    class FireWorkerApply : ActionApply<FireWorkerAction>
    {
        public override BoardState Apply(BoardState state, FireWorkerAction action)
        {
            if (state.State == GameState.Started)
            {
                Player player = state.Players[action.PlayerId];
                decimal highestWage = -1m;
                int highestWageId = -1;

                for (int i = 0; i < player.WorkerIds.Count; i++)
                {
                    if (state.Workers[i].Wage > highestWage)
                    {
                        highestWage = state.Workers[i].Wage;
                        highestWageId = i;
                    }
                }

                if (highestWageId > -1)
                {
                    player.WorkerIds = player.WorkerIds.Where(i => i != highestWageId).ToList();
                    player.RecentWorkers.Add(highestWageId);

                    state.Players[action.PlayerId] = player;
                    state.Workers[highestWageId] = new Worker { Job = Job.Unemployed, Wage = 0m };
                }
            }

            return state;
        }
    }
}
