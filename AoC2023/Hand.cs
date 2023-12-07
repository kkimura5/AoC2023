using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class Hand : IComparable<Hand>
    {
        //private string order = "23456789TJQKA"; 
        private string order = "J23456789TQKA"; 

        public Hand(string hand, long bid)
        {
            RawValue = hand;
            Bid = bid;
            string secondHand;
            var copy = hand.ToList();
            if (hand.Any(c => copy.All(i => i == c || i == 'J')))
            {
                Rank = 7;
            }
            else if(hand.Any(c => copy.Count(i => i == c || i == 'J') == 4))
            {
                Rank = 6;
            }
            else if (hand.Any(c => copy.Count(i => i == c || i == 'J') == 3))
            {
                if (hand.Any(x => x == 'J'))
                {
                    secondHand = hand.Replace('J', '\0');
                    var firstChar = hand.First(x => copy.Count(i => i == x || i == 'J') == 3);
                    secondHand = secondHand.Replace(firstChar, '\0');
                }
                else
                {
                    var firstChar = hand.First(x => copy.Count(i => i == x || i == 'J') == 3);
                    secondHand = hand.Replace(firstChar, '\0');
                }

                if (secondHand.Any(x => copy.Count(i => i != '\0' && i == x) == 2))
                {
                    Rank = 5;
                }
                else
                {
                    Rank = 4;
                }
            }
            else if (hand.Any(c => copy.Count(i => i == c || i == 'J') == 2))
            {
                if (hand.Any(x => x == 'J'))
                {
                    secondHand = hand.Replace('J', '\0');
                }
                else
                {
                    var firstChar = hand.First(x => copy.Count(i => i == x || i == 'J') == 2);
                    secondHand = hand.Replace(firstChar, '\0');
                }

                copy = secondHand.ToList();

                if (secondHand.Any(x => copy.Count(i => i != '\0' && (i == x || i == 'J')) == 2)) 
                {
                    Rank = 3;
                }
                else
                {
                    Rank = 2;
                }
            }
            else
            {
                Rank = 1;
            }
        }

        public string RawValue { get; }
        public long Bid { get; }
        public int Rank { get; }

        public int CompareTo(Hand other)
        {
            if (Rank > other.Rank)
            {
                return 1;
            } 
            else if (Rank < other.Rank)
            {
                return -1;
            }
            else
            {
                for ( int i = 0; i < RawValue.Length; i++ )
                {
                    if (order.IndexOf(RawValue[i]) > order.IndexOf(other.RawValue[i]))
                    {
                        return 1;
                    }
                    else if (order.IndexOf(RawValue[i]) < order.IndexOf(other.RawValue[i]))
                    {
                        return -1;
                    }
                    else
                    {
                        continue;
                    }
                }

                return 0;
            }
        }
    }
}
