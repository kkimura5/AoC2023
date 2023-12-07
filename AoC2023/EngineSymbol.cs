using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class EngineSymbol
    {
        public EngineSymbol(char character, int column, int row)
        {
            Character = character;
            X = column;
            Y = row;
        }

        public char Character { get; }
        public bool IsStar => Character == '*';
        public long X { get; set; }
        public long Y { get; set; }

        public List<EngineNumber> ConnectedNumbers { get; set; } = new List<EngineNumber>();
    }
}
