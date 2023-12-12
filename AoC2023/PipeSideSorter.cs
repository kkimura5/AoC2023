using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class PipeSideSorter
    {
        public PipeSide CurrentPipeSide { get; }
        public char CurrentChar { get; }
        public char NextChar { get; }
        public Direction Direction { get; }

        public PipeSideSorter(PipeSide currentPipeSide, char currentChar, char nextChar, Direction direction)
        {
            CurrentPipeSide = currentPipeSide;
            CurrentChar = currentChar;
            NextChar = nextChar;
            Direction = direction;
        }

        public PipeSide GetNextPipeSide()
        {
            switch (NextChar)
            {
                case '.':
                case 'I':
                case 'O':
                    return PipeSide.None;

                case 'F':
                case 'L':
                case 'J':
                case '7':
                    return HandleCorners();

                case '|':
                    if (Direction == Direction.Right)
                    {
                        return PipeSide.Left;
                    }
                    else if (Direction == Direction.Left)
                    {
                        return PipeSide.Right;
                    }
                    else
                    {
                        if (CurrentChar == 'F' ||  CurrentChar == 'L')
                        {
                            return CurrentPipeSide == PipeSide.Outer ? PipeSide.Left : PipeSide.Right;
                        }
                        else if (CurrentChar == 'J' || CurrentChar == '7')
                        {
                            return CurrentPipeSide == PipeSide.Outer ? PipeSide.Right : PipeSide.Left;
                        }
                        else
                        {
                            return CurrentPipeSide;
                        }
                    }

                case '-':
                case 'S':
                    if (Direction == Direction.Up)
                    {
                        return PipeSide.Bottom;
                    }
                    else if (Direction == Direction.Down)
                    {
                        return PipeSide.Top;
                    }
                    else
                    {
                        if (CurrentChar == 'F' || CurrentChar == '7')
                        {
                            return CurrentPipeSide == PipeSide.Outer ? PipeSide.Top: PipeSide.Bottom;
                        }
                        else if (CurrentChar == 'J' || CurrentChar == 'L')
                        {
                            return CurrentPipeSide == PipeSide.Outer ? PipeSide.Bottom : PipeSide.Top;
                        }
                        else
                        {
                            return CurrentPipeSide;
                        }
                    }
            }

            return CurrentPipeSide;
        }

        private PipeSide HandleCorners()
        {
            if (CurrentPipeSide == PipeSide.None)
            {
                return PipeSide.Outer;
            }

            if (CurrentChar == '-' || CurrentChar == '|' || CurrentChar == 'S')
            {
                return HandleHorizontals();
            }

            var flippedSide = IsFlippingSides();

            switch (Direction)
            {
                case Direction.Left:
                    if (NextChar == 'F' || NextChar == 'L')
                    {
                        if (flippedSide)
                        {
                            return CurrentPipeSide.GetOpposite();
                        }
                        else
                        {
                            return CurrentPipeSide;
                        }
                    }
                    else
                    {
                        return PipeSide.Outer;
                    }

                case Direction.Right:
                    if (NextChar == 'J' || NextChar == '7')
                    {
                        if (flippedSide)
                        {
                            return CurrentPipeSide.GetOpposite();
                        }
                        else
                        {
                            return CurrentPipeSide;
                        }

                    }
                    else
                    {
                        return PipeSide.Outer;
                    }

                case Direction.Up:
                    if (NextChar == '7' || NextChar == 'F')
                    {
                        if (flippedSide)
                        {
                            return CurrentPipeSide.GetOpposite();
                        }
                        else
                        {
                            return CurrentPipeSide;
                        }

                    }
                    else
                    {
                        return PipeSide.Outer;
                    }

                case Direction.Down:
                    if (NextChar == 'J' || NextChar == 'L')
                    {
                        if (flippedSide)
                        {
                            return CurrentPipeSide.GetOpposite();
                        }
                        else
                        {
                            return CurrentPipeSide;
                        }

                    }
                    else
                    {
                        return PipeSide.Outer;
                    }
            }

            throw new Exception();
        }

        private PipeSide HandleHorizontals()
        {
            if (CurrentChar == '-')
            {
                if ((CurrentPipeSide == PipeSide.Top && (NextChar == 'F' || NextChar == '7')) ||
                    (CurrentPipeSide == PipeSide.Bottom && (NextChar == 'L' || NextChar == 'J')))
                {
                    return PipeSide.Outer;
                }
                else
                {
                    return PipeSide.Inner;
                }
            }
            else
            {
                if ((CurrentPipeSide == PipeSide.Left && (NextChar == 'F' || NextChar == 'L')) ||
                (CurrentPipeSide == PipeSide.Right && (NextChar == '7' || NextChar == 'J')))
                {
                    return PipeSide.Outer;
                }
                else
                {
                    return PipeSide.Inner;
                }

            }
        }

        private bool IsFlippingSides()
        {
            return CurrentChar == 'F' && NextChar == 'J' ||
                CurrentChar == '7' && NextChar == 'L' ||
                CurrentChar == 'J' && NextChar == 'F' ||
                CurrentChar == 'L' && NextChar == '7';
        }
    }
}
