using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Data
{
    public struct Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public List<int> WorkerIds { get; set; }
        public List<int> RecentWorkers { get; set; }
        public decimal ResearchLevel { get; set; }
        public int NextResearchLevel { get; set; }
        public int NumFactories { get; set; }
        public int NextFactory { get; set; }
        public decimal Cash { get; set; }

        public decimal Income { get; set; }

        public Worker[] Workers(BoardState game)
        {
            return this.WorkerIds.Select(i => game.Workers[i]).ToArray();
        }
    }
}
