using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class CharGrid
    {
        private List<string> rows;

        public CharGrid(List<string> rows)
        {
            this.rows= rows;
        }

        public char this[int row, int col]
        {
            get { return rows[row][col]; }
        }

        public int MaxCol => rows.First().Length - 1;
        public int MaxRow => rows.Count - 1;
        public string GetRow(int index) => rows[index];
        public string GetCol(int index) => string.Concat(rows.Select(x => x[index]));
        public override string ToString()
        {
            return string.Join(Environment.NewLine, rows);
        }

        public RowCol FindChar(char ch)
        {
            for (int r = 0; r < MaxRow; r++) 
            {
                for (int c = 0; c < MaxCol; c++)
                {
                    if (this[r,c] == ch)
                    {
                        return new RowCol(r, c);
                    }
                }
            }

            return new RowCol(-1,-1);
        }

        internal void SetValue(RowCol location, char value)
        {
            string row = rows[location.Row];
            var start = location.Col == 0 ? string.Empty : row.Substring(0, location.Col);
            var end = location.Col > MaxCol ? string.Empty : row.Substring(location.Col + 1);
            rows[location.Row] = $"{start}{value}{end}"; 
        }
    }
}
