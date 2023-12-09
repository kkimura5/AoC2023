using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RunDay1();
            RunDay2();
            RunDay3();
            RunDay4();
            RunDay5();
            RunDay6();
            RunDay7();
            RunDay8();
            RunDay9();
            Console.ReadKey();
        }

        private static void RunDay9()
        {
            var lines = File.ReadAllLines(".\\Input\\Day9.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day9_training.txt").ToList();
            var sequences = new List<OasisSequence>();
            foreach (var line in lines)
            {
                sequences.Add(new OasisSequence(line.Split(' ').Select(x => long.Parse(x)).ToList()));
            }

            var endValues = new List<long>();
            var startValues = new List<long>();
            foreach (var sequence in sequences)
            {
                var allSequences = new List<OasisSequence>() { sequence };
                OasisSequence currentSequence = sequence;
                do
                {
                    currentSequence = currentSequence.GetChildSequence();
                    allSequences.Add(currentSequence);
                } while (!allSequences.Last().IsEnd());

                allSequences.Reverse();

                long previous = 0;
                long finalValue = 0;
                for (var i = 1; i<allSequences.Count; i++)
                {
                    var childSequence = allSequences[i];

                    finalValue = childSequence.Values.Last() + previous;
                    previous = finalValue;
                }

                endValues.Add(finalValue);

                previous = 0;
                finalValue = 0;
                for (var i = 1; i < allSequences.Count; i++)
                {
                    var childSequence = allSequences[i];

                    finalValue = childSequence.Values.First() - previous;
                    previous = finalValue;
                }

                startValues.Add(finalValue);
            }

            Console.WriteLine($"Day 9 part 1: {endValues.Sum()}");
            Console.WriteLine($"Day 9 part 2: {startValues.Sum()}");
        }

        private static void RunDay8()
        {
            var lines = File.ReadAllLines(".\\Input\\Day8.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day8_training.txt").ToList();
            string sequence = lines[0];
            var instructions = lines.Skip(2).Select(x => new MapInstruction(x)).ToList();
            foreach (var instruction in instructions)
            {
                instruction.SetLeft(instructions.Single(x => x.Node == instruction.Left));
                instruction.SetRight(instructions.Single(x => x.Node == instruction.Right));
            }

            long numSteps = FindNumStepsToNode(sequence, instructions.Single(x => x.Node == "AAA"), "ZZZ");

            Console.WriteLine($"Day 8 part 1: {numSteps}");

            numSteps = 0;

            var currentInstructions = instructions.Where(x => x.Node.EndsWith("A")).ToList();
            var zinstructions = instructions.Where(x => x.Node.EndsWith("Z")).ToList();

            long numSteps2 = 1;
            foreach (var instruction in currentInstructions)
            {
                numSteps2 *= FindNumStepsToNode(sequence, instruction, "Z");
                while (((numSteps2 / sequence.Length) % sequence.Length) == 0)
                {
                    numSteps2 /= sequence.Length;
                }
            }

            Console.WriteLine($"Day 8 part 2: {numSteps2}");
        }

        private static long FindNumStepsToNode(string sequence, MapInstruction startingInstruction, string node)
        {
            var currentInstruction = startingInstruction;
            long numSteps = 0;
            var firstTime = true;
            while ((currentInstruction.Node != node && !currentInstruction.Node.EndsWith(node)) || firstTime)
            {
                firstTime = false;
                var move = sequence[(int)(numSteps % sequence.Length)];
                currentInstruction = move == 'R' ? currentInstruction.GetRightInstruction() : currentInstruction.GetLeftInstruction();

                numSteps++;
            }

            return numSteps;
        }

        private static void RunDay7()
        {
            var lines = File.ReadAllLines(".\\Input\\Day7.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day7_training.txt").ToList();
            long value = 0;
            var hands = new List<Hand>();
            foreach (var line in lines)
            {
                var items = line.Split(' ');
                var bid = long.Parse(items[1]);
                hands.Add(new Hand(items[0], bid));
            }
            
            value = hands.OrderBy(x => x).Select((x, i) => x.Bid * (i + 1)).Sum();

            Console.WriteLine($"Day 7: {value}");
        }

        private static void RunDay6()
        {
            //var lines = File.ReadAllLines(".\\Input\\Day6_training.txt").ToList();
            var lines = File.ReadAllLines(".\\Input\\Day6.txt").ToList();
            var times = new List<int>();
            var records = new List<int>();
            foreach (var line in lines)
            {
                if (line.StartsWith("Time"))
                {
                    var timeStrings = line.Substring(line.IndexOf(":") + 1).Trim().Split(' ');
                    times = timeStrings.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => int.Parse(x)).ToList();
                }
                else if (line.StartsWith("Distance"))
                {
                    var distanceStrings = line.Substring(line.IndexOf(":") + 1).Trim().Split(' ');
                    records = distanceStrings.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => int.Parse(x)).ToList();
                }
            }

            var scores = new List<long>();
            foreach (var time in times)
            {
                long score = 0;
                for (int i = 1; i < time; i++)
                {
                    var distance = (time - i) * i;
                    if (distance > records[times.IndexOf(time)])
                    {
                        score++;
                    }
                }

                scores.Add(score);
            }

            var longTime = long.Parse(string.Join(string.Empty, times));
            var longDistance = long.Parse(string.Join(string.Empty, records));
            long bigScore = 0;
            for (int i = 1; i < longTime; i++)
            {
                var distance = (longTime - i) * i;
                if (distance > longDistance)
                {
                    bigScore++;
                }
            }

            long totalScore = 1;
            scores.ForEach(x => totalScore *= x);
            Console.WriteLine($"Day 6 part 1: {totalScore}");
            Console.WriteLine($"Day 6 part 2: {bigScore}");
        }

        private static void RunDay5()
        {
            //var lines = File.ReadAllLines(".\\Input\\Day5_training.txt").ToList();
            var lines = File.ReadAllLines(".\\Input\\Day5.txt").ToList();
            var seedsPart1 = new List<long>();
            var seedsPart2 = new List<Seed>();
            Almanac seedToSoil = null;
            Almanac soilToFertilizer = null;
            Almanac fertilizerToWater = null;
            Almanac waterToLightMaps = null;
            Almanac lightToTemperatureMaps = null;
            Almanac temperatureToHumidity = null;
            Almanac humidityToLocation = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("seeds:"))
                {
                    seedsPart1 = line.Replace("seeds:", string.Empty).Trim().Split(' ').Select(x => long.Parse(x)).ToList();

                    for (int i = 0; i < seedsPart1.Count; i += 2)
                    {
                        seedsPart2.Add(new Seed(seedsPart1[i], seedsPart1[i + 1]));
                    }
                }

                if (line.StartsWith("seed-to-soil"))
                {
                    var maps = lines.Skip(lines.IndexOf(line) + 1).TakeWhile(x => Regex.Match(x, "\\d+").Success);
                    seedToSoil = new Almanac(maps.Select(x => Mapping.Create(x)).ToList());
                }
                else if (line.StartsWith("soil-to-fertilizer"))
                {
                    var maps = lines.Skip(lines.IndexOf(line) + 1).TakeWhile(x => Regex.Match(x, "\\d+").Success);
                    soilToFertilizer = new Almanac(maps.Select(x => Mapping.Create(x)).ToList());
                }
                else if (line.StartsWith("fertilizer"))
                {
                    var maps = lines.Skip(lines.IndexOf(line) + 1).TakeWhile(x => Regex.Match(x, "\\d+").Success);
                    fertilizerToWater = new Almanac(maps.Select(x => Mapping.Create(x)).ToList());
                }
                else if (line.StartsWith("water"))
                {
                    var maps = lines.Skip(lines.IndexOf(line) + 1).TakeWhile(x => Regex.Match(x, "\\d+").Success);
                    waterToLightMaps = new Almanac(maps.Select(x => Mapping.Create(x)).ToList());
                }
                else if (line.StartsWith("light"))
                {
                    var maps = lines.Skip(lines.IndexOf(line) + 1).TakeWhile(x => Regex.Match(x, "\\d+").Success);
                    lightToTemperatureMaps = new Almanac(maps.Select(x => Mapping.Create(x)).ToList());
                }
                else if (line.StartsWith("temperature"))
                {
                    var maps = lines.Skip(lines.IndexOf(line) + 1).TakeWhile(x => Regex.Match(x, "\\d+").Success);
                    temperatureToHumidity = new Almanac(maps.Select(x => Mapping.Create(x)).ToList());
                }
                else if (line.StartsWith("humidity"))
                {
                    var maps = lines.Skip(lines.IndexOf(line) + 1).TakeWhile(x => Regex.Match(x, "\\d+").Success);
                    humidityToLocation = new Almanac(maps.Select(x => Mapping.Create(x)).ToList());
                }
            }

            long value = long.MaxValue;
            foreach (var seed in seedsPart1)
            {
                var soil = seedToSoil.Convert(seed);
                var fertilizer = soilToFertilizer.Convert(soil);
                var water = fertilizerToWater.Convert(fertilizer);
                var light = waterToLightMaps.Convert(water);
                var temperature = lightToTemperatureMaps.Convert(light);
                var humidity = temperatureToHumidity.Convert(temperature);
                var location = humidityToLocation.Convert(humidity);
                value = Math.Min(location, value);
            }

            var soil2 = Convert(seedsPart2, seedToSoil);
            Console.WriteLine($"done soil num:{soil2.Count}");
            var fertilizer2 = Convert(soil2, soilToFertilizer);
            Console.WriteLine($"done fertilizer num:{fertilizer2.Count}");
            var water2 = Convert(fertilizer2, fertilizerToWater);
            Console.WriteLine($"done water num:{water2.Count}");
            var light2 = Convert(water2, waterToLightMaps);
            Console.WriteLine($"done light num:{light2.Count}");
            var temperature2 = Convert(light2, lightToTemperatureMaps);
            Console.WriteLine($"done temperature num:{temperature2.Count}");
            var humidity2 = Convert(temperature2, temperatureToHumidity);
            Console.WriteLine($"done humidity num:{humidity2.Count}");
            var location2 = Convert(humidity2, humidityToLocation);
            Console.WriteLine($"done location num:{location2.Count}");
            long value2 = location2.Min(x => x.Start);

            Console.WriteLine($"Day 5 part 1: {value}");
            Console.WriteLine($"Day 5 part 2: {value2}");
        }

        private static List<Seed> Convert(List<Seed> seeds, Almanac almanac)
        {
            var filteredSeeds = new List<Seed>();

            foreach (var seed in seeds)
            {
                var outputSeeds = almanac.Convert(seed);

                foreach (var outputSeed in outputSeeds)
                {
                    var toJoin = filteredSeeds.Where(x => Math.Abs(x.Start - outputSeed.MaxValue) <= 1 || Math.Abs(x.MaxValue - outputSeed.Start) <= 1);
                    filteredSeeds = filteredSeeds.Except(toJoin).ToList();
                    foreach (var seedToJoin in toJoin)
                    {
                        outputSeed.Join(seedToJoin);
                    }

                    filteredSeeds.Add(outputSeed);
                }
            }
            
            return filteredSeeds;
        }

        private static void RunDay4()
        {
            //var lines = File.ReadAllLines(".\\Input\\Day4_training.txt").ToList();
            var lines = File.ReadAllLines(".\\Input\\Day4.txt").ToList();
            var cards = new List<ScratchCard>();
            foreach (var line in lines)
            {
                var winningNumbers = line.Substring(line.IndexOf(":") + 1, line.IndexOf("|") - line.IndexOf(":") - 1).Trim().Split(' ')
                    .Select(x => string.IsNullOrEmpty(x.Trim()) ? 0 : int.Parse(x)).Where(x => x != 0).ToList();
                var myNumbers = line.Substring(line.IndexOf("|") + 1).Trim().Split(' ')
                    .Select(x => string.IsNullOrEmpty(x.Trim()) ? 0 : int.Parse(x)).Where(x => x != 0).ToList();
                var card = new ScratchCard(lines.IndexOf(line), winningNumbers, myNumbers);
                cards.Add(card);
            }

            foreach (var card in cards.OrderBy(x => x.CardNum))
            {
                var numWinners = card.GetNumWinners();
                for (int i = 1; i <= numWinners; i++)
                {
                    cards.Single(x => x.CardNum == card.CardNum + i).Count += card.Count;
                }
            }

            Console.WriteLine($"Day 4 part 1: {cards.Sum(x => x.CalculateScore())}");
            Console.WriteLine($"Day 4 part 2: {cards.Sum(x => x.Count)}");
        }


        private static void RunDay3()
        {
            //var lines = File.ReadAllLines(".\\Input\\Day3_training.txt").ToList();
            var lines = File.ReadAllLines(".\\Input\\Day3.txt").ToList();
            long total = 0;
            var engineNumbers = new List<EngineNumber>();
            var symbols = new List<EngineSymbol>();
            foreach (var line in lines)
            {
                int row = lines.IndexOf(line);
                var numberMatches = Regex.Matches(line, "\\d+");
                var symbolMatches = Regex.Matches(line, "\\D");
                foreach (Match match in numberMatches)
                {
                    var engineNumber = new EngineNumber(match.Value, match.Index, row);
                    foreach (var symbol in symbols)
                    {
                        bool isConnected = Math.Abs(row - symbol.Y) <= 1 &&
                                                    (Math.Abs(symbol.X - engineNumber.Column) <= 1 || Math.Abs(symbol.X - engineNumber.Column - engineNumber.Number.Length + 1) <= 1);
                        engineNumber.IsPartNumber |= isConnected;
                        if (isConnected)
                        {
                            symbol.ConnectedNumbers.Add(engineNumber);
                        }
                    }

                    engineNumbers.Add(engineNumber);
                }

                foreach (Match match in symbolMatches)
                {
                    if (match.Value == ".")
                    {
                        continue;
                    }

                    var symbol = new EngineSymbol(match.Value.Single(), match.Index, row);
                    symbols.Add(symbol);
                    foreach (var engineNumber in engineNumbers)
                    {
                        bool isConnected = Math.Abs(engineNumber.Row - row) <= 1 &&
                                                    (Math.Abs(match.Index - engineNumber.Column) <= 1 || Math.Abs(match.Index - engineNumber.Column - engineNumber.Number.Length + 1) <= 1);
                        engineNumber.IsPartNumber |= isConnected;
                        if (isConnected && !symbol.ConnectedNumbers.Contains(engineNumber))
                        {
                            symbol.ConnectedNumbers.Add(engineNumber);
                        }
                    }
                }
            }

            foreach (var symbol in symbols)
            {
                if (symbol.IsStar && symbol.ConnectedNumbers.Count == 2)
                {
                    total += symbol.ConnectedNumbers.First().Value * symbol.ConnectedNumbers.Last().Value;
                }
            }
            Console.WriteLine($"Day 3 part 1: {engineNumbers.Where(x => x.IsPartNumber).Sum(x => x.Value)}");
            Console.WriteLine($"Day 3 part 2: {total}");
        }

        private static void RunDay2()
        {
            //var lines = File.ReadAllLines(".\\Input\\Day2_training.txt").ToList();
            var lines = File.ReadAllLines(".\\Input\\Day2.txt").ToList();
            long total = 0;
            long power = 0;
            foreach (var line in lines)
            {
                var cubeGame = new CubeGame(line);
                if (cubeGame.MaxNumRed <= 12 && cubeGame.MaxNumGreen <= 13 && cubeGame.MaxNumBlue <= 14)
                {
                    total += cubeGame.GameNum;
                }

                int currentPower = cubeGame.MaxNumBlue * cubeGame.MaxNumGreen * cubeGame.MaxNumRed;
                power += currentPower;
            }

            Console.WriteLine($"Day 2 part 1: {total}");
            Console.WriteLine($"Day 2 part 2: {power}");
        }

        private static void RunDay1()
        {
            //var lines = File.ReadAllLines(".\\Input\\Day1_training.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day1_training2.txt").ToList();
            var lines = File.ReadAllLines(".\\Input\\Day1.txt").ToList();

            var numbers = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
                { "four", 4 },
                { "five", 5 },
                { "six", 6 },
                { "seven", 7 },
                { "eight", 8 },
                { "nine", 9 },
            };

            long sum = 0;
            foreach (var line in lines)
            {
                var digits = string.Empty;
                var temp = line.ToLower();
                while (true)
                {
                    var match = Regex.Match(temp, "\\d|one|two|three|four|five|six|seven|eight|nine");
                    if (!match.Success)
                    {
                        break;
                    }

                    if (numbers.ContainsKey(match.Value))
                    {
                        digits += numbers[match.Value];
                    }
                    else
                    {
                        digits += match.Value;
                    }

                    temp = temp.Substring(0, match.Index) + temp.Substring(match.Index + 1);
                }

                if (digits.Length == 1)
                {
                    digits += digits;
                }

                long value = long.Parse($"{digits.First()}{digits.Last()}");
                sum += value;
            }

            Console.WriteLine($"Day 1: {sum}");
        }
    }
}
