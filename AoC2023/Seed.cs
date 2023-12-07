using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class Seed
    {
        public Seed(long start, long count)
        {
            Start = start;
            Count = count;
        }

        public long Start { get; private set; }
        public long Count { get; private set; }
        public long MaxValue => Start + Count - 1;

        public override string ToString()
        {
            return $"{Start} {Count}";
        }

        internal void Join(Seed seedToJoin)
        {
            var maxValue = Math.Max(MaxValue, seedToJoin.MaxValue); 
            Start = Math.Min(Start, seedToJoin.Start);
            Count = maxValue - Start + 1;
        }
    }
}
