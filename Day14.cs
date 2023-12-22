using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AdventOfCode;

public class Day14: IRunnable
{

    string fileName = "data/day14_data.txt";
    private int _finalSum = 0;

    private List<char[]> _map = new List<char[]>();

    void ProcessFile()
    {
        try
        {
            using (var sr = new StreamReader(fileName))
            {
                string line;
                int lineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
/*                    if (line == "")
                    {
                        ProcessMap();

                        _map.Clear();
                        continue;
                    }*/
                    _map.Add(line.ToCharArray());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    void RotateRight()
    {
        for (int y = 0; y < _map.Count; y++)
        {
            Push(y);
        }

        void Push(int y)
        {
            for (int x = _map[0].Length - 1; x >= 0; x--) 
            {
                if (_map[y][x] == 'O')
                {
                    var i = x;
                    _map[y][x] = '.';
                    while (i < _map[0].Length) 
                    {
                        if (_map[y][i] == '#' || _map[y][i] == 'O')
                        {
                            break;
                        }
                        i++;
                    }
                    i--;
                    _map[y][i] = 'O';
                }
            }
        }
    }

    void RotateLeft()
    {
        for (int y = 0; y < _map.Count; y++)
        {
            Push(y);
        }

        void Push(int y)
        {
            for (int x = 0; x < _map[0].Length; x++)
            {
                if (_map[y][x] == 'O')
                {
                    var i = x;
                    _map[y][x] = '.';
                    while (i >= 0)
                    {
                        if (_map[y][i] == '#' || _map[y][i] == 'O')
                        {
                            break;
                        }
                        i--;
                    }
                    i++;
                    _map[y][i] = 'O';
                }
            }
        }
    }

    void RotateUp()
    {
        for (int x = 0; x < _map[0].Length; x++)
        {
            Push(x);
        }

        void Push(int x)
        {
            for (int y = 0; y < _map.Count; y++)
            {
                if (_map[y][x] == 'O')
                {
                    var i = y;
                    _map[y][x] = '.';
                    while (i >= 0)
                    {
                        if (_map[i][x] == '#' || _map[i][x] == 'O')
                        {
                            break;
                        }
                        i--;
                    }
                    i++;
                    _map[i][x] = 'O';
                }
            }
        }
    }

    void RotateDown()
    {
        for (int x = 0; x < _map[0].Length; x++)
        {
            Push(x);
        }

        void Push(int x)
        {
            for (int y = _map.Count - 1; y >= 0; y--)
            {
                if (_map[y][x] == 'O')
                {
                    var i = y;
                    _map[y][x] = '.';
                    while (i < _map.Count)
                    {
                        if (_map[i][x] == '#' || _map[i][x] == 'O')
                        {
                            break;
                        }
                        i++;
                    }
                    i--;
                    _map[i][x] = 'O';
                }
            }
        }
    }

    private void ProcessMap()
    {
        for (int i = 0; i < _map.Count; i++)
        {
            var c = _map[i].Count(x => x == 'O');
            _finalSum += c * (_map[i].Length - i);
        }

        return;
        var score = 0;
        for (int x = 0; x < _map[0].Length; x++)
        {
            var count = 0;
            for (int y = _map.Count - 1; y >= 0; y--)
            {
                if (_map[y][x] == 'O')
                {
                    count++;
                }
                else if(_map[y][x] == '#')
                {
                    if (count > 0)
                    {
                        //i[4]
                        var l = _map.Count - 1 - y;
                        var a = (float)(l + (l - count + 1)) / 2 * count;
                        score += (int)a;
                        count = 0;
                    }
                }
            }
            if (count > 0)
            {
                // 4
                // 10 + 9 + 8 + 7 = 34
                // len + len - 1 + len - 2
                var l = _map.Count;
                //10 + (10 - 4 + 1) = 10 + 7 / 2 * 4
                float a = (l + (l - count + 1));

                var b = a / 2 * count;
                score += (int)b;
                count = 0;
            }
        }
/*        var vertical = FindVerticalMirrors(_map);
        var horizontal = FindHorizontalMirrors(_map);
        var ats = vertical + horizontal * 100;*/
        _finalSum += score;
    }

    public void Run()
    {
        ProcessFile();
        //PrintMap(_map);
        var mainList = new List<(List<char[]>, int)>();

        var originalMap = new List<char[]>(_map.Count);



        //Console.WriteLine(CompareToOriginal(originalMap));
        // 2 -> 9 == 7

        //171 -> 120
        //n = 121

        var low = 80;
        var high = 131;
        var diff = high - low;//131 - 80;
        var realNum = 1000000000 - high;
        var mod = realNum % diff;
        var n = mod + low;

        for (int i = 0; i < n; i++)
        {
            //Console.WriteLine(i);

            RotateUp();
            RotateLeft();
            RotateDown();
            RotateRight();

            //checking
            /*originalMap = new List<char[]>(_map.Count);

            foreach (char[] array in _map)
            {
                var arrayCopy = new char[array.Length];
                Array.Copy(array, arrayCopy, array.Length);
                originalMap.Add(arrayCopy);
            }

            if (!AlreadyContains(mainList, originalMap))
            {
                mainList.Add((originalMap, i));
            }
            else
            {
                var a = AlreadyContainsWithInt(mainList, originalMap);
                Console.WriteLine($"{i} -> {a}");
                //PrintMap(_map);
                //PrintMap(originalMap);
            }*/

            //if (i == 2 || i == 9)
            //Console.WriteLine();
        }

        //PrintMap(_map);



        ProcessMap();
        Console.WriteLine(_finalSum);
    }

    private bool AlreadyContains(List<(List<char[]>, int)> mainList, List<char[]> originalMap)
    {
        foreach (var a in mainList)
        {
            (List<char[]> map, int n) = a;

            if (SameMap(map, originalMap))
            {
                return true;
            }
        }
        return false;
    }

    private int AlreadyContainsWithInt(List<(List<char[]>, int)> mainList, List<char[]> originalMap)
    {
        foreach (var a in mainList)
        {
            (List<char[]> map, int n) = a;

            if (SameMap(map, originalMap))
            {
                return n;
            }
        }
        return -1;
    }

    private bool SameMap(List<char[]> map1, List<char[]> map2)
    {
        for (int i = 0; i < _map.Count; i++)
        {
            var str1 = new string(map1[i]);
            var str2 = new string(map2[i]);
            if (str1 != str2)
            {
                return false;
            }
        }
        return true;
    }

    private bool CompareToOriginal(List<char[]> originalMap)
    {
        for (int i = 0; i < _map.Count; i++)
        {
            var str1 = new string(_map[i]);
            var str2 = new string(originalMap[i]);
            if (str1 != str2)
            {
                return false;
            }
        }
        return true;
    }

    private void PrintMap(List<char[]> m)
    {
        foreach (var map in m)
        {
            Console.WriteLine(new string(map));
        }
        Console.WriteLine();
    }
}