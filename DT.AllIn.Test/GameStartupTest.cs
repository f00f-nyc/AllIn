using DT.AllIn.Data;
using DT.AllIn.Game;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DT.AllIn.Test
{
    [TestFixture]
    public class GameStartupTest
    {
        [Test]
        public void GameStartsWithCorrectNumberOfUsers()
        {
            BoardState game = DataTest.SampleGame;
            GameSettings settings = game.Settings;
            settings.NumPlayers = 3;

            game.Settings = settings;
            game.State = GameState.GameNotStarted;

            JoinAction join = new JoinAction { PlayerId = 3, PlayerName = "Player 3" };
            BoardState last = Tick.Apply(game, new List<GameAction> { join });

            Assert.AreEqual(GameState.Started, last.State);
            Assert.AreEqual(3, last.Players.Count);

            Player added = last.Players.First(p => p.PlayerId == 3);
            Assert.AreEqual(game.Settings.Factories, added.NumFactories);
            Assert.AreEqual(0, added.NextFactory);
            Assert.AreEqual(game.Settings.StartMoney, added.Cash);
        }

        [Test]
        public void GameCreatedFromSettings()
        {
            GameSettings settings = DataTest.Settings;
            BoardState game = Create.FromSettings(settings);

            Assert.AreEqual(0, game.Players.Count);
            Assert.AreEqual(0, game.Loans.Count);
            Assert.AreEqual(settings.NumPlayers * settings.WokersPerPlayer, game.Workers.Length);
            Assert.IsNotNull(game.Workers[0]);
            Assert.AreEqual(GameState.GameNotStarted, game.State);
        }

        [Test]
        public void GameTicks()
        {
            BoardState game = DataTest.SampleGame;
            DateTime beforeTimestamp = game.LastTick;
            long beforeTick = game.GameTick;
            Thread.Sleep(50);
            game = Tick.Apply(game, new GameAction[0]);
            DateTime afterTimestamp = game.LastTick;
            long afterTick = game.GameTick;

            Assert.AreEqual(beforeTick + 1, afterTick);
            Assert.Greater((afterTimestamp - beforeTimestamp).Ticks, TimeSpan.FromMilliseconds(49).Ticks);
        }
    }
}
