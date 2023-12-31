﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AoC
{
    internal class Program
    {
        private static List<string> previousValues = new List<string>();
        
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
            RunDay10();
            RunDay11();
            RunDay12();
            RunDay13();
            RunDay14();
            RunDay15();
            //RunDay16();
            //RunDay17Part1();
            RunDay17Part2();
            Console.ReadKey();
        }

        private static void RunDay17Part2()
        {
            var lines = File.ReadAllLines(".\\Input\\Day17.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day17_training.txt").ToList();
            var charGrid = new CharGrid(lines);
            var paths = new List<LavaPath>();
            paths.Add(new LavaPath(new RowCol(0, 0), 0, new List<Direction>(), new List<string>()));
            var minScoreByLocation = new Dictionary<string, int>();
            minScoreByLocation.Add($"{paths.First().GetSummary()}", 0);
            var finalLocation = new RowCol(charGrid.MaxRow, charGrid.MaxCol);
            var stepCount = 0;
            while (paths.Any())
            {
                var newPaths = new List<LavaPath>();
                var fasterPathsFound = new List<string>();
                foreach (var path in paths)
                {
                    if (fasterPathsFound.Intersect(path.PreviousLocations).Any())
                    {
                        continue;
                    }

                    var directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
                    directions.Remove(Direction.None);
                    switch (path.PreviousDirections.FirstOrDefault())
                    {
                        case Direction.Up:
                            directions.Remove(Direction.Down);
                            break;

                        case Direction.Down:
                            directions.Remove(Direction.Up);
                            break;

                        case Direction.Left:
                            directions.Remove(Direction.Right);
                            break;

                        case Direction.Right:
                            directions.Remove(Direction.Left);
                            break;
                    }

                    foreach (var direction in directions)
                    {
                        if (path.PreviousDirections.Count < 4)
                        {
                            if (path.PreviousDirections.Any() && direction != path.PreviousDirections.First())
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (path.PreviousDirections.Count >= 10 && path.PreviousDirections.Take(10).All(x => x == direction))
                            {
                                // max of 10
                                continue;
                            }

                            if (direction == path.PreviousDirections.First() || path.PreviousDirections.Take(4).All(x => x == path.PreviousDirections.First()))
                            {
                                // at least 4 before changing, or stay in the same direction
                            }
                            else
                            {
                                continue;
                            }
                        }

                        var location = path.CurrentLocation.GetNext(direction);
                        if (charGrid.IsLocationWithin(location))
                        {
                            int newScore = path.Score + int.Parse($"{charGrid[location.Row, location.Col]}");
                            var previousDirections = path.PreviousDirections.Take(10).ToList();
                            previousDirections.Insert(0, direction);

                            var previousLocations = path.PreviousLocations.ToList();
                            var newPath = new LavaPath(location, newScore, previousDirections, previousLocations);
                            string newPathSummary = newPath.GetSummary();
                            previousLocations.Add(newPathSummary);

                            if (location == finalLocation)
                            {
                                if (newPath.PreviousDirections.Take(4).All(x => x == newPath.PreviousDirections.First()))
                                {
                                    if (minScoreByLocation.ContainsKey(newPathSummary))
                                    {
                                        if (newScore < minScoreByLocation[newPathSummary])
                                        {
                                            minScoreByLocation[newPathSummary] = newPath.Score;
                                        }
                                    }
                                    else
                                    {
                                        minScoreByLocation.Add(newPathSummary, newPath.Score);
                                    }
                                }
                            }
                            else
                            {
                                if (minScoreByLocation.ContainsKey(newPathSummary))
                                {
                                    if (newScore < minScoreByLocation[newPathSummary])
                                    {
                                        minScoreByLocation[newPathSummary] = newPath.Score;
                                        newPaths = newPaths.Where(x => !x.PreviousLocations.Contains(newPathSummary) && x.GetSummary() != newPathSummary).ToList();
                                        fasterPathsFound.Add(newPathSummary);
                                        newPaths.Add(newPath);
                                    }
                                }
                                else
                                {
                                    minScoreByLocation.Add(newPathSummary, newPath.Score);
                                    newPaths.Add(newPath);
                                }
                            }
                        }
                    }
                }

                paths.Clear();
                if (newPaths.Count > 2000)
                {
                    var maxDistance = newPaths.Max(x => x.CurrentLocation.Col + x.CurrentLocation.Row);
                    var minDistance = newPaths.Min(x => x.CurrentLocation.Col + x.CurrentLocation.Row);
                    if (minDistance < 0.6 * maxDistance)
                    {
                        newPaths = newPaths.Where(x => x.CurrentLocation.Col + x.CurrentLocation.Row > minDistance).ToList();
                    }
                    //newPaths = newPaths.OrderBy(x => x.Score / (x.CurrentLocation.Col + x.CurrentLocation.Row)).Take((int)(newPaths.Count() * 0.95)).ToList();
                }

                paths = newPaths.Where(x => x.CurrentLocation != finalLocation).ToList();
                var roughLocation = newPaths.Any() ? newPaths.First().CurrentLocation.Row + newPaths.First().CurrentLocation.Col : 0;
                Console.WriteLine($"step {++stepCount}: Reached: {roughLocation}; numPaths {paths.Count}");
            }

            var finalLocationScores = minScoreByLocation.Where(x => x.Key.Contains(finalLocation.ToString())).ToList();
            Console.WriteLine($"Day 17 part 2: {finalLocationScores.Min(x => x.Value)}");
        }

        private static void RunDay17Part1()
        {
            var lines = File.ReadAllLines(".\\Input\\Day17.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day17_training.txt").ToList();
            var charGrid = new CharGrid(lines);
            var paths = new List<LavaPath>();
            paths.Add(new LavaPath(new RowCol(0, 0), 0, new List<Direction>(), new List<string>()));
            var minScoreByLocation = new Dictionary<string, int>();
            minScoreByLocation.Add($"{paths.First().GetSummary()}", 0);
            var finalLocation = new RowCol(charGrid.MaxRow, charGrid.MaxCol);
            while (paths.Any())
            {
                var newPaths = new List<LavaPath>();
                var fasterPathsFound = new List<string>();
                foreach (var path in paths)
                {
                    if (fasterPathsFound.Intersect(path.PreviousLocations).Any())
                    {
                        continue;
                    }
                    
                    var directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
                    directions.Remove(Direction.None);
                    switch (path.PreviousDirections.FirstOrDefault())
                    {
                        case Direction.Up:
                            directions.Remove(Direction.Down);
                            break;

                        case Direction.Down:
                            directions.Remove(Direction.Up);
                            break;

                        case Direction.Left:
                            directions.Remove(Direction.Right);
                            break;
                        
                        case Direction.Right:
                            directions.Remove(Direction.Left);
                            break;
                    }

                    foreach (var direction in directions)
                    {
                        if (path.PreviousDirections.Count >= 3 && path.PreviousDirections.Take(3).All(x => x == direction))
                        {
                            continue;
                        }

                        var location = path.CurrentLocation.GetNext(direction);
                        if (charGrid.IsLocationWithin(location))
                        {
                            int newScore = path.Score + int.Parse($"{charGrid[location.Row, location.Col]}");
                            var previousDirections = path.PreviousDirections.Take(2).ToList();
                            previousDirections.Insert(0, direction);

                            var previousLocations = path.PreviousLocations.ToList();
                            var newPath = new LavaPath(location, newScore, previousDirections, previousLocations);
                            string newPathSummary = newPath.GetSummary();
                            previousLocations.Add(newPathSummary);

                            if (minScoreByLocation.ContainsKey(newPathSummary))
                            {
                                if (newScore < minScoreByLocation[newPathSummary])
                                {
                                    minScoreByLocation[newPathSummary] = newPath.Score;
                                    newPaths = newPaths.Where(x => !x.PreviousLocations.Contains(newPathSummary) && x.GetSummary() != newPathSummary).ToList();
                                    fasterPathsFound.Add(newPathSummary);
                                    newPaths.Add(newPath);
                                }
                            }
                            else
                            {
                                minScoreByLocation.Add(newPathSummary, newPath.Score);
                                newPaths.Add(newPath);
                            }
                        }
                    }
                }

                paths.Clear();
                paths = newPaths.Where(x => x.CurrentLocation != finalLocation).ToList();
                var minLocation = newPaths.Any() ? newPaths.First().CurrentLocation.Row + newPaths.First().CurrentLocation.Col : 0;
                Console.WriteLine($"Reached: {minLocation}; numPaths {paths.Count}");
            }

            var finalLocationScores = minScoreByLocation.Where(x => x.Key.Contains(finalLocation.ToString())).ToList();
            Console.WriteLine($"Day 17 part 1: {finalLocationScores.Min(x=> x.Value)}");
        }

        private static void RunDay16()
        {
            var lines = File.ReadAllLines(".\\Input\\Day16.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day16_training.txt").ToList();

            var total1 = SimulateBeamPath(lines, new Tuple<RowCol, Direction>(new RowCol(0, 0), Direction.Right));
            Console.WriteLine($"Day 16 part 1: {total1}");

            var maxValue = 0;
            for (int r = 0; r < lines.Count; r++)
            {
                maxValue = Math.Max(SimulateBeamPath(lines, new Tuple<RowCol, Direction>(new RowCol(r, 0), Direction.Right)), maxValue);
                maxValue = Math.Max(SimulateBeamPath(lines, new Tuple<RowCol, Direction>(new RowCol(r, lines[0].Length - 1), Direction.Left)), maxValue);
            }

            for (int c = 0; c < lines.Count; c++)
            {
                maxValue = Math.Max(SimulateBeamPath(lines, new Tuple<RowCol, Direction>(new RowCol(0, c), Direction.Down)), maxValue);
                maxValue = Math.Max(SimulateBeamPath(lines, new Tuple<RowCol, Direction>(new RowCol(lines.Count-1, c), Direction.Up)), maxValue);

            }

            Console.WriteLine($"Day 16 part 2: {maxValue}");
        }

        private static int SimulateBeamPath(List<string> lines, Tuple<RowCol, Direction> initialLocation)
        {
            var charGrid = new CharGrid(lines);
            var statusGrid = new CharGrid(lines);

            var beamPaths = new List<Tuple<RowCol, Direction>>() { initialLocation };
            var cachedLocations = new List<Tuple<RowCol, Direction>>();

            while (beamPaths.Any())
            {
                var newBeamPaths = new List<Tuple<RowCol, Direction>>();
                foreach (var beamPath in beamPaths)
                {
                    cachedLocations.Add(beamPath);
                    var currentChar = charGrid[beamPath.Item1.Row, beamPath.Item1.Col];
                    statusGrid.SetValue(beamPath.Item1, '#');
                    switch (currentChar)
                    {
                        case '.':
                            var currentDirection = beamPath.Item2;
                            var nextLocation = beamPath.Item1.GetNext(currentDirection);
                            var nextDirection = currentDirection;
                            if (charGrid.IsLocationWithin(nextLocation))
                            {
                                newBeamPaths.Add(new Tuple<RowCol, Direction>(nextLocation, currentDirection));
                            }

                            break;

                        case '|':
                        case '-':
                            var possibleBeamPaths = HandleSplitter(currentChar, beamPath);
                            newBeamPaths.AddRange(possibleBeamPaths.Where(x => charGrid.IsLocationWithin(x.Item1)));
                            break;

                        case '\\':
                        case '/':
                            var newBeamPath = HandleReflector(currentChar, beamPath);
                            if (charGrid.IsLocationWithin(newBeamPath.Item1))
                            {
                                newBeamPaths.Add(newBeamPath);
                            }
                            break;
                    }
                }

                beamPaths.Clear();
                foreach (var beamPath in newBeamPaths)
                {
                    if (!cachedLocations.Any(x => x.Item1 == beamPath.Item1 && x.Item2 == beamPath.Item2))
                    {
                        beamPaths.Add(beamPath);
                    }
                }
            }

            return statusGrid.ToString().Count(x => x == '#');
        }

        private static IEnumerable<Tuple<RowCol, Direction>> HandleSplitter(char currentChar, Tuple<RowCol, Direction> beamPath)
        {
            var currentLocation = beamPath.Item1;
            var currentDirection = beamPath.Item2;
            var newBeamPaths = new List<Tuple<RowCol, Direction>>();

            if (currentChar == '|')
            {
                if (currentDirection == Direction.Up || currentDirection == Direction.Down) 
                {
                    newBeamPaths.Add(new Tuple<RowCol, Direction>(beamPath.Item1.GetNext(currentDirection), currentDirection));
                }
                else
                {
                    newBeamPaths.Add(new Tuple<RowCol, Direction>(currentLocation.GetNext(Direction.Up), Direction.Up));
                    newBeamPaths.Add(new Tuple<RowCol, Direction>(currentLocation.GetNext(Direction.Down), Direction.Down));
                }
            }
            else
            {
                if (currentDirection == Direction.Right || currentDirection == Direction.Left)
                {
                    newBeamPaths.Add(new Tuple<RowCol, Direction>(currentLocation.GetNext(currentDirection), currentDirection));
                }
                else
                {
                    newBeamPaths.Add(new Tuple<RowCol, Direction>(currentLocation.GetNext(Direction.Right), Direction.Right));
                    newBeamPaths.Add(new Tuple<RowCol, Direction>(currentLocation.GetNext(Direction.Left), Direction.Left));
                }
            }

            return newBeamPaths;
        }

        private static Tuple<RowCol, Direction> HandleReflector(char currentChar, Tuple<RowCol, Direction> beamPath)
        {
            RowCol nextLocation;
            Direction nextDirection;
            Tuple<RowCol, Direction> newBeamPath = null;
            switch (beamPath.Item2)
            {
                case Direction.Up:
                    nextDirection = currentChar == '\\' ?  Direction.Left : Direction.Right;
                    nextLocation = beamPath.Item1.GetNext(nextDirection);
                    newBeamPath = new Tuple<RowCol, Direction>(nextLocation, nextDirection);
                    break;

                case Direction.Left:
                    nextDirection = currentChar == '\\' ? Direction.Up : Direction.Down;
                    nextLocation = beamPath.Item1.GetNext(nextDirection);
                    newBeamPath = new Tuple<RowCol, Direction>(nextLocation, nextDirection);
                    break;

                case Direction.Right:
                    nextDirection = currentChar == '\\' ? Direction.Down : Direction.Up;
                    nextLocation = beamPath.Item1.GetNext(nextDirection);
                    newBeamPath = new Tuple<RowCol, Direction>(nextLocation, nextDirection);
                    break;

                case Direction.Down:
                    nextDirection = currentChar == '\\' ? Direction.Right : Direction.Left;
                    nextLocation = beamPath.Item1.GetNext(nextDirection);
                    newBeamPath = new Tuple<RowCol, Direction>(nextLocation, nextDirection);
                    break;
            }

            return newBeamPath;
        }

        private static void RunDay15()
        {
            var lines = File.ReadAllLines(".\\Input\\Day15.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day15_training.txt").ToList();

            var list = lines.First().Split(',').ToList();

            long sum = 0;
            var boxes = new List<LensBox>();
            foreach (var item in list)
            {
                long value = 0;
                foreach (char c in item)
                {
                    value += (int)c;
                    value *= 17;
                    value %= 256;
                }

                sum += value;

                var match = Regex.Match(item, "-|=");
                var boxNumber = 0;
                foreach(char c in item.Substring(0, match.Index))
                {
                    boxNumber += (int)c;
                    boxNumber *= 17;
                    boxNumber %= 256;

                    if (!boxes.Any(x => x.BoxNumber == boxNumber))
                    {
                        boxes.Add(new LensBox((int)boxNumber));
                    }
                }

                var box = boxes.Single(x => x.BoxNumber == boxNumber);
                string label = item.Substring(0, match.Index);
                
                if (match.Success)
                {
                    switch (match.Value)
                    {
                        case "-":
                            box.Lenses = box.Lenses.Where(x => x.Label != label).ToList(); 
                            break;

                        case "=":
                            var newLens = new Lens() { Label = label, FocalLength = int.Parse(item.Substring(match.Index + 1)) };
                            if (box.Lenses.Any(x => x.Label == label))
                            {
                                var oldLens = box.Lenses.Single(x => x.Label == label);
                                var index = box.Lenses.IndexOf(oldLens);
                                box.Lenses.Remove(oldLens);
                                box.Lenses.Insert(index, newLens);
                            }
                            else
                            {
                                box.Lenses.Add(newLens);
                            }
                            break;
                    }
                }
            }

            Console.WriteLine($"Day 15 Part 1: {sum}");
            long lensValue = 0;
            foreach (var box in boxes)
            {
                foreach (var lens in box.Lenses)
                {
                    var lensIndex = box.Lenses.IndexOf(lens) + 1;
                    lensValue += (box.BoxNumber + 1) * lensIndex * lens.FocalLength;
                }
            }

            Console.WriteLine($"Day 15 Part 2: {lensValue}");
        }

        private static void RunDay14()
        {
            var lines = File.ReadAllLines(".\\Input\\Day14.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day14_training.txt").ToList();
            var grid = new CharGrid(lines);
            Roll(grid, Direction.Up);
            long total = CalculateWeight(grid);

            Console.WriteLine($"Day 14 Part 1: {total}");

            grid = new CharGrid(lines);
            var directions = new List<Direction>() { Direction.Up, Direction.Left, Direction.Down, Direction.Right };
            long totalCycles = 1000000000;
            long i = 0;
            while (i < totalCycles)
            {
                foreach (var direction in directions)
                {
                    Roll(grid, direction);
                }
                
                i++;

                if (previousValues.Contains(grid.ToString()))
                {
                    var previousIndex = previousValues.IndexOf(grid.ToString()) + 1;
                    var currentIndex = i;
                    Console.WriteLine($"found match after {i} rolls, previously at {previousIndex}");
                    while (i < totalCycles)
                    {
                        i += currentIndex - previousIndex;
                    }

                    i -= currentIndex - previousIndex;

                    Console.WriteLine($"now at index {i}");
                    previousValues.Clear();
                }
                else
                {
                    previousValues.Add(grid.ToString());
                }

                //if ( i <= 3)
                //{
                //    Console.WriteLine($"After {i} cycles: ");
                //    for (int r = 0; r <= grid.MaxRow; r++)
                //    {
                //        Console.WriteLine(grid.GetRow(r));
                //    }
                //}
            }

            total = CalculateWeight(grid);
            Console.WriteLine($"Day 14 Part 2: {total}");
        }

        private static void Roll(CharGrid grid, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                case Direction.Down:
                    for (int c = 0; c <= grid.MaxCol; c++)
                    {
                        var column = grid.GetCol(c);
                        var match = Regex.Match(column, "#");
                        var cubeRockIndices = new List<int>();
                        while (match.Success)
                        {
                            cubeRockIndices.Add(match.Index);
                            match = match.NextMatch();
                        }

                        var marker = 0;
                        var segments = new List<string>();
                        foreach (var cubeRockIndex in cubeRockIndices)
                        {
                            var segment = column.Substring(marker, cubeRockIndex - marker);
                            string output1 = $"{string.Concat(direction == Direction.Up ? segment.OrderByDescending(x => x) : segment.OrderBy(x => x))}";
                            segments.Add($"{output1}#");
                            marker = cubeRockIndex + 1;
                        }

                        var finalSegment = column.Substring(marker);
                        string output = $"{string.Concat(direction == Direction.Up ? finalSegment.OrderByDescending(x => x) : finalSegment.OrderBy(x => x))}";
                        segments.Add(output);

                        string newValue = string.Join(string.Empty, segments);
                        grid.SetCol(c, newValue);
                    }
                    break;

                case Direction.Left:
                case Direction.Right:
                    for (int r = 0; r <= grid.MaxRow; r++)
                    {
                        var row = grid.GetRow(r);
                        var match = Regex.Match(row, "#");

                        var cubeRockIndices = new List<int>();
                        while (match.Success)
                        {
                            cubeRockIndices.Add(match.Index);
                            match = match.NextMatch();
                        }

                        var marker = 0;
                        var segments = new List<string>();
                        foreach (var cubeRockIndex in cubeRockIndices)
                        {
                            var segment = row.Substring(marker, cubeRockIndex - marker);
                            string output1 = $"{string.Concat(direction == Direction.Left ? segment.OrderByDescending(x => x) : segment.OrderBy(x => x))}";
                            segments.Add($"{output1}#");
                            marker = cubeRockIndex + 1;
                        }

                        var finalSegment = row.Substring(marker);
                        string output = $"{string.Concat(direction == Direction.Left ? finalSegment.OrderByDescending(x => x) : finalSegment.OrderBy(x => x))}";
                        segments.Add(output);

                        string newValue = string.Join(string.Empty, segments);
                        grid.SetRow(r, newValue);
                    }

                    break;

            }
        }

        private static long CalculateWeight(CharGrid grid)
        {
            long total = 0;
            for (int c = 0; c <= grid.MaxCol; c++)
            {
                var column = grid.GetCol(c);
                for (int r = 0; r < column.Length; r++)
                {
                    if (column[r] == 'O')
                    {
                        total += column.Length - r;
                    }
                }
            }

            return total;
        }

        private static void RunDay13()
        {
            var lines = File.ReadAllLines(".\\Input\\Day13.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day13_training.txt").ToList();
            var collection = new List<string>();
            var images = new List<CharGrid>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) && collection.Any())
                {
                    images.Add(new CharGrid(collection));
                    collection = new List<string>();
                }
                else
                {
                    collection.Add(line);
                }
            }
            
            images.Add(new CharGrid(collection));

            long sum = 0;
            var axes = new Dictionary<int, RowCol>();
            foreach (var image in images)
            {
                var index = images.IndexOf(image);
                for (int r = 1; r <= image.MaxRow; r++)
                {
                    if (IsMirrored(image.MaxRow, image.GetRow, r))
                    {
                        sum += 100 * r;
                        axes[index] = new RowCol(r, 0);
                    }
                }

                for (int c = 1; c <= image.MaxCol; c++)
                {
                    if (IsMirrored(image.MaxCol, image.GetCol, c))
                    {
                        sum += c;
                        axes[index] = new RowCol(0, c);
                    }
                }
            }

            Console.WriteLine($"Day 13 Part 1: {sum}");

            long sum2 = 0;
            foreach (var image in images)
            {
                var index = images.IndexOf(image);
                var isFound = false;
                for (int smudgeRow = 0; smudgeRow <= image.MaxRow; smudgeRow++)
                {
                    if (isFound)
                    {
                        break;
                    }

                    for (int smudgeCol = 0; smudgeCol <= image.MaxCol; smudgeCol++)
                    {
                        if (isFound)
                        {
                            break;
                        }

                        var prevValue = image[smudgeRow, smudgeCol];
                        image.SetValue(new RowCol(smudgeRow, smudgeCol), prevValue == '.' ? '#' : '.');
                        for (int r = 1; r <= image.MaxRow; r++)
                        {
                            if (IsMirrored(image.MaxRow, image.GetRow, r) && r != axes[index].Row)
                            {
                                sum2 += 100 * r;
                                isFound = true;
                                break;
                            }
                        }


                        if (!isFound)
                        {
                            for (int c = 1; c <= image.MaxCol; c++)
                            {
                                if (IsMirrored(image.MaxCol, image.GetCol, c) && c != axes[index].Col)
                                {
                                    sum2 += c;
                                    isFound = true;
                                    break;
                                }
                            }
                        }

                        image.SetValue(new RowCol(smudgeRow, smudgeCol), prevValue);

                    }
                }
            }

            Console.WriteLine($"Day 13 Part 2: {sum2}");
        }

        private static bool IsMirrored(int maxValue, Func<int, string> getLineDelegate, int axisIndex)
        {
            var isMirroredBetween = true;
            for (int i = 1; i <= axisIndex; i++)
            {
                if (axisIndex + i - 1 > maxValue)
                {
                    break;
                }

                string str1 = getLineDelegate(axisIndex - i);
                string str2 = getLineDelegate(axisIndex + i - 1);
                if (str1 != str2)
                {
                    isMirroredBetween = false;
                }
            }

            return isMirroredBetween;
        }

        private static void RunDay12()
        {
            var lines = File.ReadAllLines(".\\Input\\Day12.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day12_training.txt").ToList();
            long sum = 0;
            long sum2 = 0;
            foreach (var line in lines)
            {
                var sections = line.Split(' ');
                var numbers = sections[1].Trim().Split(',').Select(x => int.Parse(x)).ToList();
                string status = sections[0];

                var springGroup = new SpringGroup(status, numbers);
                var numArrangements1 = springGroup.GetNumArrangements();
                sum += numArrangements1;

                var extraGroup = new SpringGroup($"{status}?{status}?{status}?{status}?{status}", Enumerable.Repeat(numbers,5).SelectMany(x => x).ToList());
                var numArrangements3 = extraGroup.GetNumArrangements();

                sum2 += extraGroup.GetNumArrangements();
            }

            Console.WriteLine($"Day 12 Part 1: {sum}");
            Console.WriteLine($"Day 12 Part 2: {sum2}");
        }

        private static void RunDay11()
        {
            var lines = File.ReadAllLines(".\\Input\\Day11.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day11_training.txt").ToList();
            var charGrid = new CharGrid(lines);
            var planets = new List<RowCol>();
            var doubledRows = new List<int>();
            var doubledColumns = new List<int>();

            for (int r = 0; r <= charGrid.MaxRow; r++)
            {
                if (!charGrid.GetRow(r).Contains('#'))
                {
                    doubledRows.Add(r);
                }

                for (int c = 0; c <= charGrid.MaxCol; c++)
                {
                    if (!charGrid.GetCol(c).Contains('#') && !doubledColumns.Contains(c))
                    {
                        doubledColumns.Add(c);
                    }

                    if (charGrid[r, c] == '#')
                    {
                        planets.Add(new RowCol(r, c));
                    }
                }
            }

            long sum = SumPlanetDistances(planets, doubledRows, doubledColumns, 1);
            Console.WriteLine($"Day 11 Part 1: {sum}");
            sum = SumPlanetDistances(planets, doubledRows, doubledColumns, 999999);
            Console.WriteLine($"Day 11 Part 2 (1000000): {sum}");
        }

        private static long SumPlanetDistances(List<RowCol> planets, List<int> doubledRows, List<int> doubledColumns, long multiplier)
        {
            long sum = 0;
            for (int i = 0; i < planets.Count; i++)
            {
                var selectedPlanet = planets[i];
                for (int j = i + 1; j < planets.Count; j++)
                {
                    var otherPlanet = planets[j];
                    var difference = otherPlanet - selectedPlanet;
                    sum += Math.Abs(difference.Col) + Math.Abs(difference.Row);

                    var startRow = Math.Min(otherPlanet.Row, selectedPlanet.Row);
                    var endRow = Math.Max(otherPlanet.Row, selectedPlanet.Row);
                    sum += Enumerable.Range(startRow, endRow - startRow).Intersect(doubledRows).Count() * multiplier;

                    var startCol = Math.Min(otherPlanet.Col, selectedPlanet.Col);
                    var endCol = Math.Max(otherPlanet.Col, selectedPlanet.Col);
                    sum += Enumerable.Range(startCol, endCol - startCol).Intersect(doubledColumns).Count() * multiplier;
                }
            }

            return sum;
        }

        private static void RunDay10()
        {
            var lines = File.ReadAllLines(".\\Input\\Day10.txt").ToList();
            //var lines = File.ReadAllLines(".\\Input\\Day10_training.txt").ToList();

            var charGrid = new CharGrid(lines);
            var start = charGrid.FindChar('S');

            var paths = new List<List<RowCol>>();
            var firstPath = new List<RowCol>() { start };
            var finalPath = new List<RowCol>();
            paths.Add(firstPath);

            while (paths.Any())
            {
                var newPaths = new List<List<RowCol>>();
                foreach (var path in paths)
                {
                    var lastLocation = path.Last();
                    var lastPipe = charGrid[lastLocation.Row, lastLocation.Col];

                    var nextLocations = new List<RowCol>()
                    {
                        new RowCol(lastLocation.Row+1, lastLocation.Col),
                        new RowCol(lastLocation.Row-1, lastLocation.Col),
                        new RowCol(lastLocation.Row, lastLocation.Col + 1),
                        new RowCol(lastLocation.Row, lastLocation.Col - 1)
                    };

                    nextLocations = nextLocations.Where(l => l.Row <= charGrid.MaxRow && l.Col <= charGrid.MaxCol).ToList();
                    nextLocations = nextLocations.Where(l => l.Row >= 0 && l.Col >= 0).ToList();

                    foreach (var nextLocation in nextLocations)
                    {
                        var nextPipe = charGrid[nextLocation.Row, nextLocation.Col];
                        var direction = (nextLocation - lastLocation).GetDirection();

                        if (path.Count > 1)
                        {
                            var reverseDirection = (path[path.Count - 2] - lastLocation).GetDirection();
                            if (direction == reverseDirection)
                            {
                                continue;
                            }
                        }

                        if (IsPipeValid(lastPipe, nextPipe, direction))
                        {
                            var newPath = path.ToList();
                            newPath.Add(nextLocation);
                            newPaths.Add(newPath);
                        }
                    }
                }

                paths.Clear();
                paths.AddRange(newPaths);

                if (paths.Any(x => charGrid[x.Last().Row, x.Last().Col] == 'S'))
                {
                    finalPath = paths.First(x => charGrid[x.Last().Row, x.Last().Col] == 'S');
                    break;
                }
            }

            Console.WriteLine($"Day 10 Part 1: {finalPath.Count / 2}");

            var size = Math.Max(charGrid.MaxCol+ 2,charGrid.MaxRow + 2);
            var newCharGrid = new CharGrid(Enumerable.Repeat(string.Join("", Enumerable.Repeat('.', size)), size).ToList());
            foreach (var location in finalPath)
            {
                newCharGrid.SetValue(location, charGrid[location.Row, location.Col]);
            }

            int count = 0;
            for (int r = 0; r <= newCharGrid.MaxRow; r++)
            {
                var isInside = false;
                var previousChar = '.';
                var currentPipeSide = PipeSide.None;
                for (int c = 0; c <= newCharGrid.MaxCol; c++)
                {
                    var currentChar = newCharGrid[r, c];

                    if (previousChar == '|')
                    {
                        isInside = !isInside;
                    }
                    else if ((previousChar == '7' || previousChar == 'J') && currentPipeSide == PipeSide.Inner)
                    {
                        isInside = !isInside;
                    }

                    if (currentChar == '.' && isInside)
                    {
                        count++;
                    }

                    var pipeSideSorter = new PipeSideSorter(currentPipeSide, previousChar, currentChar, Direction.Right);
                    currentPipeSide = pipeSideSorter.GetNextPipeSide();
                    previousChar = currentChar;
                }
            }

            Console.WriteLine($"Day 10 Part 2: {count}");
        }

        private static bool IsPipeValid(char lastPipe, char nextPipe, Direction direction)
        {
            var validDirections = new List<Direction>();
            switch (lastPipe)
            {
                case 'S':
                    validDirections = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();
                    break;

                case 'F':
                    validDirections = new List<Direction>() { Direction.Right, Direction.Down };
                    break;

                case 'L':
                    validDirections = new List<Direction>() { Direction.Right, Direction.Up };
                    break;

                case 'J':
                    validDirections = new List<Direction>() { Direction.Left, Direction.Up };
                    break;

                case '7':
                    validDirections = new List<Direction>() { Direction.Left, Direction.Down };
                    break;

                case '-':
                    validDirections = new List<Direction>() { Direction.Right, Direction.Left };
                    break;

                case '|':
                    validDirections = new List<Direction>() { Direction.Up, Direction.Down };
                    break;

            }

            if (!validDirections.Contains(direction))
            {
                return false;
            }

            if (nextPipe == 'S')
            {
                return true;
            }

            switch (direction)
            {
                case Direction.Right:
                    return nextPipe == '-' || nextPipe == 'J' || nextPipe == '7';

                case Direction.Up:
                    return nextPipe == '|' || nextPipe == 'F' || nextPipe == '7';

                case Direction.Down:
                    return nextPipe == '|' || nextPipe == 'J' || nextPipe == 'L';

                case Direction.Left:
                    return nextPipe == '-' || nextPipe == 'F' || nextPipe == 'L';

            }

            return false;
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
                for (var i = 1; i < allSequences.Count; i++)
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
