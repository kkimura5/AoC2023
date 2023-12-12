using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal struct RowCol
    {
        public RowCol(int r, int c)
        {
            Row = r;
            Col = c;
        }

        public int Row { get; set; }
        public int Col { get; set; }

        internal Direction GetDirection()
        {
            if (Row == -1)
            {
                return Direction.Up;
            }
            else if (Row == 1)
            {
                return Direction.Down;
            }
            else if (Col == 1) 
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Left;
            }
        }

        public static bool operator ==(RowCol left, RowCol right)
        {
            return left.Row == right.Row && left.Col == right.Col;
        }

        public static bool operator !=(RowCol left, RowCol right)
        {
            return left.Row != right.Row || left.Col != right.Col;
        }

        public override string ToString()
        {
            return $"({Row}, {Col})";
        }
        public static RowCol operator -(RowCol first, RowCol second)
        {
            return new RowCol(first.Row - second.Row, first.Col - second.Col);
        }
    }
}
