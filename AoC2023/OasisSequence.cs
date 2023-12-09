using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class OasisSequence
    {

        public OasisSequence(List<long> values)
        {
            Values = values;
        }
        public List<long> Values { get; }

        public OasisSequence GetChildSequence()
        {
            var newValues = new List<long>();
            for (int i = 1; i < Values.Count; i++)
            {
                var newValue = Values[i] - Values[i-1];
                newValues.Add(newValue);
            }

            return new OasisSequence(newValues);
        }
        public override string ToString()
        {
            return string.Join(" ", Values);
        }

        public bool IsEnd()
        {
            return Values.All(x => x == 0);
        }
    }
}
