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
    public class GameWorkersTest
    {
        private static int UnhiredWorkersCount(BoardState game)
        {
            return game.Workers.Where(w => w.Job == Job.Unemployed).Count();
        }

        private static Worker[] GetPlayerWorkers(BoardState game, int playerId)
        {
            return game.Players[playerId].WorkerIds.Select(i => game.Workers[i]).ToArray();
        }

        [Test]
        public void CannotHireIfGameIsNotStarted()
        {
            BoardState game = DataTest.SampleGame;
            game.State = GameState.GameNotStarted;
            HireWorkerAction hire = new HireWorkerAction { PlayerId = 0 };
            
            int workersBefore = GameWorkersTest.UnhiredWorkersCount(game);
            game = Tick.Apply(game, new List<GameAction> { hire });
            Assert.AreEqual(workersBefore, GameWorkersTest.UnhiredWorkersCount(game));
        }

        [Test]
        public void WorkersAreHiredFromPoolFirst()
        {
            BoardState game = DataTest.SampleGame;
            HireWorkerAction hire = new HireWorkerAction() { PlayerId = 0 };

            int workersBefore = GameWorkersTest.UnhiredWorkersCount(game);
            Assert.AreEqual(2, workersBefore, "The test is written based on the assumption that there are exactly 2 workers free in the pool.");

            game = Tick.Apply(game, new List<GameAction> { hire });
            game = Tick.Apply(game, new List<GameAction> { hire });
            Assert.AreEqual(0, GameWorkersTest.UnhiredWorkersCount(game));

            Worker[] player2WorkersBefore = GameWorkersTest.GetPlayerWorkers(game, 1);
            game = Tick.Apply(game, new List<GameAction> { hire });
            Worker[] player2WorkersAfter = GameWorkersTest.GetPlayerWorkers(game, 1);

            Assert.AreEqual(player2WorkersBefore.Length - 1, player2WorkersAfter.Length);
        }

        [Test]
        public void WorkersFromPoolHiredAtSetWage()
        {
            BoardState game = DataTest.SampleGame;
            List<int> unhiredWokerIds = new List<int>();
            for (int i=0; i<game.Workers.Length; i++) 
            {
                if (game.Workers[i].Job == Job.Unemployed) 
                {
                    unhiredWokerIds.Add(i);
                }
            }
            Assert.AreEqual(2, unhiredWokerIds.Count, "The test is written based on the assumption that there are exactly 2 workers free in the pool.");

            HireWorkerAction hire = new HireWorkerAction() { PlayerId = 0 };
            game = Tick.Apply(game, new List<GameAction> { hire, hire });

            Worker hiredWorker = game.Workers[unhiredWokerIds[0]];
            Assert.AreEqual(Job.Factory, hiredWorker.Job);
            Assert.AreEqual(game.Settings.WorkerInitialWage, hiredWorker.Wage);
        }

        [Test]
        public void HiringAllWorkers()
        {
            BoardState game = DataTest.SampleGame;
            HireWorkerAction hire = new HireWorkerAction() { PlayerId = 0 };
            List<GameAction> hireList = game.Workers.Select(w => hire).Cast<GameAction>().ToList();
            game = Tick.Apply(game, hireList);

            List<decimal> currentWagesBefore = game.Workers.Select(w => w.Wage).ToList();
            Tick.Apply(game, hireList);
            List<decimal> currentWagesAfter = game.Workers.Select(w => w.Wage).ToList();

            for (int i = 0; i < currentWagesAfter.Count; i++)
            {
                Assert.AreEqual(currentWagesBefore[i], currentWagesAfter[i]);
            }
        }

        [Test]
        public void HiringFromAnotherPlayerHonorsSettings()
        {
            BoardState game = DataTest.SampleGame;
            HireWorkerAction hire = new HireWorkerAction() { PlayerId = 1 };
            game = Tick.Apply(game, new List<GameAction> { hire, hire });

            int wId = game.Players[0].WorkerIds[0];
            decimal wageBefore = game.Workers[wId].Wage;
            game = Tick.Apply(game, new List<GameAction> { hire });
            decimal wageAfter = game.Workers[wId].Wage;

            Assert.AreEqual(wageBefore * game.Settings.WorkerLureWageMultiplier, wageAfter);
        }

        [Test]
        public void CannotFireIfGameIsNotStarted()
        {
            BoardState game = DataTest.SampleGame;
            game.State = GameState.GameNotStarted;
            FireWorkerAction fire = new FireWorkerAction { PlayerId = 0 };

            int workersBefore = GameWorkersTest.UnhiredWorkersCount(game);
            game = Tick.Apply(game, new List<GameAction> { fire });
            Assert.AreEqual(workersBefore, GameWorkersTest.UnhiredWorkersCount(game));
        }

        [Test]
        public void FireWorkerActionWorks()
        {
            BoardState game = DataTest.SampleGame;
            FireWorkerAction fire = new FireWorkerAction { PlayerId = 0 };
            game = Tick.Apply(game, new List<GameAction> { fire });
            Assert.AreEqual(0, GameWorkersTest.GetPlayerWorkers(game, 0).Length);
        }

        [Test]
        public void FireWorkerChoosesHighestPaid()
        {
            BoardState game = DataTest.SampleGame;
            decimal highestWageBefore = game.Players[1].WorkerIds.Select(w => game.Workers[w]).Max(w => w.Wage);
            FireWorkerAction fire = new FireWorkerAction { PlayerId = 1 };
            game = Tick.Apply(game, new List<GameAction> { fire });
            decimal highestWageAfter = game.Players[1].WorkerIds.Select(w => game.Workers[w]).Max(w => w.Wage);

            Assert.Greater(highestWageBefore, highestWageAfter);
        }

        [Test]
        public void WorkersArePaidTheirWage()
        {
            BoardState game = DataTest.SampleGame;
            game.Workers[0].Wage = 10000m;
            Thread.Sleep(game.Settings.YearLength);

            game = Tick.Apply(game, new GameAction[0]);
            Assert.Less(game.Players[0].Cash, -9000m);
        }

        [Test]
        public void CannotHireWorkerJustFied()
        {
            BoardState game = DataTest.SampleGame;
            HireWorkerAction hire = new HireWorkerAction() { PlayerId = 1 };
            game = Tick.Apply(game, new List<GameAction> { hire, hire });

            Assert.AreEqual(0, GameWorkersTest.UnhiredWorkersCount(game), "This test relies on the fact that there are exactly two unhired workers");
            FireWorkerAction fire = new FireWorkerAction() { PlayerId = 1 };
            game = Tick.Apply(game, new List<GameAction> { fire });

            Assert.That(game.Players[1].RecentWorkers.Count > 0);
            Assert.AreEqual(1, GameWorkersTest.UnhiredWorkersCount(game));
            game = Tick.Apply(game, new List<GameAction> { hire });
            Assert.AreEqual(1, GameWorkersTest.UnhiredWorkersCount(game));
        }

        [Test]
        public void HiringAFiredWorkerRemovesRecentWorkerIndicator()
        {
            BoardState game = DataTest.SampleGame;
            HireWorkerAction player1Hire = new HireWorkerAction() { PlayerId = 1 };
            game = Tick.Apply(game, new List<GameAction> { player1Hire, player1Hire });

            Assert.AreEqual(0, GameWorkersTest.UnhiredWorkersCount(game), "This test relies on the fact that there are exactly two unhired workers");
            FireWorkerAction fire = new FireWorkerAction() { PlayerId = 1 };
            game = Tick.Apply(game, new List<GameAction> { fire });

            HireWorkerAction player0Hire = new HireWorkerAction() { PlayerId = 0 };
            game = Tick.Apply(game, new List<GameAction> { player0Hire });
            Assert.AreEqual(0, game.Players[1].RecentWorkers.Count);
        }

        [Test]
        public void AssignWorkerHonored()
        {
            BoardState game = DataTest.SampleGame;
            int workerId = game.Players[1].WorkerIds.First(w => game.Workers[w].Job == Job.Factory);
            AssignWorkerAction assign = new AssignWorkerAction { PlayerId = 1, Work = Job.Research, WorkerId = workerId };
            game = Tick.Apply(game, new List<GameAction> { assign });
            Assert.AreEqual(Job.Research, game.Workers[workerId].Job);
        }
    }
}
