using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC
{
    internal class MapInstruction
    {
        private MapInstruction leftInstruction;
        private MapInstruction rightInstruction;

        public MapInstruction(string instruction)
        {
            var match = Regex.Match(instruction, @"(?<node>\w{3}) = \((?<left>\w{3}), (?<right>\w{3})\)");
            Node = match.Groups["node"].Value;
            Left = match.Groups["left"].Value;
            Right = match.Groups["right"].Value;
        }

        public string Node { get; }
        public string Left { get; }
        public string Right { get; }
        public override string ToString()
        {
            return $"{Node} = ({Left}, {Right})";
        }
        public MapInstruction GetLeftInstruction() => leftInstruction;
        public MapInstruction GetRightInstruction() => rightInstruction;
        internal void SetLeft(MapInstruction mapInstruction)
        {
            leftInstruction = mapInstruction;
        }

        internal void SetRight(MapInstruction mapInstruction)
        {
            rightInstruction = mapInstruction;
        }
    }
}
