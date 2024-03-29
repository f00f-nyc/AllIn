﻿using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    internal class AssignWorkerApply : ActionApply<AssignWorkerAction>
    {
        public override BoardState Apply(BoardState state, AssignWorkerAction action)
        {
            if (state.State == GameState.Started)
            {
                Console.WriteLine("  -> Assigning worker {0} job of {1}", action.WorkerId, action.Work);

                if (state.Players[action.PlayerId].WorkerIds.Contains(action.WorkerId))
                {
                    state.Workers[action.WorkerId].Job = action.Work;
                }
            }

            return state;
        }
    }
}
