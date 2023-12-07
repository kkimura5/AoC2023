using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC
{
    internal class CubeGame
    {
        public CubeGame(string game)
        {
            GameNum = int.Parse(Regex.Match(game, "Game (?<gameNum>\\d+):").Groups["gameNum"].Value);
            MaxNumRed = ParseRedMatches(game, " (?<num>\\d+) red");
            MaxNumBlue = ParseRedMatches(game, " (?<num>\\d+) blue");
            MaxNumGreen = ParseRedMatches(game, " (?<num>\\d+) green");
        }

        private int ParseRedMatches(string game, string pattern)
        {
            var matches = Regex.Matches(game, pattern);
            var maxVal = 0;
            foreach (Match match in matches)
            {
                maxVal = Math.Max(maxVal, int.Parse(match.Groups["num"].Value));
            }

            return maxVal;
        }

        public int GameNum { get; set; }
        public int MaxNumRed { get; set; }
        public int MaxNumGreen { get; set;}
        public int MaxNumBlue { get; set;}
    }
}
