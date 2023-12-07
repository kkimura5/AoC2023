using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class EngineNumber
    {
        public EngineNumber(string number, int column, int row)
        {
            Number = number;
            Column = column;
            Row = row;
        }

        public string Number { get; }
        public int Value => int.Parse(Number);
        public int Column { get; }
        public int Row { get; }
        public bool IsPartNumber { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
