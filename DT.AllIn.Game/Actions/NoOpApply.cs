using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    internal class NoOpApply : ActionApply<NoOpAction>
    {
        public override BoardState Apply(BoardState state, NoOpAction action)
        {
            return state;
        }
    }
}
