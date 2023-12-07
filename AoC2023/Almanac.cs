using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class Almanac
    {
        public Almanac(List<Mapping> mappings)
        {
            Mappings = mappings;
        }

        public List<Mapping> Mappings { get; }

        public List<Seed> Convert(Seed inputSeed) 
        {
            var seeds = new List<Seed>();
            var currentValue = inputSeed.Start;
            while (currentValue < inputSeed.MaxValue)
            {
                var mapping = Mappings.SingleOrDefault(x => x.IsValueInRange(currentValue));
                if (mapping == null)
                {
                    var nextValue = Mappings.OrderBy(x => x.Source).FirstOrDefault(x => x.Source > currentValue)?.Source ?? inputSeed.MaxValue;
                    seeds.Add(new Seed(currentValue, nextValue - currentValue + 1));
                    currentValue = nextValue;
                    continue;
                }

                var newSeed = mapping.CreateNewSeed(currentValue, inputSeed.MaxValue);
                seeds.Add(newSeed);
                currentValue += newSeed.Count;
            }

            return seeds;
        }

        public long Convert(long value)
        {
            foreach (var mapping in Mappings)
            {
                if (mapping.IsValueInRange(value))
                {
                    return mapping.Convert(value);
                }
            }

            return value;
        }
    }
}
