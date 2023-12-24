using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class LavaPath
    {
        public LavaPath(RowCol location, int score, List<Direction> previousDirections, List<string> previousLocations)
        {
            CurrentLocation = location;
            Score = score;
            PreviousDirections = previousDirections;
            PreviousLocations = previousLocations;
        }

        public RowCol CurrentLocation { get; set; }
        public int Score { get; set; }
        public List<Direction> PreviousDirections { get; set; } 
        public List<string> PreviousLocations { get; set; }

        public override string ToString()
        {
            return $"{CurrentLocation}: {Score}";
        }

        internal string GetSummary()
        {
            var previousDirections = string.Empty;
            var firstDirection = PreviousDirections.FirstOrDefault();
            foreach (var direction in PreviousDirections)
            {
                if (direction != firstDirection)
                {
                    break;
                }

                previousDirections += direction.ToString();
            }

            return $"{CurrentLocation}{previousDirections}";
        }
    }
}
