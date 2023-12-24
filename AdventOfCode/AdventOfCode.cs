using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Numerics;
using System.Text.RegularExpressions;

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

        public enum GridDirection
        {
            None,
            South,
            East,
            North,
            West,
        }

        public static BigInteger Day1(int part = 2)
        {
            var sr = new StreamReader("Day1-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = 0;

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

        public static BigInteger Day13(int part = 2)
        {
            var sr = new StreamReader("Day13-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = 0;
            List<List<List<char>>> grids = new();
            int currentGrid = 1;

            while (line != null)
            {
                if (line.Length == 0)
                {
                    currentGrid++;
                    line = sr.ReadLine();
                    continue;
                }

                if (grids.Count < currentGrid)
                {
                    grids.Add(new());
                }

                grids[currentGrid - 1].Add(line.ToCharArray().ToList());

                line = sr.ReadLine();
            }
            for (int i = 0; i < grids.Count; i++)
            {
                var grid = grids[i];

                // Compare all the rows
                for (int row1 = 0; row1 < grid.Count - 1; row1++)
                {
                    BigInteger mismatchedCells = 0;
                    for (int row2 = 0; row2 < grid.Count; row2++)
                    {
                        var topRow = row1 - row2;
                        var bottomRow = row1 + row2 + 1;
                        if (topRow >= 0 && bottomRow >= 0 && topRow < bottomRow && topRow < grid.Count && bottomRow < grid.Count)
                        {
                            // Compare the two rows and find if all the cells match
                            for (var col = 0; col < grid[row1].Count; col++)
                            {
                                if (grid[topRow][col] != grid[bottomRow][col])
                                {
                                    mismatchedCells++;
                                }
                            }
                        }
                    }
                    if (mismatchedCells == (part == 2 ? 1 : 0))
                    {
                        output += 100 * (row1 + 1);
                    }
                }

                // Compare all the columns
                for (int col1 = 0; col1 < grid[0].Count - 1; col1++)
                {
                    BigInteger mismatchedCells = 0;
                    for (int col2 = 0; col2 < grid.Count; col2++)
                    {
                        var leftCol = col1 - col2;
                        var rightCol = col1 + col2 + 1;
                        if (leftCol >= 0 && rightCol >= 0 && leftCol < rightCol && leftCol < grid[0].Count && rightCol < grid[0].Count)
                        {
                            // Compare the two columns and find if all the cells match
                            for (var row = 0; row < grid.Count; row++)
                            {
                                if (grid[row][leftCol] != grid[row][rightCol])
                                {
                                    mismatchedCells++;
                                }
                            }
                        }
                    }
                    if (mismatchedCells == (part == 2 ? 1 : 0))
                    {
                        output += col1 + 1;
                    }
                }
            }

            return output;
        }

        public static int Day14(int part = 2)
        {
            var sr = new StreamReader("Day14-Input.txt");
            var line = sr.ReadLine();
            var output = 0;
            const char rollingRock = 'O';
            const char stationaryRock = '#';
            const char emptyTile = '.';
            List<List<char>> grid = new();
            BigInteger numCycles = 1000000000;

            var tiltNorth = () =>
            {
                var northLoad = 0;
                for (int i = 0; i < grid.Count; i++)
                {
                    for (int j = 0; j < grid[i].Count; j++)
                    {
                        if (grid[i][j] == rollingRock)
                        {
                            var newNorth = i - 1;
                            while (newNorth >= 0 && grid[newNorth][j] == emptyTile)
                            {
                                grid[newNorth][j] = rollingRock;
                                grid[newNorth + 1][j] = emptyTile;
                                newNorth--;
                            }
                            northLoad += grid.Count - newNorth - 1;
                        }
                    }
                }
                return northLoad;
            };

            var tiltWest = () =>
            {
                for (int j = 0; j < grid[0].Count; j++)
                {
                    for (int i = 0; i < grid.Count; i++)
                    {
                        if (grid[i][j] == rollingRock)
                        {
                            var newWest = j - 1;
                            while (newWest >= 0 && grid[i][newWest] == emptyTile)
                            {
                                grid[i][newWest] = rollingRock;
                                grid[i][newWest + 1] = emptyTile;
                                newWest--;
                            }
                        }
                    }
                }
            };

            var tiltSouth = () =>
            {
                for (int i = grid.Count - 1; i >= 0; i--)
                {
                    for (int j = 0; j < grid[i].Count; j++)
                    {
                        if (grid[i][j] == rollingRock)
                        {
                            var newSouth = i + 1;
                            while (newSouth <= grid[i].Count - 1 && grid[newSouth][j] == emptyTile)
                            {
                                grid[newSouth][j] = rollingRock;
                                grid[newSouth - 1][j] = emptyTile;
                                newSouth++;
                            }
                        }
                    }
                }
            };

            var tiltEast = () =>
            {
                var northLoad = 0;
                for (int j = grid[0].Count - 1; j >= 0; j--)
                {
                    for (int i = 0; i < grid.Count; i++)
                    {
                        if (grid[i][j] == rollingRock)
                        {
                            var newEast = j + 1;
                            while (newEast <= grid[i].Count - 1 && grid[i][newEast] == emptyTile)
                            {
                                grid[i][newEast] = rollingRock;
                                grid[i][newEast - 1] = emptyTile;
                                newEast++;
                            }
                            northLoad += grid.Count - i;
                        }
                    }
                }
                return northLoad;
            };

            var performCycle = () =>
            {
                tiltNorth();
                tiltWest();
                tiltSouth();
                return tiltEast();
            };

            while (line != null)
            {
                grid.Add(line.ToCharArray().ToList());

                line = sr.ReadLine();
            }

            // For part 1 we just want to tilt north one time and return the output
            if (part != 2)
            {
                return tiltNorth();
            }

            BigInteger currentCycles = 0;
            // Dictionary of all grids we encountered already. Value is how many cycles we encountered it after. Used to detect when the grids are caught in a loop of cycles
            Dictionary<string, BigInteger> encounteredGrids = new();
            while (currentCycles < numCycles)
            {
                currentCycles++;
                output = performCycle();

                string gridStr = "";
                for (int i = 0; i < grid.Count; i++)
                {
                    gridStr += string.Join("", grid[i]);
                }

                // If we previously encountered this grid before, then we're caught in a loop which is the size of the difference between this current cycle and the cycle we last saw that grid on
                // So we can skip ahead massively by moving past all of the loops then finishing up what remains near the end
                if (encounteredGrids.ContainsKey(gridStr))
                {
                    var loopLength = currentCycles - encounteredGrids[gridStr];
                    var numLoopsToPerform = (numCycles - currentCycles) / loopLength;
                    currentCycles += numLoopsToPerform * loopLength;
                }

                encounteredGrids[gridStr] = currentCycles;
            }

            return output;
        }

        public static BigInteger Day15(int part = 2)
        {
            var sr = new StreamReader("Day15-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = 0;
            List<string> codes = new();
            OrderedDictionary[] boxes = new OrderedDictionary[256];
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i] = new();
            }

            while (line != null)
            {
                codes = line.Split(",").ToList();

                line = sr.ReadLine();
            }

            for (int j = 0; j < codes.Count; j++)
            {
                var code = codes[j];
                BigInteger currentValue = 0;
                for (int i = 0; i < code.Length; i++)
                {
                    if (part != 2 || (code[i] != '=' && code[i] != '-'))
                    {
                        currentValue += code[i];
                        currentValue *= 17;
                        currentValue %= 256;
                    }
                    else if (code[i] == '=')
                    {
                        var label = code.Split("=")[0];
                        boxes[(int)currentValue][label] = code;
                        break;
                    }
                    else if (code[i] == '-')
                    {
                        var label = code.Split("-")[0];
                        boxes[(int)currentValue].Remove(label);
                        break;
                    }
                }

                if (part != 2)
                {
                    output += currentValue;
                }
            }

            if (part == 2)
            {
                for (int i = 0; i < boxes.Length; i++)
                {
                    for (int j = 0; j < boxes[i].Count; j++)
                    {
                        BigInteger focalLength = Convert.ToInt32(boxes[i][j].ToString().Split("=")[1]);
                        output += (i + 1) * (j + 1) * focalLength;
                    }
                }
            }

            return output;
        }

        public class LightBeam
        {
            public GridDirection Direction { get; set; } = GridDirection.East;
            public Tuple<int, int> Position { get; set; } = Tuple.Create(0, 0);

            public LightBeam(Tuple<int, int> position, GridDirection direction)
            {
                Position = position;
                Direction = direction;
            }
        }

        public static BigInteger Day16(int part = 2)
        {
            var sr = new StreamReader("Day16-Input.txt");
            var line = sr.ReadLine();
            List<List<char>> grid = new();
            const char energized = '#';
            const char verticalSplitter = '|';
            const char horizontalSplitter = '-';

            while (line != null)
            {
                grid.Add(line.ToCharArray().ToList());

                line = sr.ReadLine();
            }

            var calculateGridEnergy = (Tuple<int, int> startingPosition, GridDirection startingDirection) =>
            {
                List<List<char>> energizedGrid = new();
                BigInteger energizedCells = 0;
                for (int i = 0; i < grid.Count; i++)
                {
                    energizedGrid.Add(grid[i].ToArray().ToList());
                }

                List<LightBeam> lightBeams = new()
                {
                    new LightBeam(startingPosition, startingDirection),
                };

                HashSet<Tuple<int, int, GridDirection>> previousLightBeams = new();

                var travelAllLightBeams = () =>
                {
                    for (var lightIndex = 0; lightIndex < lightBeams.Count; lightIndex++)
                    {
                        var lightBeam = lightBeams[lightIndex];
                        var i = lightBeam.Position.Item1;
                        var j = lightBeam.Position.Item2;
                        if (i < 0 || j < 0 || i >= grid.Count || j >= grid[i].Count || previousLightBeams.Contains(Tuple.Create(lightBeam.Position.Item1, lightBeam.Position.Item2, lightBeam.Direction)))
                        {
                            lightBeams.Remove(lightBeam);
                            lightIndex--;
                            continue;
                        }

                        previousLightBeams.Add(Tuple.Create(lightBeam.Position.Item1, lightBeam.Position.Item2, lightBeam.Direction));
                        energizedGrid[i][j] = energized;
                        if (lightBeam.Direction == GridDirection.North)
                        {
                            if (grid[i][j] == horizontalSplitter)
                            {
                                lightBeam.Direction = GridDirection.West;
                                lightBeam.Position = Tuple.Create(i, j - 1);
                                lightBeams.Add(new LightBeam(Tuple.Create(i, j + 1), GridDirection.East));
                            }
                            else if (grid[i][j] == '\\' || grid[i][j] == '/')
                            {
                                lightBeam.Direction = grid[i][j] == '\\' ? GridDirection.West : GridDirection.East;
                                lightBeam.Position = Tuple.Create(i, grid[i][j] == '\\' ? j - 1 : j + 1);
                            }
                            else
                            {
                                lightBeam.Position = Tuple.Create(i - 1, j);
                            }
                        }
                        else if (lightBeam.Direction == GridDirection.South)
                        {
                            if (grid[i][j] == horizontalSplitter)
                            {
                                lightBeam.Direction = GridDirection.West;
                                lightBeam.Position = Tuple.Create(i, j - 1);
                                lightBeams.Add(new LightBeam(Tuple.Create(i, j + 1), GridDirection.East));
                            }
                            else if (grid[i][j] == '\\' || grid[i][j] == '/')
                            {
                                lightBeam.Direction = grid[i][j] == '\\' ? GridDirection.East : GridDirection.West;
                                lightBeam.Position = Tuple.Create(i, grid[i][j] == '\\' ? j + 1 : j - 1);
                            }
                            else
                            {
                                lightBeam.Position = Tuple.Create(i + 1, j);
                            }
                        }
                        else if (lightBeam.Direction == GridDirection.West)
                        {
                            if (grid[i][j] == verticalSplitter)
                            {
                                lightBeam.Direction = GridDirection.North;
                                lightBeam.Position = Tuple.Create(i - 1, j);
                                lightBeams.Add(new LightBeam(Tuple.Create(i + 1, j), GridDirection.South));
                            }
                            else if (grid[i][j] == '\\' || grid[i][j] == '/')
                            {
                                lightBeam.Direction = grid[i][j] == '\\' ? GridDirection.North : GridDirection.South;
                                lightBeam.Position = Tuple.Create(grid[i][j] == '\\' ? i - 1 : i + 1, j);
                            }
                            else
                            {
                                lightBeam.Position = Tuple.Create(i, j - 1);
                            }
                        }
                        else if (lightBeam.Direction == GridDirection.East)
                        {
                            if (grid[i][j] == verticalSplitter)
                            {
                                lightBeam.Direction = GridDirection.North;
                                lightBeam.Position = Tuple.Create(i - 1, j);
                                lightBeams.Add(new LightBeam(Tuple.Create(i + 1, j), GridDirection.South));
                            }
                            else if (grid[i][j] == '\\' || grid[i][j] == '/')
                            {
                                lightBeam.Direction = grid[i][j] == '\\' ? GridDirection.South : GridDirection.North;
                                lightBeam.Position = Tuple.Create(grid[i][j] == '\\' ? i + 1 : i - 1, j);
                            }
                            else
                            {
                                lightBeam.Position = Tuple.Create(i, j + 1);
                            }
                        }
                    }
                };

                while (lightBeams.Count > 0)
                {
                    travelAllLightBeams();
                }

                for (int i = 0; i < energizedGrid.Count; i++)
                {
                    for (int j = 0; j < energizedGrid[i].Count; j++)
                    {
                        if (energizedGrid[i][j] == energized)
                        {
                            energizedCells++;
                        }
                    }
                }

                return energizedCells;
            };

            if (part != 2)
            {
                return calculateGridEnergy(Tuple.Create(0, 0), GridDirection.East);
            }

            BigInteger maximumGridEnergy = 0;
            for (int i = 0; i < grid.Count; i++)
            {
                var gridEnergyStartingWest = calculateGridEnergy(Tuple.Create(i, 0), GridDirection.East);
                var gridEnergyStartingEast = calculateGridEnergy(Tuple.Create(i, grid[i].Count - 1), GridDirection.West);

                maximumGridEnergy = maximumGridEnergy >= gridEnergyStartingWest ? maximumGridEnergy : gridEnergyStartingWest;
                maximumGridEnergy = maximumGridEnergy >= gridEnergyStartingEast ? maximumGridEnergy : gridEnergyStartingEast;
            }

            for (int j = 0; j < grid[0].Count; j++)
            {
                var gridEnergyStartingNorth = calculateGridEnergy(Tuple.Create(0, j), GridDirection.South);
                var gridEnergyStartingSouth = calculateGridEnergy(Tuple.Create(grid.Count - 1, j), GridDirection.North);

                maximumGridEnergy = maximumGridEnergy >= gridEnergyStartingNorth ? maximumGridEnergy : gridEnergyStartingNorth;
                maximumGridEnergy = maximumGridEnergy >= gridEnergyStartingSouth ? maximumGridEnergy : gridEnergyStartingSouth;
            }

            return maximumGridEnergy;
        }

        public static BigInteger Day17(int part = 2)
        {
            var sr = new StreamReader("Day17-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = new BigInteger(1e20);
            var grid = new List<List<char>>();

            // Tuple is ordered by current row, current column, direction, how much the direction repeated, and the current total heat loss
            var crucibles = new PriorityQueue<Tuple<int, int, GridDirection, int, BigInteger>, BigInteger>();

            // Tuple is same as above except mapping first four to total heat loss
            var encounteredCrucibles = new Dictionary<Tuple<int, int, GridDirection, int>, BigInteger>();

            while (line != null)
            {
                grid.Add(line.ToCharArray().ToList());

                line = sr.ReadLine();
            }

            // Add the starting point to process
            crucibles.Enqueue(Tuple.Create(0, 0, GridDirection.None, -1, BigInteger.Zero), 0);

            while (crucibles.Count > 0)
            {
                var crucible = crucibles.Dequeue();
                var i = crucible.Item1;
                var j = crucible.Item2;
                var direction = crucible.Item3;
                var directionRepeats = crucible.Item4;
                var currentHeatLoss = crucible.Item5;
                var crucibleKey = Tuple.Create(i, j, direction, directionRepeats);
                if (encounteredCrucibles.ContainsKey(crucibleKey))
                {
                    continue;
                }
                encounteredCrucibles[crucibleKey] = crucible.Item5;

                // Travel in each direction as long as the movements are valid
                foreach (GridDirection newDirection in Enum.GetValues(typeof(GridDirection)))
                {
                    if (newDirection == GridDirection.None) continue;
                    var newRow = i + (newDirection == GridDirection.North ? -1 : newDirection == GridDirection.South ? 1 : 0);
                    var newColumn = j + (newDirection == GridDirection.West ? -1 : newDirection == GridDirection.East ? 1 : 0);
                    var newDirectionRepeats = (newDirection == direction) ? directionRepeats + 1 : 1;
                    var invalidDirectionReverse = (direction == GridDirection.North && newDirection == GridDirection.South)
                        || (direction == GridDirection.South && newDirection == GridDirection.North)
                        || (direction == GridDirection.West && newDirection == GridDirection.East)
                        || (direction == GridDirection.East && newDirection == GridDirection.West);
                    var invalidDirectionTooManyRepeats = newDirectionRepeats > (part == 2 ? 10 : 3);
                    var invalidDirectionMustStaySameDirection = part == 2 && (direction != newDirection && directionRepeats < 4 && directionRepeats != -1);
                    if (newRow < 0 || newRow >= grid.Count || newColumn < 0 || newColumn >= grid[newRow].Count || invalidDirectionReverse || invalidDirectionTooManyRepeats || invalidDirectionMustStaySameDirection)
                    {
                        continue;
                    }
                    var newHeatLoss = currentHeatLoss + grid[newRow][newColumn] - '0';
                    crucibles.Enqueue(Tuple.Create(newRow, newColumn, newDirection, newDirectionRepeats, newHeatLoss), newHeatLoss);
                    if (newRow == grid.Count - 1 && newColumn == grid[0].Count - 1 && newHeatLoss < output)
                    {
                        output = newHeatLoss;
                    }
                }
            }

            return output;
        }

        public static BigInteger Day18(int part = 2)
        {
            var sr = new StreamReader("Day18-Input.txt");
            var line = sr.ReadLine();
            BigInteger row = 0;
            BigInteger column = 0;
            BigInteger perimeter = 0;
            List<Tuple<BigInteger, BigInteger>> points = new()
            {
                Tuple.Create(BigInteger.Zero, BigInteger.Zero),
            };

            while (line != null)
            {
                char direction = line.Split(" ")[0][0];
                int numTiles = Convert.ToInt32(line.Split(" ")[1]);

                if (part == 2)
                {
                    numTiles = Convert.ToInt32("0x" + line.Split(" ")[2].Substring(2, 5), 16);
                    var hexDir = Convert.ToInt32("0x" + line.Split(" ")[2].Substring(7, 1), 16);
                    direction = hexDir == 0 ? 'R' : hexDir == 1 ? 'D' : hexDir == 2 ? 'L' : hexDir == 3 ? 'U' : '\0';
                }

                perimeter += numTiles;
                row = row + numTiles * (direction == 'U' ? -1 : direction == 'D' ? 1 : 0);
                column = column + numTiles * (direction == 'L' ? -1 : direction == 'R' ? 1 : 0);
                points.Add(Tuple.Create(row, column));

                line = sr.ReadLine();
            }

            // This solution uses the shoelace theorem to calculate the area of the trench
            BigInteger area = 0;
            for (int i = 0; i < points.Count; i++)
            {
                var behind = i - 1 >= 0 ? i - 1 : i - 1 + points.Count();
                area += points[i].Item1 * (points[behind].Item2 - points[(i + 1) % points.Count].Item2);
            }

            area = BigInteger.Abs(area) / 2;

            return area - (perimeter / 2) + 1 + perimeter;
        }

        public static BigInteger Day19(int part = 2)
        {
            var sr = new StreamReader("Day19-Input.txt");
            var line = sr.ReadLine();
            BigInteger output = 0;
            // Tuple is category (xmas), operator (> or <), rating number, and new rule name (or A/R)
            Dictionary<string, List<Tuple<char, char, BigInteger, string>>> workflow = new();
            bool parsingWorkflows = true;

            // Recursive function to handle part 1's logic
            Func<string, Dictionary<char, BigInteger>, bool> processItem = (string ruleName, Dictionary<char, BigInteger> item) => { return false; };
            processItem = (string ruleName, Dictionary<char, BigInteger> item) =>
            {
                if (ruleName == "R")
                {
                    // Reject
                    return false;
                }
                if (ruleName == "A")
                {
                    // Accept
                    return true;
                }

                var rules = workflow[ruleName];

                for (var i = 0; i < rules.Count; i++)
                {
                    var rule = rules[i];
                    if (i == rules.Count - 1)
                    {
                        return processItem(rule.Item4, item);
                    }

                    var category = rule.Item1;
                    var oper = rule.Item2;
                    var ratingNum = rule.Item3;
                    var newRuleName = rule.Item4;

                    if ((oper == '>' && item[category] > ratingNum) || (oper == '<' && item[category] < ratingNum))
                    {
                        return processItem(newRuleName, item);
                    }
                }

                return false;
            };

            // Recursive function to handle part 2's logic
            Func<string, Dictionary<char, Tuple<BigInteger, BigInteger>>, BigInteger> processItemRanges = (string ruleName, Dictionary<char, Tuple<BigInteger, BigInteger>> itemRange) => { return 0; };
            processItemRanges = (string ruleName, Dictionary<char, Tuple<BigInteger, BigInteger>> itemRange) =>
            {
                BigInteger result = 0;

                if (ruleName == "R")
                {
                    return 0;
                }

                if (ruleName == "A")
                {
                    // Return all the permutations of all the xmas variations within the item's range
                    result = 1;
                    foreach (var range in itemRange.Values)
                    {
                        result *= range.Item2 - range.Item1 + 1;
                    }
                    return result;
                }

                var rules = workflow[ruleName];

                for (var i = 0; i < rules.Count; i++)
                {
                    var rule = rules[i];
                    if (i == rules.Count - 1)
                    {
                        result += processItemRanges(rule.Item4, itemRange);
                        break;
                    }

                    var category = rule.Item1;
                    var oper = rule.Item2;
                    var ratingNum = rule.Item3;
                    var newRuleName = rule.Item4;
                    var rangeMin = itemRange[category].Item1;
                    var rangeMax = itemRange[category].Item2;
                    Tuple<BigInteger, BigInteger> rangeForNewRule = Tuple.Create(BigInteger.Zero, BigInteger.Zero);
                    Tuple<BigInteger, BigInteger> rangeForThisRule = Tuple.Create(BigInteger.Zero, BigInteger.Zero);

                    // If part of the range matches the new rule, split the range into the portion that matches the new rule and the portion that still matches this current rule
                    if (oper == '<')
                    {
                        rangeForNewRule = Tuple.Create(rangeMin, ratingNum - 1);
                        rangeForThisRule = Tuple.Create(ratingNum, rangeMax);
                    }
                    else if (oper == '>')
                    {
                        rangeForNewRule = Tuple.Create(ratingNum + 1, rangeMax);
                        rangeForThisRule = Tuple.Create(rangeMin, ratingNum);
                    }
                    if (rangeForNewRule.Item1 <= rangeForNewRule.Item2)
                    {
                        var copyOfItemRange = itemRange.ToDictionary(entry => entry.Key, entry => entry.Value);
                        copyOfItemRange[category] = rangeForNewRule;
                        result += processItemRanges(newRuleName, copyOfItemRange);
                    }
                    if (rangeForThisRule.Item1 <= rangeForThisRule.Item2)
                    {
                        itemRange[category] = rangeForThisRule;
                    }
                    else
                    {
                        break;
                    }
                }

                return result;
            };

            while (line != null)
            {
                if (line.Length == 0)
                {
                    parsingWorkflows = false;
                    line = sr.ReadLine();
                    continue;
                }

                if (parsingWorkflows)
                {
                    var ruleName = line.Split("{")[0];
                    var rules = line.Split("{")[1].Trim('}').Split(",").ToList();
                    if (!workflow.ContainsKey(ruleName))
                    {
                        workflow[ruleName] = new();
                    }

                    for (int i = 0; i < rules.Count; i++)
                    {
                        var rule = rules[i];
                        if (i == rules.Count - 1)
                        {
                            workflow[ruleName].Add(Tuple.Create('\0', '\0', BigInteger.Zero, rule));
                            continue;
                        }
                        var category = rule[0];
                        var oper = rule[1];
                        var num = new BigInteger(Convert.ToInt32(rule.Split(":")[0].Substring(2)));
                        var newWorkflow = rule.Split(":")[1];
                        workflow[ruleName].Add(Tuple.Create(category, oper, num, newWorkflow));
                    }
                }
                else if (part != 2)
                {
                    var itemStrings = line.Trim('{').Trim('}').Split(",");
                    Dictionary<char, BigInteger> item = new();
                    for (int i = 0; i < itemStrings.Length; i++)
                    {
                        var itemString = itemStrings[i];
                        var category = itemString[0];
                        BigInteger ratingNum = Convert.ToInt32(itemString.Split("=")[1]);
                        item[category] = ratingNum;
                    }

                    if (processItem("in", item))
                    {
                        foreach (var ratingNum in item.Values)
                        {
                            output += ratingNum;
                        }
                    }
                }
                else
                {
                    Dictionary<char, Tuple<BigInteger, BigInteger>> itemRanges = new();
                    var min = new BigInteger(1);
                    var max = new BigInteger(4000);
                    itemRanges['x'] = Tuple.Create(min, max);
                    itemRanges['m'] = Tuple.Create(min, max);
                    itemRanges['a'] = Tuple.Create(min, max);
                    itemRanges['s'] = Tuple.Create(min, max);
                    output += processItemRanges("in", itemRanges);

                    break;
                }

                line = sr.ReadLine();
            }

            return output;
        }

        public static BigInteger Day20(int part = 2)
        {
            var sr = new StreamReader("Day20-Input.txt");
            var line = sr.ReadLine();
            const char flipFlop = '%';
            const char conjunction = '&';
            const string lowPulse = "low";
            const string highPulse = "high";
            BigInteger lowPulses = 0;
            BigInteger highPulses = 0;
            BigInteger buttonPresses = 0;
            BigInteger part2Output = 0;

            // Key is module name, value is Tuple ordered by module name, module type (% or &), module memory (dictionary is needed for conjunctions, key is source module name, value is either on/off or most recent pulse type), and a list of all modules it outputs to
            var modules = new Dictionary<string, Tuple<string, char, Dictionary<string, string>, List<string>>>();

            // Tuple is source module, destination module, and pulse type (low, high)
            var pulses = new Queue<Tuple<string, string, string>>();

            var broadcasterTargets = new List<string>();

            while (line != null)
            {
                var lineSourceToDestinations = line.Split(" -> ");
                var sourceModuleAndType = lineSourceToDestinations[0];
                var destinationModules = lineSourceToDestinations[1].Split(", ");

                if (lineSourceToDestinations[0].Contains("broadcaster"))
                {
                    broadcasterTargets = destinationModules.ToList();

                    line = sr.ReadLine();
                    continue;
                }
                var sourceModuleType = sourceModuleAndType[0];
                var sourceModuleName = sourceModuleAndType.Substring(1);
                var sourceModuleMemory = new Dictionary<string, string>();

                if (sourceModuleType == flipFlop)
                {
                    // Flip flops are initially off
                    sourceModuleMemory[sourceModuleName] = "off";
                }

                modules[sourceModuleName] = Tuple.Create(sourceModuleName, sourceModuleType, sourceModuleMemory, destinationModules.ToList());

                line = sr.ReadLine();
            }

            // Initialize all the conjuction modules who have an input to a default of low pulse
            foreach (var module in modules)
            {
                var moduleName = module.Key;
                var destinationModules = module.Value.Item4;
                for (int i = 0; i < destinationModules.Count; i++)
                {
                    var destinationModule = destinationModules[i];
                    if (modules.ContainsKey(destinationModule) && modules[destinationModule].Item2 == conjunction)
                    {
                        var destModule = modules[destinationModule];
                        var destinationModuleMemory = destModule.Item3;
                        destinationModuleMemory[moduleName] = lowPulse;
                        modules[destinationModule] = Tuple.Create(destModule.Item1, destModule.Item2, destinationModuleMemory, destModule.Item4);
                    }
                }
            }

            // rx is fed into by a conjunction which means that in order for it to receive a low pulse, it needs ALL of the connections to be sending a high pulse
            // Which means that you have a bunch of cycles where each of those connections receive a high pulse and you need all of them to align
            // Which is the same type of problem as Day8 part 2 where you find the length of all those cycles and find the LCM.
            // Source to machine power is the module that calls into rx. For example if we had vd -> rx as a rule then vd is the source
            var sourceToMachinePower = modules.Values.Where(x => x.Item4.Contains("rx")).First().Item1;
            var cycles = new Dictionary<string, BigInteger>();
            var numOfConcurrentCycles = modules.Values.Where(x => x.Item4.Contains(sourceToMachinePower)).Count();

            while (true)
            {
                buttonPresses++;
                if ((part != 2 && buttonPresses > 1000) || part2Output > 0)
                {
                    break;
                }

                lowPulses++;
                foreach (var broadcasterTarget in broadcasterTargets)
                {
                    pulses.Enqueue(Tuple.Create("broadcaster", broadcasterTarget, lowPulse));
                }

                while (pulses.Count > 0)
                {
                    var pulse = pulses.Dequeue();
                    var sourceModuleName = pulse.Item1;
                    var destinationModuleName = pulse.Item2;
                    var pulseType = pulse.Item3;

                    lowPulses += pulseType == lowPulse ? 1 : 0;
                    highPulses += pulseType == highPulse ? 1 : 0;

                    if (!modules.ContainsKey(destinationModuleName))
                    {
                        continue;
                    }

                    var destinationModule = modules[destinationModuleName];
                    var destinationModuleType = destinationModule.Item2;
                    var destinationModuleMemory = destinationModule.Item3;
                    var destinationModuleDestinations = destinationModule.Item4;
                    string newMemoryValue;
                    string newPulseType = "";
                    bool sendNewPulses = false;

                    if (destinationModuleName == sourceToMachinePower && pulseType == highPulse)
                    {
                        if (!cycles.ContainsKey(sourceModuleName))
                        {
                            cycles[sourceModuleName] = buttonPresses;
                        }

                        if (cycles.Count == numOfConcurrentCycles)
                        {
                            part2Output = LCM(cycles.Values.ToArray());
                            break;
                        }
                    }

                    if (destinationModuleType == flipFlop && pulseType == lowPulse)
                    {
                        newMemoryValue = destinationModuleMemory[destinationModuleName] == "off" ? "on" : "off";
                        destinationModuleMemory[destinationModuleName] = newMemoryValue;
                        modules[destinationModuleName] = Tuple.Create(destinationModule.Item1, destinationModule.Item2, destinationModuleMemory, destinationModule.Item4);
                        newPulseType = newMemoryValue == "on" ? highPulse : lowPulse;
                        sendNewPulses = true;
                    }
                    else if (destinationModuleType == conjunction)
                    {
                        // Conjunction modules remember the most recent pulse from its source, so set its memory from the source to this pulse
                        newMemoryValue = pulseType;
                        destinationModuleMemory[sourceModuleName] = newMemoryValue;

                        modules[destinationModuleName] = Tuple.Create(destinationModule.Item1, destinationModule.Item2, destinationModuleMemory, destinationModule.Item4);

                        // If the conjunction module remembered high pulses for all inputs then it sends a low pulse, otherwise it sends a high pulse
                        newPulseType = lowPulse;
                        foreach (var destinationModuleMemoryValue in destinationModuleMemory.Values)
                        {
                            if (destinationModuleMemoryValue == lowPulse)
                            {
                                newPulseType = highPulse;
                                break;
                            }
                        }
                        sendNewPulses = true;
                    }

                    if (sendNewPulses)
                    {
                        // Send the new pulse to all the destinations
                        foreach (var destinationModuleDestination in destinationModuleDestinations)
                        {
                            pulses.Enqueue(Tuple.Create(destinationModuleName, destinationModuleDestination, newPulseType));
                        }
                    }
                }
            }

            return part == 2 ? part2Output : lowPulses * highPulses;
        }

        public static BigInteger Day21(int part = 2)
        {
            var sr = new StreamReader("Day21-Input.txt");
            var line = sr.ReadLine();
            const char rock = '#';
            int targetSteps = 64;
            List<List<char>> grid = new();

            while (line != null)
            {
                grid.Add(line.ToCharArray().ToList());

                line = sr.ReadLine();
            }

            Tuple<int, int> start = Tuple.Create(0, 0);
            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0;  j < grid[i].Count; j++)
                {
                    if (grid[i][j] == 'S')
                    {
                        start = Tuple.Create(i, j);
                        break;
                    }
                }
            }

            HashSet<Tuple<int, int>> visited = new()
            {
                Tuple.Create(start.Item1, start.Item2),
            };
            HashSet<Tuple<int, int>> validFinalSteps = new();
            // Tuple is row, column, and number of steps
            Queue<Tuple<int, int, BigInteger>> queue = new();
            queue.Enqueue(Tuple.Create(start.Item1, start.Item2, BigInteger.Zero));
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var row = current.Item1;
                var column = current.Item2;
                var currentSteps = current.Item3;

                if (currentSteps % 2 == targetSteps % 2)
                {
                    validFinalSteps.Add(Tuple.Create(row, column));
                }

                if (currentSteps == targetSteps)
                {
                    continue;
                }

                // Travel in each valid direction
                foreach (GridDirection newDirection in Enum.GetValues(typeof(GridDirection)))
                {
                    if (newDirection == GridDirection.None) continue;
                    var newRow = row + (newDirection == GridDirection.North ? -1 : newDirection == GridDirection.South ? 1 : 0);
                    var newColumn = column + (newDirection == GridDirection.West ? -1 : newDirection == GridDirection.East ? 1 : 0);

                    if (newRow < 0 || newColumn < 0 || newRow > grid.Count - 1 || newColumn > grid[newRow].Count - 1 || visited.Contains(Tuple.Create(newRow, newColumn)) || grid[newRow][newColumn] == rock)
                    {
                        continue;
                    }

                    visited.Add(Tuple.Create(newRow, newColumn));
                    queue.Enqueue(Tuple.Create(newRow, newColumn, currentSteps + 1));
                }
            }

            return validFinalSteps.Count;
        }

        public static BigInteger Day22(int part = 2)
        {
            var sr = new StreamReader("Day22-Input.txt");
            var line = sr.ReadLine();
            BigInteger part1Output = 0;
            BigInteger part2Output = 0;
            // Tuple is 6 values, first three are x,y,z values of one end, second three are x,y,z values of the other end
            var bricks = new List<Tuple<int, int, int, int, int, int>>();

            // Checks if two bricks overlap each other in the (x,y) coordinates. If true it means either brickA can support brickB or vice versa
            var bricksOverlapHorizontally = (Tuple<int, int, int, int, int, int> brickA, Tuple<int, int, int, int, int, int> brickB) =>
            {
                if (Math.Max(brickA.Item1, brickB.Item1) <= Math.Min(brickA.Item4, brickB.Item4) && Math.Max(brickA.Item2, brickB.Item2) <= Math.Min(brickA.Item5, brickB.Item5))
                {
                    return true;
                }

                return false;
            };

            while (line != null)
            {
                var startXYZ = line.Split("~")[0].Split(",").Select(x => Convert.ToInt32(x)).ToList();
                var endXYZ = line.Split("~")[1].Split(",").Select(x => Convert.ToInt32(x)).ToList();
                bricks.Add(Tuple.Create(startXYZ[0], startXYZ[1], startXYZ[2], endXYZ[0], endXYZ[1], endXYZ[2]));

                line = sr.ReadLine();
            }

            // Sort the bricks by their Z value so that we process the ones closer to the ground first so that we can make them fall first
            bricks.Sort((a, b) => a.Item3 - b.Item3);

            // Some bricks haven't finished falling to the ground yet. Perform the act of making all the bricks finish falling
            for (int i = 0; i < bricks.Count; i++)
            {
                var brickA = bricks[i];

                // newBrickHeight indicates where the brick will fall to. If it winds up being 1 then it lands on the ground as nothing was in its way to keep falling
                int newBrickHeight = 1;

                // Since bricks are sorted by height, we only need to check the bricks that come before this one because a brick can only crash into one that's lower to the ground
                for (int j = i - 1; j >= 0; j--)
                {
                    var brickB = bricks[j];
                    if (bricksOverlapHorizontally(brickA, brickB))
                    {
                        // If brickB overlaps brickA, brickA can't fall further than brickB's max height so check all other blocks and find the furthest blockA can fall before it either crashes into the ground or another block
                        newBrickHeight = Math.Max(newBrickHeight, brickB.Item6 + 1);
                    }
                }

                // Move blockA to its new height after falling as far as it can
                bricks[i] = Tuple.Create(brickA.Item1, brickA.Item2, newBrickHeight, brickA.Item4, brickA.Item5, brickA.Item6 - brickA.Item3 + newBrickHeight);
            }

            // Key is a tuple for a brick, value is a set of all bricks that that brick supports
            Dictionary<Tuple<int, int, int, int, int, int>, HashSet<Tuple<int, int, int, int, int, int>>> brickSupports = new();
            // Key is a tuple for a brick, value is a set of all bricks that that brick is supported by
            Dictionary<Tuple<int, int, int, int, int, int>, HashSet<Tuple<int, int, int, int, int, int>>> brickSupportedBy = new();

            for (int i = 0; i < bricks.Count; i++)
            {
                var higherBrick = bricks[i];
                if (!brickSupports.ContainsKey(higherBrick))
                {
                    brickSupports[higherBrick] = new();
                }
                if (!brickSupportedBy.ContainsKey(higherBrick))
                {
                    brickSupportedBy[higherBrick] = new();
                }
                for (int j = i - 1; j >= 0; j--)
                {
                    var lowerBrick = bricks[j];

                    // If higherBrick is directly sitting on top of lower brick then add higherBrick to the list of blocks supported by lowerBrick
                    if (bricksOverlapHorizontally(higherBrick, lowerBrick) && higherBrick.Item3 == lowerBrick.Item6 + 1)
                    {
                        if (!brickSupports.ContainsKey(lowerBrick))
                        {
                            brickSupports[lowerBrick] = new();
                        }
                        if (!brickSupportedBy.ContainsKey(lowerBrick))
                        {
                            brickSupportedBy[lowerBrick] = new();
                        }

                        brickSupports[lowerBrick].Add(higherBrick);
                        brickSupportedBy[higherBrick].Add(lowerBrick);
                    }
                }
            }

            // Calculate which bricks can be safely removed
            for (int i = 0; i < bricks.Count; i++)
            {
                var brick = bricks[i];
                bool canBeRemoved = true;
                var supportedBricks = brickSupports[brick];
                Queue<Tuple<int, int, int, int, int, int>> bricksToCheck = new();
                HashSet<Tuple<int, int, int, int, int, int>> fallingBricks = new() { brick }; // We include this brick itself because we're attempting to remove it

                foreach (var supportedBrick in supportedBricks)
                {
                    if (brickSupportedBy[supportedBrick].Count == 1)
                    {
                        canBeRemoved = false;
                        // For part 2 we need to check all the other bricks supported by this brick to see the chain reaction that's happening if we remove it
                        bricksToCheck.Enqueue(supportedBrick);
                    }
                }

                fallingBricks = fallingBricks.Concat(bricksToCheck).ToHashSet();

                while (bricksToCheck.Count > 0)
                {
                    var brickToCheck = bricksToCheck.Dequeue();

                    // We do an except here because all of the bricks we preciously marked as falling can no longer be a support
                    supportedBricks = brickSupports[brickToCheck].Except(fallingBricks).ToHashSet();

                    foreach (var supportedBrick in supportedBricks)
                    {
                        var remainingSupports = brickSupportedBy[supportedBrick].Except(fallingBricks).ToHashSet();
                        if (remainingSupports.Count == 0)
                        {
                            bricksToCheck.Enqueue(supportedBrick);
                            fallingBricks.Add(supportedBrick);
                        }
                    }
                }

                if (canBeRemoved)
                {
                    part1Output++;
                }

                part2Output += fallingBricks.Count - 1; // We subtract 1 from the count because the brick itself doesn't count as one of the other bricks caught in the chain reaction
            }

            return part != 2 ? part1Output : part2Output;
        }

        public static BigInteger Day23(int part = 2)
        {
            var sr = new StreamReader("Day23-Input.txt");
            var line = sr.ReadLine();
            List<List<char>> grid = new();
            BigInteger output = 0;
            const char pathTile = '.';
            const char forestTile = '#';
            const char northSlope = '^';
            const char eastSlope = '>';
            const char southSlope = 'v';
            const char westSlope = '<';

            bool firstLine = true;
            Tuple<int, int> start = Tuple.Create(0, 0);
            Tuple<int, int> end;

            while (line != null)
            {
                if (firstLine)
                {
                    start = Tuple.Create(0, line.IndexOf(pathTile));
                    firstLine = false;
                }
                grid.Add(line.ToList());

                line = sr.ReadLine();
            }

            end = Tuple.Create(grid.Count - 1, grid[grid.Count - 1].IndexOf(pathTile));

            // Because of the way the graph is formatted, we can convert our graph into something simpler with way fewer points.
            // Important points keeps track of points that either are the start point, end point, or points which contain a branching point.
            // Non branching points only have 1 valid destination they can go (since you can't backtrack), so if you calculate the distance between important points,
            // you can create a graph where all the non branching points don't exist and greatly reduce the computational time
            var importantPoints = new List<Tuple<int, int>>() { start, end };

            for (int row = 0; row < grid.Count; row++)
            {
                for (int column = 0; column < grid[row].Count; column++)
                {
                    if (grid[row][column] == forestTile) continue;

                    var numNeighbors = 0;
                    var forcedSlopeDirection = GridDirection.None;
                    if (part != 2)
                    {
                        if (grid[row][column] == northSlope) { forcedSlopeDirection = GridDirection.North; }
                        if (grid[row][column] == southSlope) { forcedSlopeDirection = GridDirection.South; }
                        if (grid[row][column] == eastSlope) { forcedSlopeDirection = GridDirection.East; }
                        if (grid[row][column] == westSlope) { forcedSlopeDirection = GridDirection.West; }
                    }

                    if (forcedSlopeDirection != GridDirection.None) continue;

                    foreach (GridDirection newDirection in Enum.GetValues(typeof(GridDirection)))
                    {
                        if (newDirection == GridDirection.None) continue;

                        var newRow = row + (newDirection == GridDirection.North ? -1 : newDirection == GridDirection.South ? 1 : 0);
                        var newColumn = column + (newDirection == GridDirection.West ? -1 : newDirection == GridDirection.East ? 1 : 0);

                        if (newRow < 0 || newRow > grid.Count - 1 || newColumn < 0 || newColumn > grid[newRow].Count - 1 || grid[newRow][newColumn] == forestTile) continue;

                        numNeighbors++;
                    }

                    if (numNeighbors > 2)
                    {
                        importantPoints.Add(Tuple.Create(row, column));
                    }
                }
            }

            // Now we create a new graph between the connected important points where the distance between them is the length of the non branching path between them
            // Outer dictionary maps an important point to a list of other important points mapped to the distance between them
            var graph = new Dictionary<Tuple<int, int>, Dictionary<Tuple<int, int>, BigInteger>>();
            var encounteredPoints = new HashSet<Tuple<int, int>>();

            // Travel through the original grid to populate our graph
            for (int i = 0; i < importantPoints.Count; i++)
            {
                var currentPoint = importantPoints[i];
                Queue<Tuple<int, int, BigInteger>> pathBeingTraveled = new();
                pathBeingTraveled.Enqueue(Tuple.Create(currentPoint.Item1, currentPoint.Item2, BigInteger.Zero));
                encounteredPoints.Clear();
                encounteredPoints.Add(currentPoint);

                while (pathBeingTraveled.Count > 0)
                {
                    var currentPathPoint = pathBeingTraveled.Dequeue();
                    var row = currentPathPoint.Item1;
                    var column = currentPathPoint.Item2;
                    var currentPathPointKey = Tuple.Create(row, column);
                    var distance = currentPathPoint.Item3;
                    var forcedSlopeDirection = GridDirection.None;

                    // If we ran into another important point, store the distance between both points in our graph
                    if (distance != 0 && importantPoints.Contains(currentPathPointKey))
                    {
                        if (!graph.ContainsKey(currentPoint))
                        {
                            graph[currentPoint] = new();
                        }

                        graph[currentPoint][currentPathPointKey] = distance;
                        continue;
                    }

                    if (part != 2)
                    {
                        if (grid[row][column] == northSlope) { forcedSlopeDirection = GridDirection.North; }
                        if (grid[row][column] == southSlope) { forcedSlopeDirection = GridDirection.South; }
                        if (grid[row][column] == eastSlope) { forcedSlopeDirection = GridDirection.East; }
                        if (grid[row][column] == westSlope) { forcedSlopeDirection = GridDirection.West; }
                    }

                    // Travel along the grid path in each valid direction
                    foreach (GridDirection newDirection in Enum.GetValues(typeof(GridDirection)))
                    {
                        if (newDirection == GridDirection.None) continue;

                        var newRow = row + (newDirection == GridDirection.North ? -1 : newDirection == GridDirection.South ? 1 : 0);
                        var newColumn = column + (newDirection == GridDirection.West ? -1 : newDirection == GridDirection.East ? 1 : 0);

                        if (newRow < 0 || newRow > grid.Count - 1 || newColumn < 0 || newColumn > grid[newRow].Count - 1 || grid[newRow][newColumn] == forestTile || encounteredPoints.Contains(Tuple.Create(newRow, newColumn))) continue;
                        if (forcedSlopeDirection != GridDirection.None && forcedSlopeDirection != newDirection) continue;

                        pathBeingTraveled.Enqueue(Tuple.Create(newRow, newColumn, distance + 1));
                        encounteredPoints.Add(Tuple.Create(newRow, newColumn));
                    }
                }
            }

            // Now do a depth first search to find the maximum distance between the start point and the end point
            var pointsBeingTraveled = new PriorityQueue<Tuple<int, int, BigInteger, HashSet<Tuple<int, int>>>, BigInteger>();
            pointsBeingTraveled.Enqueue(Tuple.Create(start.Item1, start.Item2, BigInteger.Zero, new HashSet<Tuple<int, int>>()), 0);

            while (pointsBeingTraveled.Count > 0)
            {
                var currentPoint = pointsBeingTraveled.Dequeue();
                var currentPointKey = Tuple.Create(currentPoint.Item1, currentPoint.Item2);
                var distance = currentPoint.Item3;
                encounteredPoints = currentPoint.Item4;

                // If we reached the end point then check if we traveled further than our previous best, store the output, and finish this path
                if (currentPoint.Item1 == end.Item1 && currentPoint.Item2 == end.Item2)
                {
                    if (output < currentPoint.Item3)
                    {
                        output = currentPoint.Item3;
                    }

                    continue;
                }

                var encounteredPointsCopy = encounteredPoints.ToArray().ToHashSet();
                encounteredPointsCopy.Add(currentPointKey);

                var connectedPoints = graph[currentPointKey];
                foreach (var connectedPoint in connectedPoints)
                {
                    if (encounteredPointsCopy.Contains(connectedPoint.Key)) continue;

                    var newDistance = distance + connectedPoint.Value;
                    pointsBeingTraveled.Enqueue(Tuple.Create(connectedPoint.Key.Item1, connectedPoint.Key.Item2, newDistance, encounteredPointsCopy), 0 - newDistance);
                }
            }

            return output;
        }
    }
}
