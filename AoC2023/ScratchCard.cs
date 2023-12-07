using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    internal class ScratchCard
    {
        public ScratchCard(int cardNum, List<int> winningNumbers, List<int> myNumbers)
        {
            CardNum = cardNum;
            WinningNumbers = winningNumbers;
            MyNumbers = myNumbers;
        }

        public int CardNum { get; }
        public List<int> WinningNumbers { get; }
        public List<int> MyNumbers { get; }
        public int Count { get; set; } = 1;
        public long CalculateScore()
        {
            int winners = GetNumWinners();
            return winners > 0 ? (long)Math.Pow(2, winners - 1) : 0;
        }

        public int GetNumWinners()
        {
            return MyNumbers.Count(x => WinningNumbers.Contains(x));
        }
    }
}
