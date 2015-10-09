using DT.AllIn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Game.Actions
{
    internal class GiveGiftApply : ActionApply<GiveGiftAction>
    {
        public override BoardState Apply(BoardState state, GiveGiftAction action)
        {
            if (state.State == GameState.Started)
            {
                Console.WriteLine("  -> Player {0} gifiting player {1}, amount: {2}", action.PlayerId, action.ToPlayerId, action.GiftAmount);

                Player giver = state.Players[action.PlayerId];
                Player receiver = state.Players[action.ToPlayerId];

                giver.Cash -= action.GiftAmount;
                receiver.Cash += action.GiftAmount;

                state.Players[action.PlayerId] = giver;
                state.Players[action.ToPlayerId] = receiver;
            }

            return state;
        }
    }
}
