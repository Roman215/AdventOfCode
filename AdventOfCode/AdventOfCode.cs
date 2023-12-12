using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;

namespace AdventOfCode
{
    internal class AdventOfCode
    {
        static BigInteger LCM(BigInteger[] numbers)
        {
            return numbers.Aggregate((S, val) => S * val / GCD(S, val));
        }

        static BigInteger GCD(BigInteger a, BigInteger b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        public static int Day1(int part = 2)
        {
            var sr = new StreamReader("Day1-Input.txt");
            var line = sr.ReadLine();
            var output = 0;

            while (line != null)
            {
                var lineString = line;
                if (part == 2)
                {
                    lineString = line.Replace("one", "o1e").Replace("two", "t2o").Replace("three", "t3e").Replace("four", "f4r").Replace("five", "f5e").Replace("six", "s6x").Replace("seven", "s7n").Replace("eight", "e8t").Replace("nine", "n9e");
                }
                var numbers = lineString.Where(c => char.IsDigit(c)).Select(c => c - '0');
                output += numbers.FirstOrDefault() * 10;
                output += numbers.LastOrDefault();

                line = sr.ReadLine();
            }

            return output;
        }

        public static int Day2()
        {
            var sr = new StreamReader("Day2-Input.txt");
            var line = sr.ReadLine();
            var output = 0;
            var maxColorMap = new Dictionary<string, int>()
            {
                { "red", 12 },
                { "green", 13 },
                { "blue", 14 },
            };

            while (line != null)
            {
                var lineSplitByGame = line.Split(": ");
                var id = Convert.ToInt32(lineSplitByGame[0].Replace("Game ", ""));
                var lineSplitByRounds = lineSplitByGame[1].Split("; ");
                bool invalidGame = false;
                var minColorMap = new Dictionary<string, int>();
                var power = 1;

                foreach (var round in lineSplitByRounds)
                {
                    if (invalidGame)
                    {
                        break;
                    }

                    var draws = round.Split(", ");
                    foreach (var draw in draws)
                    {
                        var drawSplitByNumAndColor = draw.Split(" ");
                        var num = Convert.ToInt32(drawSplitByNumAndColor[0]);
                        var color = drawSplitByNumAndColor[1];

                        // Part 1
                        /*if (num > maxColorMap[color])
                        {
                            invalidGame = true;
                            break;
                        }*/

                        // Part 2
                        if (!minColorMap.ContainsKey(color) || num > minColorMap[color])
                        {
                            minColorMap[color] = num;
                        }
                    }
                }

                // Part 1
                /*if (!invalidGame)
                {
                    output += id;
                }*/

                // Part 2
                foreach (var kvPair in minColorMap)
                {
                    power *= kvPair.Value;
                }
                //Console.WriteLine(power);
                output += power;

                line = sr.ReadLine();
            }

            return output;
        }

        public static int Day3()
        {
            var sr = new StreamReader("Day3-Input.txt");
            var line = sr.ReadLine();
            List<List<char>> inputMatrix = new();
            var part1Output = 0;
            var output = 0;
            Dictionary<string, List<int>> gearPivots = new();

            while (line != null)
            {
                inputMatrix.Add(line.ToCharArray().ToList());

                line = sr.ReadLine();
            }

            for (int i = 0; i < inputMatrix.Count; i++)
            {
                string numberString = "";
                for (int j = 0; j < inputMatrix[i].Count; j++)
                {
                    char currentChar = inputMatrix[i][j];
                    var iMin = 0;
                    var iMax = inputMatrix.Count - 1;
                    var jMin = 0;
                    var jMax = inputMatrix[0].Count - 1;

                    if (char.IsDigit(currentChar))
                    {
                        iMin = Math.Max(iMin, i - 1);
                        iMax = Math.Min(iMax, i + 1);
                        jMin = Math.Max(jMin, j - 1);

                        numberString += currentChar;
                        j++;

                        while (j < inputMatrix[i].Count && char.IsDigit(inputMatrix[i][j]))
                        {
                            currentChar = inputMatrix[i][j];
                            numberString += currentChar;
                            j++;
                        }

                        jMax = Math.Min(jMax, j);

                        for (int k = iMin; k <= iMax; k++)
                        {
                            for (int l = jMin; l <= jMax; l++)
                            {
                                var charToCheck = inputMatrix[k][l];
                                if (charToCheck != '.' && !char.IsLetterOrDigit(charToCheck))
                                {
                                    part1Output += Convert.ToInt32(numberString);
                                    //Console.WriteLine(numberString);

                                    if (charToCheck == '*')
                                    {
                                        var key = k + " " + l;
                                        if (!gearPivots.ContainsKey(key))
                                        {
                                            gearPivots[key] = new List<int>();
                                        }
                                        gearPivots[key].Add(Convert.ToInt32(numberString));
                                    }
                                }
                            }
                        }
                        numberString = "";
                    }
                }
            }

            foreach (var kvPair in gearPivots)
            {
                if (kvPair.Value.Count == 2)
                {
                    output += kvPair.Value[0] * kvPair.Value[1];
                }
            }

            return output;
        }

        public static int Day4()
        {
            var sr = new StreamReader("Day4-Input.txt");
            var line = sr.ReadLine();
            var output = 0;
            var cardCopies = new Dictionary<int, int>();
            var lastCardNumber = 0;

            while (line != null)
            {
                var cardNumber = Convert.ToInt32(line.Split("Card ")[1].Split(":")[0]);
                var winningNumbers = line.Split(": ")[1].Split(" | ")[0].Split(" ").Where(s => int.TryParse(s, out _));
                var playedNumbers = line.Split(": ")[1].Split(" | ")[1].Split(" ").Where(s => int.TryParse(s, out _));
                var cardValuePart1 = 0;
                var cardValuePart2 = 0;
                if (cardNumber > lastCardNumber)
                {
                    lastCardNumber = cardNumber;
                }

                if (!cardCopies.ContainsKey(cardNumber))
                {
                    cardCopies[cardNumber] = 0;
                }
                cardCopies[cardNumber]++;

                foreach (var playedNumber in playedNumbers)
                {
                    if (winningNumbers.Contains(playedNumber))
                    {
                        if (cardValuePart1 == 0)
                        {
                            cardValuePart1 = 1;
                        }
                        else
                        {
                            cardValuePart1 *= 2;
                        }

                        cardValuePart2++;
                    }
                }

                for (int i = 1; i <= cardValuePart2; i++)
                {
                    if (!cardCopies.ContainsKey(cardNumber + i))
                    {
                        cardCopies[cardNumber + i] = 0;
                    }
                    cardCopies[cardNumber + i] += cardCopies[cardNumber];
                }

                // Part 1
                //output += cardValuePart1;

                line = sr.ReadLine();
            }

            // Part 2
            foreach (var kvPair in cardCopies)
            {
                var cardNumber = kvPair.Key;
                if (cardNumber <= lastCardNumber)
                {
                    output += kvPair.Value;
                }
            }

            return output;
        }

        public static long Day5()
        {
            var sr = new StreamReader("Day5-Input.txt");
            var line = sr.ReadLine();
            List<long> seeds = new();
            List<List<long>> seedToSoilMap = new();
            List<List<long>> soilToFertilizerMap = new();
            List<List<long>> fertilizerToWaterMap = new();
            List<List<long>> waterToLightMap = new();
            List<List<long>> lightToTemperatureMap = new();
            List<List<long>> temperatureToHumidityMap = new();
            List<List<long>> humidityToLocationMap = new();
            List<List<long>> currentMap = null;
            long minSeedLocation = long.MaxValue;
            List<List<List<long>>> currentMapList = new()
            {
                seedToSoilMap,
                soilToFertilizerMap,
                fertilizerToWaterMap,
                waterToLightMap,
                lightToTemperatureMap,
                temperatureToHumidityMap,
                humidityToLocationMap,
            };
            List<long> seedPairs = new();


            while (line != null)
            {
                if (line.Contains("seeds: "))
                {
                    // Part 1
                    //seeds = line.Split("seeds: ")[1].Split(" ").Select(s => Convert.ToInt64(s)).ToList();

                    // Part 2
                    seedPairs = line.Split("seeds: ")[1].Split(" ").Select(s => Convert.ToInt64(s)).ToList();
                }
                else if (line.Contains("seed-to-soil map:"))
                {
                    currentMap = seedToSoilMap;
                }
                else if (line.Contains("soil-to-fertilizer map:"))
                {
                    currentMap = soilToFertilizerMap;
                }
                else if (line.Contains("fertilizer-to-water map:"))
                {
                    currentMap = fertilizerToWaterMap;
                }
                else if (line.Contains("water-to-light map:"))
                {
                    currentMap = waterToLightMap;
                }
                else if (line.Contains("light-to-temperature map:"))
                {
                    currentMap = lightToTemperatureMap;
                }
                else if (line.Contains("temperature-to-humidity map:"))
                {
                    currentMap = temperatureToHumidityMap;
                }
                else if (line.Contains("humidity-to-location map:"))
                {
                    currentMap = humidityToLocationMap;
                }
                else if (line.Length > 0 && line.Split(" ").Length == 3)
                {
                    var source = Convert.ToInt64(line.Split(" ")[1]);
                    var destination = Convert.ToInt64(line.Split(" ")[0]);
                    var length = Convert.ToInt64(line.Split(' ')[2]);
                    var currentLine = new List<long>
                    {
                        source,
                        destination,
                        length
                    };
                    if (currentMap != null)
                    {
                        currentMap.Add(currentLine);
                    }
                }

                line = sr.ReadLine();
            }

            for (int i = 0; i < seedPairs.Count; i += 2)
            {
                for (long j = seedPairs[i]; j < seedPairs[i] + seedPairs[i + 1]; j++)
                {
                    var currentValue = j;
                    foreach (var map in currentMapList)
                    {
                        foreach (var currentRange in map)
                        {
                            var source = currentRange[0];
                            var destination = currentRange[1];
                            var length = currentRange[2];

                            if (currentValue >= source && currentValue < source + length)
                            {
                                currentValue = destination + (currentValue - source);
                                break;
                            }
                        }
                    }

                    if (currentValue < minSeedLocation)
                    {
                        minSeedLocation = currentValue;
                    }
                }
            }

            return minSeedLocation;
        }

        public static long Day6(int part = 2)
        {
            var sr = new StreamReader("Day6-Input.txt");
            var line = sr.ReadLine();
            long output = 0;
            var times = new List<long>();
            var distances = new List<long>();

            while (line != null)
            {
                if (line.Contains("Time:"))
                {
                    times = Regex.Replace(line.Split("Time:")[1], @"\s+", " ").Trim().Split(" ").Select(s => Convert.ToInt64(s)).ToList();

                    // Part 2
                    if (part == 2)
                    {
                        string combined = "";
                        foreach (var time in times)
                        {
                            combined += time.ToString();
                        }
                        times = new List<long>() { Convert.ToInt64(combined) };
                    }
                }
                else if (line.Contains("Distance:"))
                {
                    distances = Regex.Replace(line.Split("Distance:")[1], @"\s+", " ").Trim().Split(" ").Select(s => Convert.ToInt64(s)).ToList();

                    // Part 2
                    if (part == 2)
                    {
                        string combined = "";
                        foreach (var distance in distances)
                        {
                            combined += distance.ToString();
                        }
                        distances = new List<long>() { Convert.ToInt64(combined) };
                    }
                }

                line = sr.ReadLine();
            }

            for (int i = 0; i < times.Count; i++)
            {
                var winningCombos = 0;
                for (long j = 1; j < times[i]; j++)
                {
                    var heldTime = j;
                    var travelTime = times[i] - heldTime;

                    if (heldTime * travelTime > distances[i])
                    {
                        winningCombos++;
                    }
                }

                if (winningCombos > 0)
                {
                    if (output == 0)
                    {
                        output = 1;
                    }
                    output *= winningCombos;
                }
            }

            return output;
        }

        public static long Day7(int part = 2)
        {
            var sr = new StreamReader("Day7-Input.txt");
            var line = sr.ReadLine();
            long output = 0;
            List<string> handsAndBets = new();
            Dictionary<char, int> cardValues = new()
            {
                { 'A', 14 },
                { 'K', 13 },
                { 'Q', 12 },
                { 'J', part == 1 ? 11 : 1 },
                { 'T', 10 },
                { '9', 9},
                { '8', 8 },
                { '7', 7 },
                { '6', 6 },
                { '5', 5 },
                { '4', 4 },
                { '3', 3 },
                { '2', 2 },
            };

            var sameRankHandComparison = (string handA, string handB) =>
            {
                for (int i = 0; i < handA.Length; i++)
                {
                    if (handA[i] != handB[i])
                    {
                        return cardValues[handA[i]] < cardValues[handB[i]] ? -1 : 1;
                    }
                }
                return 0;
            };

            while (line != null)
            {
                handsAndBets.Add(line);

                line = sr.ReadLine();
            }

            handsAndBets.Sort((handAndBetA, handAndBetB) =>
            {
                Dictionary<char, int> cardRepeatsA = new();
                Dictionary<char, int> cardRepeatsB = new();

                var handA = handAndBetA.Split(" ")[0];
                var handB = handAndBetB.Split(" ")[0];

                foreach (char c in handA)
                {
                    if (!cardRepeatsA.ContainsKey(c))
                    {
                        cardRepeatsA[c] = 0;
                    }
                    cardRepeatsA[c]++;
                }

                foreach (char c in handB)
                {
                    if (!cardRepeatsB.ContainsKey(c))
                    {
                        cardRepeatsB[c] = 0;
                    }
                    cardRepeatsB[c]++;
                }

                if (part == 2)
                {
                    if (cardRepeatsA.Count > 1 && cardRepeatsA.ContainsKey('J'))
                    {
                        var numJokers = cardRepeatsA['J'];
                        cardRepeatsA.Remove('J');
                        var bestCards = cardRepeatsA.ToList().OrderByDescending(kv => kv.Value).ToList();

                        if (bestCards.Count == 1 || bestCards[0].Value != bestCards[1].Value)
                        {
                            cardRepeatsA[bestCards[0].Key] += numJokers;
                        }
                        else if (handA.IndexOf(bestCards[0].Key) < handA.IndexOf(bestCards[1].Key))
                        {
                            cardRepeatsA[bestCards[0].Key] += numJokers;
                        }
                        else
                        {
                            cardRepeatsA[bestCards[1].Key] += numJokers;
                        }
                    }

                    if (cardRepeatsB.Count > 1 && cardRepeatsB.ContainsKey('J'))
                    {
                        var numJokers = cardRepeatsB['J'];
                        cardRepeatsB.Remove('J');
                        var bestCards = cardRepeatsB.ToList().OrderByDescending(kv => kv.Value).ToList();
                        if (bestCards.Count == 1 || bestCards[0].Value != bestCards[1].Value)
                        {
                            cardRepeatsB[bestCards[0].Key] += numJokers;
                        }
                        else if (handB.IndexOf(bestCards[0].Key) < handB.IndexOf(bestCards[1].Key))
                        {
                            cardRepeatsB[bestCards[0].Key] += numJokers;
                        }
                        else
                        {
                            cardRepeatsB[bestCards[1].Key] += numJokers;
                        }
                    }
                }

                if (cardRepeatsA.Count == cardRepeatsB.Count)
                {
                    var cardRepeatsAValuesSorted = cardRepeatsA.Values.OrderBy(v => v).ToList();
                    var cardRepeatsBValuesSorted = cardRepeatsB.Values.OrderBy(v => v).ToList();

                    for (int i = 0; i < cardRepeatsAValuesSorted.Count(); i++)
                    {
                        if (cardRepeatsAValuesSorted[i] != cardRepeatsBValuesSorted[i])
                        {
                            return cardRepeatsAValuesSorted[i] > cardRepeatsBValuesSorted[i] ? -1 : 1;
                        }
                    }

                    return sameRankHandComparison(handA, handB);
                }

                return cardRepeatsA.Count < cardRepeatsB.Count ? 1 : -1;
            });

            for (int i = 0; i < handsAndBets.Count; i++)
            {
                output += Convert.ToInt64(handsAndBets[i].Split(" ")[1]) * (i + 1);
            }

            return output;
        }

        public static BigInteger Day8()
        {
            var sr = new StreamReader("Day8-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = 0;
            Dictionary<string, Dictionary<char, string>> nodesToNodesMap = new();
            string directions = "";

            while (line != null)
            {
                if (line.Length > 0)
                {
                    if (line.Contains("="))
                    {
                        var currentNode = line.Split(" = ")[0];
                        var leftNode = line.Split(" = ")[1].Trim('(').Trim(')').Split(", ")[0];
                        var rightNode = line.Split(" = ")[1].Trim('(').Trim(')').Split(", ")[1];
                        nodesToNodesMap[currentNode] = new();
                        nodesToNodesMap[currentNode]['L'] = leftNode;
                        nodesToNodesMap[currentNode]['R'] = rightNode;
                    }
                    else
                    {
                        directions = line;
                    }
                }

                line = sr.ReadLine();
            }

            var currentNodes = nodesToNodesMap.Keys.Where(k => k.EndsWith("A")).ToList();
            var lastNodes = new HashSet<string>(nodesToNodesMap.Keys.Where(k => k.EndsWith("Z")));
            var currentNodesVisitedNodes = new Dictionary<string, List<string>>();

            for (int i = 0; i < currentNodes.Count; i++)
            {
                var firstNode = currentNodes[i];
                var currentNode = firstNode;
                currentNodesVisitedNodes[currentNode] = new();
                var currentNodeVisitedNodes = currentNodesVisitedNodes[currentNode];
                int index = 0;

                while (true)
                {
                    char currentStep = directions[index];
                    currentNode = nodesToNodesMap[currentNode][currentStep];
                    var foundNodePos = currentNodeVisitedNodes.IndexOf(index + currentNode);

                    if (foundNodePos >= 0)
                    {
                        currentNodesVisitedNodes[firstNode] = currentNodesVisitedNodes[firstNode].Skip(foundNodePos).ToList();
                        break;
                    }

                    currentNodeVisitedNodes.Add(index + currentNode);

                    index = (index + 1) % directions.Length;
                }
            }

            var lcm = LCM(currentNodesVisitedNodes.Values.Select(value => (BigInteger)value.Count()).ToArray());

            output = lcm;

            return output;
        }

        public static BigInteger Day9Extrapolate(List<BigInteger> values, int part = 2)
        {
            if (values.Count == 1)
            {
                return values[0];
            }

            List<BigInteger> newValues = new List<BigInteger>();
            for (int i = 0; i < values.Count - 1; i++)
            {
                newValues.Add(values[i + 1] - values[i]);
            }

            bool allAreZero = true;
            for (int i = 0; i < newValues.Count; i++)
            {
                if (newValues[i] != 0)
                {
                    allAreZero = false;
                    break;
                }
            }

            if (!allAreZero)
            {
                return part == 2 ? (values.First() - Day9Extrapolate(newValues)) : (values.Last() + Day9Extrapolate(newValues));
            }

            return part == 2 ? (values.First() - newValues.First()) : (values.Last() + newValues.Last());
        }

        public static BigInteger Day9(int part = 2)
        {
            var sr = new StreamReader("Day9-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = 0;
            List<List<BigInteger>> histories = new();

            while (line != null)
            {
                histories.Add(line.Split(" ").Select(s => new BigInteger(Convert.ToInt32(s))).ToList());

                line = sr.ReadLine();
            }

            for (int i = 0; i < histories.Count; i++)
            {
                output += Day9Extrapolate(histories[i], part);
            }

            return output;
        }

        public static BigInteger Day10(int part = 2)
        {
            var sr = new StreamReader("Day10-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = 0;
            List<List<char>> grid = new();
            List<List<char>> magnifiedGrid = new();
            char groundChar = '.';
            char newPipeChar = '@';

            // Part 2 would be way easier if we zoom in our graph to something which can clearly identify loops/gaps
            // So substitute each tile with a 3x3 representation of the same object. For example:
            // 7 has west and south connections and so does this 3x3 object:
            // . . .
            // @ @ .
            // . @ .
            var magnifyTile = (char tile) =>
            {
                List<List<char>> zoomedInTile = new();
                while (zoomedInTile.Count < 3)
                {
                    zoomedInTile.Add(new List<char>() { groundChar, groundChar, groundChar });
                }

                if (tile == '|')
                {
                    zoomedInTile[0][1] = zoomedInTile[1][1] = zoomedInTile[2][1] = newPipeChar;
                }
                else if (tile == '-')
                {
                    zoomedInTile[1][0] = zoomedInTile[1][1] = zoomedInTile[1][2] = newPipeChar;
                }
                else if (tile == 'L')
                {
                    zoomedInTile[1][1] = zoomedInTile[0][1] = zoomedInTile[1][2] = newPipeChar;
                }
                else if (tile == 'J')
                {
                    zoomedInTile[1][1] = zoomedInTile[0][1] = zoomedInTile[1][0] = newPipeChar;
                }
                else if (tile == '7')
                {
                    zoomedInTile[1][1] = zoomedInTile[2][1] = zoomedInTile[1][0] = newPipeChar;
                }
                else if (tile == 'F')
                {
                    zoomedInTile[1][1] = zoomedInTile[2][1] = zoomedInTile[1][2] = newPipeChar;
                }

                return zoomedInTile;
            };

            while (line != null)
            {
                grid.Add(line.ToCharArray().ToList());

                line = sr.ReadLine();
            }

            for (int i = 0; i < grid.Count * 3; i++)
            {
                magnifiedGrid.Add(new List<char>());
            }

            Tuple<int, int> start = Tuple.Create(0, 0);
            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0; j < grid[i].Count; j++)
                {
                    var tile = grid[i][j];

                    if (tile == 'S')
                    {
                        start = Tuple.Create(3 * i + 1, 3 * j + 1);
                        bool connectsNorth = (i > 0 && (grid[i - 1][j] == '|' || grid[i - 1][j] == '7' || grid[i - 1][j] == 'F')) ? true : false;
                        bool connectsSouth = (i < grid.Count - 1 && (grid[i + 1][j] == '|' || grid[i + 1][j] == 'L' || grid[i + 1][j] == 'J')) ? true : false;
                        bool connectsWest = (j > 0 && (grid[i][j - 1] == '-' || grid[i][j - 1] == 'L' || grid[i][j - 1] == 'F')) ? true : false;
                        bool connectsEast = (j < grid[i].Count && (grid[i][j + 1] == '-' || grid[i][j + 1] == 'J' || grid[i][j + 1] == '7')) ? true : false;
                        tile = (connectsNorth && connectsSouth) ? '|'
                            : (connectsNorth && connectsWest) ? 'J'
                            : (connectsNorth && connectsEast) ? 'L'
                            : (connectsSouth && connectsWest) ? '7'
                            : (connectsSouth && connectsEast) ? 'F'
                            : (connectsWest && connectsEast) ? '-'
                            : '.';
                    }
                    var magnifiedTile = magnifyTile(tile);

                    magnifiedGrid[3 * i] = magnifiedGrid[3 * i].Concat(magnifiedTile[0]).ToList();
                    magnifiedGrid[3 * i + 1] = magnifiedGrid[3 * i + 1].Concat(magnifiedTile[1]).ToList();
                    magnifiedGrid[3 * i + 2] = magnifiedGrid[3 * i + 2].Concat(magnifiedTile[2]).ToList();
                }
            }

            Queue<Tuple<int, int>> queue = new();
            HashSet<Tuple<int, int>> visited = new();

            // Part 1 logic
            if (part != 2)
            {
                queue.Enqueue(start);
                visited.Add(start);

                while (queue.Count > 0)
                {
                    output++;
                    var current = queue.Dequeue();
                    var directionToTry = Tuple.Create(current.Item1 - 1, current.Item2); // North
                    if (magnifiedGrid[directionToTry.Item1][directionToTry.Item2] == newPipeChar && !visited.Contains(directionToTry))
                    {
                        queue.Enqueue(directionToTry);
                        visited.Add(directionToTry);
                        continue;
                    }
                    directionToTry = Tuple.Create(current.Item1 + 1, current.Item2); // South
                    if (magnifiedGrid[directionToTry.Item1][directionToTry.Item2] == newPipeChar && !visited.Contains(directionToTry))
                    {
                        queue.Enqueue(directionToTry);
                        visited.Add(directionToTry);
                        continue;
                    }
                    directionToTry = Tuple.Create(current.Item1, current.Item2 - 1); // West
                    if (magnifiedGrid[directionToTry.Item1][directionToTry.Item2] == newPipeChar && !visited.Contains(directionToTry))
                    {
                        queue.Enqueue(directionToTry);
                        visited.Add(directionToTry);
                        continue;
                    }
                    directionToTry = Tuple.Create(current.Item1, current.Item2 + 1); // East
                    if (magnifiedGrid[directionToTry.Item1][directionToTry.Item2] == newPipeChar && !visited.Contains(directionToTry))
                    {
                        queue.Enqueue(directionToTry);
                        visited.Add(directionToTry);
                        continue;
                    }
                }

                return output / 3 / 2; // Divide 3 because of our zoomed graph that's 3x bigger and divide 2 because we went the whole loop around, while instead we only want the half point
            }

            // For part 2 we want to brute force travel in each direction. Put all the edges of the graph into our queue
            for (int i = 0; i < magnifiedGrid.Count; i++)
            {
                queue.Enqueue(Tuple.Create(0, i));
                queue.Enqueue(Tuple.Create(magnifiedGrid.Count - 1, i));
            }
            for (int i = 0; i < magnifiedGrid[0].Count; i++)
            {
                queue.Enqueue(Tuple.Create(i, 0));
                queue.Enqueue(Tuple.Create(i, magnifiedGrid[0].Count - 1));
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (visited.Contains(current))
                {
                    continue;
                }

                if (current.Item1 < 0 || current.Item1 >= magnifiedGrid.Count || current.Item2 < 0 || current.Item2 >= magnifiedGrid[current.Item1].Count)
                {
                    continue;
                }

                visited.Add(current);

                // If we hit a wall, there's no point in continuing this direction
                if (magnifiedGrid[current.Item1][current.Item2] == newPipeChar)
                {
                    continue;
                }

                // Append all four directions into our queue to try next
                queue.Enqueue(Tuple.Create(current.Item1 - 1, current.Item2));
                queue.Enqueue(Tuple.Create(current.Item1 + 1, current.Item2));
                queue.Enqueue(Tuple.Create(current.Item1, current.Item2 - 1));
                queue.Enqueue(Tuple.Create(current.Item1, current.Item2 + 1));
            }

            // Now that we explored the magnified graph fully where possible, it's time to count how many nodes were not visited. We need to basically demagnify our answer
            // If even one of the 3x3 grids representing a magnified square was visited, then the original spot was also visited and we don't want to count it.
            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0; j < grid[i].Count; j++)
                {
                    bool visitedThisOriginalTile = false;
                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            if (visited.Contains(Tuple.Create(3 * i + x, 3 * j + y)))
                            {
                                visitedThisOriginalTile = true;
                            }
                        }
                    }
                    if (!visitedThisOriginalTile)
                    {
                        output++;
                    }
                }
            }

