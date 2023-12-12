using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal enum PipeSide
    {
        None,
        Inner,
        Outer,
        Left,
        Right,
        Top,
        Bottom
    }

    internal static class PipeSideExtensions
    {
        public static PipeSide GetOpposite(this PipeSide side)
        {
            if (side == PipeSide.Inner)
            {
                return PipeSide.Outer;
            }
            if (side == PipeSide.Outer) 
            {
                return PipeSide.Inner;
            }

            throw new Exception();      
        }
    }
}
