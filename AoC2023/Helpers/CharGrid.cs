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
        private char[,] grid;

        public CharGrid(List<string> rows)
        {
            grid = new char[rows.Count, rows[0].Length];
            for (int r = 0; r < rows.Count; r++)
            {
                for (int c = 0; c < rows[r].Length; c++)
                {
                    grid[r, c] = rows[r][c];
                }
            }
        }

        public CharGrid(char[,] chars)
        {
            grid = chars;
        }

        public char this[int row, int col]
        {
            get { return grid[row,col]; }
        }

        public char[,] GetGrid() => grid;
        public override string ToString()
        {
            var output = string.Empty;
            for (int i = 0; i <= MaxRow; i++)
            {
                output += GetRow(i);
            }

            return output;
        }
        public int MaxCol => grid.GetLength(1)-1;
        public int MaxRow => grid.GetLength(0)-1;
        public string GetRow(int index) => string.Concat(Enumerable.Range(0, grid.GetLength(1))
                .Select(x => grid[index, x]));
        public string GetCol(int index) => string.Concat(Enumerable.Range(0, grid.GetLength(0))
                .Select(x => grid[x, index]));

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

        internal void SetRow(int row, string value)
        {
            for (int c = 0; c < value.Length; c++)
            {
                grid[row,c] = value[c];
            }
        }

        internal void SetCol(int col, string value)
        {
            for (int r = 0; r < value.Length; r++)
            {
                grid[r, col] = value[r];
            }
        }

        internal void SetValue(RowCol location, char value)
        {
            grid[location.Row, location.Col] = value; 
        }

        internal bool IsLocationWithin(RowCol nextLocation)
        {
            return nextLocation.Row >= 0 && nextLocation.Col >= 0 
                && nextLocation.Row <= MaxRow && nextLocation.Col <= MaxCol;
        }
    }
}
