using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    internal class HireWorkerApply: ActionApply<HireWorkerAction>
    {
        private static int[] Range(int length)
        {
            int[] ret = new int[length];
            for (int i = 0; i < length; i++)
            {
                ret[i] = i;
            }
            return ret;
        }

        public override BoardState Apply(BoardState state, HireWorkerAction action)
        {
            if (state.State == GameState.Started)
            {
                Player player = state.Players[action.PlayerId];
                var workers = HireWorkerApply
                    .Range(state.Workers.Length)
                    .Where(i => !player.WorkerIds.Contains(i))
                    .Where(i => !player.RecentWorkers.Contains(i))
                    .Select(i => new Tuple<int, Worker>(i, state.Workers[i]))
                    .ToArray();

                if (workers.Length > 0)
                {
                    var minWage = workers.Min(t => t.Item2.Wage);
                    var worker = workers.First(w => w.Item2.Wage == minWage);

                    if (worker.Item2.Job == Job.Unemployed)
                    {
                        state.Workers[worker.Item1] = new Worker
                                                      {
                                                          Job = Job.Factory,
                                                          Wage = state.Settings.WorkerInitialWage
                                                      };
                    }
                    else
                    {
                        state.Workers[worker.Item1] = new Worker
                                                      {
                                                          Job = Job.Factory,
                                                          Wage = state.Settings.WorkerLureWageMultiplier * worker.Item2.Wage
                                                      };

                        foreach (Player other in state.Players)
                        {
                            if (other.WorkerIds.IndexOf(worker.Item1) > 0)
                            {
                                other.WorkerIds.Remove(worker.Item1);
                            }
                        }
                    }

                    foreach (Player otherPlayer in state.Players)
                    {
                        if (otherPlayer.RecentWorkers.Contains(worker.Item1))
                        {
                            otherPlayer.RecentWorkers.Remove(worker.Item1);
                        }
                    }

                    player.WorkerIds.Add(worker.Item1);
                }
            }

            return state;
        }
    }
}
