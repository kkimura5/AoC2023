using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class Mapping  
    {
        public Mapping(long start, long source, long range)
        {
            Start = start;
            Source = source;
            Range = range;
        }

        public long Start { get; }
        public long Source { get; }
        public long Range { get; }
        public long MaxInputValue => Source + Range - 1;
        public long MaxOutputValue => Start + Range - 1;
        public override string ToString()
        {
            return $"{Start}:{Source}:{Range}";
        }

        internal static Mapping Create(string input)
        {
            var inputs = input.Trim().Split(' ').Select(x => long.Parse(x)).ToList();
            return new Mapping(inputs[0], inputs[1], inputs[2]);
        }

        internal long Convert(long value)
        {
            return value - Source + Start;
        }

        internal bool IsValueInRange(long value)
        {
            return value >= Source && value <= MaxInputValue;
        }

        internal Seed CreateNewSeed(long input, long maxValue)
        {
            return new Seed(Convert(input), Math.Min(maxValue, MaxInputValue) - input + 1);
        }
    }
}