            return output;
        }

        public static BigInteger Day11(int part = 2)
        {
            var sr = new StreamReader("Day11-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = 0;
            List<List<char>> grid = new();
            const char galaxyChar = '#';
            List<int> emptyRows = new();
            List<int> emptyColumns = new();
            List<Tuple<int, int>> galaxies = new();
            BigInteger expansion = part == 2 ? 1000000 : 2;

            while (line != null)
            {
                grid.Add(line.ToCharArray().ToList());

                line = sr.ReadLine();
            }

            for (int i = 0; i < grid.Count; i++)
            {
                bool isEmpty = true;
                for (int j = 0; j < grid[i].Count; j++)
                {
                    if (grid[i][j] == galaxyChar)
                    {
                        galaxies.Add(Tuple.Create(i, j));
                        isEmpty = false;
                    }
                }
                if (isEmpty)
                {
                    emptyRows.Add(i);
                }
            }

            for (int i = 0; i < grid[0].Count; i++)
            {
                bool isEmpty = true;
                for (int j = 0; j < grid.Count; j++)
                {
                    if (grid[j][i] == galaxyChar)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (isEmpty)
                {
                    emptyColumns.Add(i);
                }
            }

            // Find the distance between the galaxies and sum them up
            for (int i = 0; i < galaxies.Count; i++)
            {
                var galaxyA = galaxies[i];
                for (int j = i + 1; j < galaxies.Count; j++)
                {
                    var galaxyB = galaxies[j];
                    BigInteger distance = Math.Abs(galaxyB.Item1 - galaxyA.Item1) + Math.Abs(galaxyB.Item2 - galaxyA.Item2);
                    foreach (var emptyRow in emptyRows)
                    {
                        if (emptyRow >= Math.Min(galaxyA.Item1, galaxyB.Item1) && emptyRow <= Math.Max(galaxyA.Item1, galaxyB.Item1))
                        {
                            distance += expansion - 1;
                        }
                    }
                    foreach (var emptyColumn in emptyColumns)
                    {
                        if (emptyColumn >= Math.Min(galaxyA.Item2, galaxyB.Item2) && emptyColumn <= Math.Max(galaxyA.Item2, galaxyB.Item2))
                        {
                            distance += expansion - 1;
                        }
                    }
                    output += distance;
                }
            }

            return output;
        }

        public static BigInteger Day12(int part = 2)
        {
            var sr = new StreamReader("Day12-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = 0;
            const char damagedChar = '#';
            const char operationalChar = '.';
            const char unknownChar = '?';
            Dictionary<Tuple<string, string>, BigInteger> encountered = new();

            Func<string, List<int>, BigInteger> countRemainingArrangements = (string x, List<int> y) => { return 0; };
            countRemainingArrangements = (string remainingLine, List<int> remainingBlocks) =>
            {
                if (remainingLine.Length == 0)
                {
                    // Not a valid arrangement if we hit the end of the line and yet we still have more blocks to process
                    return (remainingBlocks.Count == 0) ? new BigInteger(1) : new BigInteger(0);
                }

                if (remainingBlocks.Count == 0)
                {
                    // If we finished processing all the damaged block sections, we expect no more damaged blocks in the remaining line
                    return !remainingLine.Contains(damagedChar) ? new BigInteger(1) : new BigInteger(0);
                }

                string remainingBlockStr = string.Join(',', remainingBlocks);
                if (encountered.ContainsKey(Tuple.Create(remainingLine, remainingBlockStr)))
                {
                    return encountered[Tuple.Create(remainingLine, remainingBlockStr)];
                }

                BigInteger result = 0;

                if (remainingLine[0] == operationalChar || remainingLine[0] == unknownChar)
                {
                    // Count the rest of the results where we substituted ? with a . (or this char was already a .)
                    result += countRemainingArrangements(remainingLine[1..], remainingBlocks);
                }
                if (remainingLine[0] == damagedChar || remainingLine[0] == unknownChar)
                {
                    // If we're currently processing a potential damaged character and we see a sequence of damaged characters matching (or can match the expected count of damaged springs)
                    if (remainingBlocks[0] <= remainingLine.Length && !remainingLine.Substring(0, remainingBlocks[0]).Contains(operationalChar)
                        && (remainingBlocks[0] == remainingLine.Length || remainingLine[remainingBlocks[0]] != damagedChar))
                    {
                        // Move ahead past the block of damaged characters and move to the next remainingBlock
                        // For example if remaining line is ####..## and remaining blocks are 4, 2 then move on to .## and remaining blocks 2
                        result += countRemainingArrangements(remainingLine.Length > remainingBlocks[0] + 1 ? remainingLine.Substring(remainingBlocks[0] + 1) : "", remainingBlocks.Skip(1).ToList());
                    }
                }

                encountered[Tuple.Create(remainingLine, remainingBlockStr)] = result;
                return result;
            };

            while (line != null)
            {
                var grid = line.Split(" ")[0];
                var numbers = line.Split(" ")[1].Split(",").Select(s => Convert.ToInt32(s)).ToList();

                if (part == 2)
                {
                    grid = grid + '?' + grid + '?' + grid + '?' + grid + '?' + grid;
                    numbers = numbers.Concat(numbers).Concat(numbers).Concat(numbers).Concat(numbers).ToList();
                }

                output += countRemainingArrangements(grid, numbers);

                line = sr.ReadLine();
            }

            return output;
        }
    }
}
