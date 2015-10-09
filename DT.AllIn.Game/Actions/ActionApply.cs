using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    internal interface IActionApply
    {
        BoardState Apply(BoardState state, GameAction action);
    }

    internal abstract class ActionApply<T> : IActionApply
        where T : GameAction
    {
        public abstract BoardState Apply(BoardState state, T action);

        public BoardState Apply(BoardState state, GameAction action)
        {
            return this.Apply(state, (T)action);
        }
    }
}
