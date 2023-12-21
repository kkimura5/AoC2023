using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC
{
    internal class SpringGroup
    {
        private Dictionary<string, long> countsByString = new Dictionary<string, long>();
        public SpringGroup(string status, List<int> numbers) 
        {
            Status = status;
            Numbers = numbers;
        }

        public string Status { get; }
        public List<int> Numbers { get; }

        public long GetNumArrangements()
        {
            return Calculate(Numbers.First(), Numbers.Skip(1).ToList(), Status);
        }

        private long Calculate(int currentNumber, List<int> remainingNumbers, string remainingString)
        {
            if (countsByString.ContainsKey($"{remainingString}{currentNumber}{string.Join(",", remainingNumbers)}"))
            {
                return countsByString[$"{remainingString}{currentNumber}{string.Join(",", remainingNumbers)}"];
            }

            var pattern = @"(\?|#)+";
            var match = Regex.Match(remainingString, pattern);

            var total = currentNumber + remainingNumbers.Sum();
            long count = 0;
            while (match.Success)
            {
                var offset = 0;

                while (offset + currentNumber <= match.Length)
                {
                    if (remainingString.Substring(match.Index + offset).Count(x => x == '?' || x == '#') < total
                    || remainingString.Substring(match.Index + offset).Length < total + remainingNumbers.Count
                    || remainingString.Substring(0, match.Index + offset).Any(x => x == '#'))
                    {
                        break;
                    }

                    if (remainingString.Substring(match.Index + offset, currentNumber).All(x => x == '?' || x == '#'))
                    {
                        var newString = remainingString.Substring(match.Index + offset + currentNumber);
                        if (!newString.StartsWith("#"))
                        {
                            if (!remainingNumbers.Any())
                            {
                                if (!newString.Any(x => x == '#'))
                                {
                                    count += 1;
                                }
                            }
                            else
                            {
                                count += Calculate(remainingNumbers.First(), remainingNumbers.Skip(1).ToList(), newString.Substring(1));
                            }
                        }
                    }

                    offset++;
                }

                if (remainingString.Substring(match.Index + offset).Count(x => x == '?' || x == '#') < total
                    || remainingString.Substring(match.Index + offset).Length < total + remainingNumbers.Count
                    || remainingString.Substring(0, match.Index + offset).Any(x => x == '#'))
                {
                    break;
                }

                match = match.NextMatch();
            }

            countsByString.Add($"{remainingString}{currentNumber}{string.Join(",",remainingNumbers)}", count);
            return count;
        }

        public bool IsValid(string testValue)
        {
            var pattern = "#+";
            var matches = Regex.Matches(testValue, pattern);

            if (matches.Count != Numbers.Count)
            {
                return false; 
            }

            var index = 0;
            foreach (Match match in matches) 
            {
                if (!match.Success)
                {
                    return false;
                }
                else if (match.Value.Length != Numbers[index])
                {
                    return false;
                }

                index++;
            }

            return true;
        }

    }
}
