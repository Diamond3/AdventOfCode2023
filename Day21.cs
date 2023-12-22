namespace AdventOfCode;

public class Day21: IRunnable
{
    string fileName = "data/day21_data.txt";

    int _finalSum = 0;

    Dictionary<string, List<string>> _flows = new();

    List<char[]> _map = new();

    List<string> _possiblePaths = new();

    (int x, int y) _start = (0, 0);

    HashSet<(int x, int y)> _stepCoordinates = new();

    int _lowestX = int.MaxValue, _lowestY = int.MaxValue;
    int _highestX = int.MinValue, _highestY = int.MinValue;

    void ProcessFile()
    {
        try
        {
            using (var sr = new StreamReader(fileName))
            {
                var currentlyFlows = true;
                string line;
                var lineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    var ch = line.ToCharArray();
                    for (int i = 0; i < ch.Length; i++)
                    {
                        if (ch[i] == 'S')
                        {
                            _start = (i, lineNumber);
                            break;
                        }
                    }

                    _map.Add(ch);

                    lineNumber++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    void Move(int stepCount, List<char[]> map)
    {
        var queue = new Queue<(int x, int y, int step)>();
        var visited = new HashSet<(int x, int y, int step)>();
        _stepCoordinates.Clear();

        _lowestX = int.MaxValue;
        _lowestY = int.MaxValue;
        _highestX = int.MinValue;
        _highestY = int.MinValue;

        _finalSum = 0;


        queue.Enqueue((_start.x, _start.y, 0));
        map[_start.y][_start.x] = '.';

        while (queue.Count > 0)
        {
            var (x, y, step) = queue.Dequeue();

            if (step == stepCount)
            {
                if (x > _highestX)
                    _highestX = x;
                if (x < _lowestX)
                    _lowestX = x;
                if (y > _highestY)
                    _highestY = y;
                if (y < _lowestY)
                    _lowestY = y;

                _stepCoordinates.Add((x, y));
                //map[y][x] = 'O';
                _finalSum++;
            }
            else
            {
                var xLen = map[0].Length;
                var xCurr = ((x) % xLen + xLen) % xLen;
                var xRight = ((x + 1) % xLen + xLen) % xLen;
                var xLeft = ((x - 1) % xLen + xLen) % xLen;

                var yLen = map.Count;
                var yCurr = ((y) % yLen + yLen) % yLen;
                var yUp = ((y + 1) % yLen + yLen) % yLen;
                var yDown = ((y - 1) % yLen + yLen) % yLen;

                if (map[yCurr][xRight] == '.' && !visited.Contains((x + 1, y, step + 1)))
                {
                    queue.Enqueue((x + 1, y, step + 1));
                    visited.Add((x + 1, y, step + 1));
                }
                if (map[yCurr][xLeft] == '.' && !visited.Contains((x - 1, y, step + 1)))
                {
                    queue.Enqueue((x - 1, y, step + 1));
                    visited.Add((x - 1, y, step + 1));

                }
                if (_map[yUp][xCurr] == '.' && !visited.Contains((x, y + 1, step + 1)))
                {
                    queue.Enqueue((x, y + 1, step + 1));
                    visited.Add((x, y + 1, step + 1));

                }
                if (map[yDown][xCurr] == '.' && !visited.Contains((x, y - 1, step + 1)))
                {
                    queue.Enqueue((x, y - 1, step + 1));
                    visited.Add((x, y - 1, step + 1));
                }
            }
        }

    }

    //In original map there is:
    //  7699 circles with 200 and 202 steps
    //  7651 circles with 201 and 155 steps should be same for 26501365 steps

    //7x7 map
    //odd - red, even - blue
    //3 -> 1x odd, 7/7 = 1
    //13 -> 1x odd, 4x even, and some sides 27 -> 21 / 7 = 3 full squares in the middle
    //19 -> 9x odd, 4x even, and some sides 39 -> 35 / 7 = 5 full squares in the middle
    //27 -> 9x odd, 16x even, and some sides 55 -> 49 / 7 = 7 full squares in the middle
    //33 -> 25x odd, 16x even and some sides 67 -> 63 / 7 = 9 full squares in the middle


    //26501365 steps
    //404600 
    //53002731 -> 53002731 / 131 = 404601 squares in the middle row
    //to find even 130th - 64th step map
    //to find full odd 131th - 65th step map

    //202301 - red squares in the middle
    //202300 - blue squares in the middle

    //+40925694601 all odd (65) +40925694601 * 7651
    //+40925290000 all even (64) +40925290000 * 7699
    //-202301 * 4 red corners (65) gaunam -202301 * 3766
    //+202300 * 4 blue corners (64) gaunam +202300 * 3929

    //ats = 40925694601L * 7651L + 40925290000L * 7699L - 202301L * 3766L + 202300L * 3929L;

    public void Run()
    {

        var ats = 40925694601L * 7651L + 40925290000L * 7699L - 202301L * 3766L + 202300L * 3929L;
        Console.WriteLine(ats);

        //recalculate even and odds?

        Console.WriteLine(ats);

        //Console.WriteLine(-5 % 3);
        ProcessFile();
        var lastDiff = 0;
        /*Console.WriteLine(26501365m * 2);
        Console.WriteLine(404600m * 131);
        Console.WriteLine(404601m * 131);
        for (decimal i = 1; i < 26501365m * 2; i++)
        {
            //Console.WriteLine(131m * i);
            if(26501365m * 2 > 131m * i && 26501365m * 2 < 131m * (i + 1))
            {
                Console.WriteLine(i);
                Console.WriteLine("Found");
                break;
            }
        }*/
        var n = 7;

        var l = new int[] { 13, 19, 27, 33, 39 };
        var c = 0;

        var blueNow = true;

        var r = 1;
        var b = 0;

        for (int i = 1; i > 0; i++)
        {
            if (blueNow)
            {
                b += 2;
                blueNow = !blueNow;
            }
            else
            {
                r += 2;
                blueNow = !blueNow;
            }

            if (r + b == 9)
            {
                Console.WriteLine($"step: {r + b}, r = {r}, b = {b}");
                break;
            }
        }

        var R = 202301; //mid row numbers
        var B = 202300;
        var count = 0;
        long sumR = 0;
        long sumB = 0;
        for (int i = 1; i > 0; i++)
        {

            R--;
            B--;

            sumR += R;
            sumB += B;

            count++;
            if (R == 1)
            {
                break;
            }
        }
        sumR *= 2;
        sumB *= 2;//mirror

        sumR += 202301;//add middle
        sumB += 202300;
        Console.WriteLine(sumR); // all full red and blue squares
        Console.WriteLine(sumB);

        Move(64, _map);
        //PrintMap(_map);
        //Console.WriteLine(_finalSum); 
        //130th - 64th 7699 - 3770 = 3929 (even corners)
        //131th - 65th 7651 - 3885 = 3766 (odd corners)
        /*for (int i = 3; i < 30; i++)
        {
            var pred = i * 2 + 1;

                Console.WriteLine(i);
                Move(i, _map);
                PrintMap(_map);
                c++;
                //}
                //Console.WriteLine($"Predicted: {i * 2 + 1}");
                //Console.WriteLine($"n: {i} -> {_highestX - _lowestX + 1}");
                var diff = _highestX - _lowestX + 1;
                //if (diff > 7 * 9 && diff < 70)
                //PrintMap(_map);
                //lastDiff = _highestX - _lowestX + 1;
                //Console.WriteLine($"n={i} -> sum: {_finalSum} -> (width: {_highestX - _lowestX + 1} height: {_highestY - _lowestY + 1})");
                //Console.Write(_finalSum + ", ");

           
            //PrintMap(_map);



        }*/
    }

    private void PrintMap(List<char[]> map)
    {
        var circles = 0;
        var possible = 0L;
        for (int i = _lowestY; i <= _highestY; i++)
        {
            for (int j = _lowestY; j <= _highestX; j++)
            {
                var xLen = map[0].Length;
                var xCurr = ((j) % xLen + xLen) % xLen;

                var yLen = map.Count;
                var yCurr = ((i) % yLen + yLen) % yLen;

                //for visualization
                if (i == yCurr && j == xCurr) //middle
                {
                    if (_stepCoordinates.Contains((j, i)))
                    {
                        possible++;
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                else if (i == yCurr && j > xCurr && j < (xCurr + xLen + xLen)) //first left right top bot
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (i == yCurr && j < xCurr && j > (xCurr - xLen - xLen))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (j == xCurr && i > yCurr && i < (yCurr + yLen + yLen))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (j == xCurr && i < yCurr && i > (yCurr - yLen - yLen))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                else if (i == yCurr && j > xCurr && j < (xCurr + xLen + xLen + xLen)) //second left right top bot
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (i == yCurr && j < xCurr && j > (xCurr - xLen - xLen - xLen))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (j == xCurr && i > yCurr && i < (yCurr + yLen + yLen + yLen))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (j == xCurr && i < yCurr && i > (yCurr - yLen - yLen - yLen))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }

                if (_stepCoordinates.Contains((j, i)))
                {
                    if (i == yCurr && j == xCurr)
                    {
                        circles++;
                    }
                    Console.Write('O');
                }
                else
                {
                    Console.Write(map[yCurr][xCurr]);
                }
                Console.Write(' ');
            }
            Console.WriteLine();
        }

        Console.WriteLine(circles);
        Console.WriteLine("Possible: " + possible);

        /*for (int i = 0; i < map.Count; i++)
        {
            Console.WriteLine(new string(map[i]));
        }*/
    }
}