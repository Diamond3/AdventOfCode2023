namespace AdventOfCode;
public class Day10 : IRunnable
{
    const string FilePath = "data/day10_data.txt";

    private List<List<char>> _mainMap = new();
    private List<List<char>> _map = new();
    
    private Dictionary<char, List<short>> _directions = new ()
        {
            { '|', new List<short>() { 0, 2} },
            { '-', new List<short>() { 1, 3 } },
            { 'L', new List<short>() { 0, 1 } },
            { 'J', new List<short>() { 0, 3 } },
            { '7', new List<short>() { 2, 3 } },
            { 'F', new List<short>() { 1, 2 } },
            { 'S', new List<short>() { 0, 1, 2, 3 } }

        };

    private HashSet<(int, int, int, int)> _alreadySqueezed = new();

    private static int _pathLength = 0;
    private static int[] _current = new int[2];
    private static int[] _start = new int[2];

    private List<string> _startPoints = new();

    private List<int> _lastColumn = new();
    private long _finalSum = 0;

    private void ProcessFile()
    {
        try
        {
            using var sr = new StreamReader(FilePath);
            string line;
            int lineNumber = 0;

            while ((line = sr.ReadLine()) != null)
            {
                _map.Add(new List<char>());
                _map.Last().AddRange(line.ToCharArray());
                _mainMap.Add(new List<char>());
                _mainMap.Last().AddRange(line.ToCharArray());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    public void Run()
    {
        ProcessFile();
        SetStart();
        _start[0] = _current[0];
        _start[1] = _current[1];

        Walk();
        ConvertALettersToPipes();

        OutputMap();

        EnlargeMap();

        OutputMap();

        CheckFromSidesAndFillO();

        OutputMap();

        ShrinkMap();

        //CheckFromSidesAndFillOOnlySqueezing();

        OutputMap();
        var ans = CountDots();
        Console.WriteLine(ans);
    }

    private int CountDots()
    {
        var c = 0;
        for (int i = 0; i < _map.Count; i++)
        {
            for (int j = 0; j < _map[0].Count; j++)
            {
                if (_map[i][j] == '.')
                {
                    c++;
                }
            }
        }
        return c;
    }

    private void ShrinkMap()
    {
        List<List<char>> tempMap = new();
        for (int i = 0; i < _map.Count; i += 2)
        {
            var l = new List<char>();
            for (int j = 0; j < _map[0].Count; j += 2) //down
            {
                l.Add(_map[i][j]);
            }
            tempMap.Add(l);
        }
        _map = tempMap;
    }

    private void EnlargeMap()
    {
        List<List<char>> tempMap = new();
        for (int i = 0; i < _map.Count; i++)
        {
            var l = new List<char>();
            var l2 = new List<char>();
            for (int j = 0; j < _map[0].Count; j++) //down
            {
                l.Add(_map[i][j]);
                if (_map[i][j] == 'F' || _map[i][j] == 'L' || _map[i][j] == 'S' || _map[i][j] == '-')
                {
                    l.Add('-');
                }
                else
                {
                    l.Add('.');
                }

                if (_map[i][j] == '7' || _map[i][j] == 'F' || _map[i][j] == 'S' || _map[i][j] == '|')
                {
                    l2.Add('|');
                }
                else
                {
                    l2.Add('.');
                }

                if (l.Last() == '|')
                {
                    l2.Add('|');
                }
                else
                {
                    l2.Add('.');
                }
            }
            tempMap.Add(l);
            tempMap.Add(l2);
        }
        _map = tempMap;
    }

    private void ConvertALettersToPipes()
    {
        for (int i = 0; i < _map.Count; i++)
        {
            for (int j = 0; j < _map[0].Count; j++)
            {
                if (_map[i][j] == 'A')
                {
                    _map[i][j] = _mainMap[i][j];
                }
                else
                {
                    _map[i][j] = '.';
                }
            }
        }
    }

    private void CheckFromSidesAndFillO()
    {
        for (int i = 0; i < _map[0].Count; i++) //top line
        {
            if (_map[0][i] == '.')
            {
                CalculateOpenAreasWithCorners(0, i);
            }
        }

        for (int i = 0; i < _map[0].Count; i++) //bot line
        {
            if (_map[_map.Count - 1][i] == '.')
            {
                CalculateOpenAreasWithCorners(_map.Count - 1, i);
            }
        }

        for (int i = 0; i < _map.Count; i++) //left line
        {
            if (_map[i][0] == '.')
            {
                CalculateOpenAreasWithCorners(i, 0);
            }
        }

        for (int i = 0; i < _map.Count; i++) //right line
        {
            if (_map[i][_map[0].Count - 1] == '.')
            {
                CalculateOpenAreasWithCorners(i, _map[0].Count - 1);
            }
        }
    }

    private void OutputMap(int? y0 = null, int? x0 = null, int? y1 = null, int? x1 = null)
    {
        Console.WriteLine();
        for (int i = 0; i < _map.Count; i++)
        {
            var str = "";
            for (int j = 0; j < _map[0].Count; j++)
            {
                if (x0 != null && x0 == j && y0 == i)
                {
                    str += 'A';
                }
                else if (x0 != null && x1 == j && y1 == i)
                {
                    str += 'B';
                }
                else
                {
                    str += _map[i][j];
                }
            }
            Console.WriteLine(str);
        }
    }

    private void CalculateOpenAreas(int y, int x)
    {   
        _map[y][x] = 'O';
        if(IsAbleToSpread(y - 1, x)) //Up
        {
            CalculateOpenAreas(y - 1, x);
        }
        if (IsAbleToSpread(y + 1, x)) //Down
        {
            CalculateOpenAreas(y + 1, x);
        }
        if (IsAbleToSpread(y, x + 1)) //Right
        {
            CalculateOpenAreas(y, x + 1);
        }
        if (IsAbleToSpread(y, x - 1)) //Left
        {
            CalculateOpenAreas(y, x - 1);
        }
    }

    private void CalculateOpenAreasWithCorners(int startY, int startX)
    {
        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push((startY, startX));

        while (stack.Count > 0)
        {
            (int y, int x) = stack.Pop();

            if (_map[y][x] != '.') continue;

            _map[y][x] = 'O';

            if (IsAbleToSpread(y - 1, x)) //Up
                stack.Push((y - 1, x));
            if (IsAbleToSpread(y + 1, x)) //Down
                stack.Push((y + 1, x));
            if (IsAbleToSpread(y, x + 1)) //Right
                stack.Push((y, x + 1));
            if (IsAbleToSpread(y, x - 1)) //Left
                stack.Push((y, x - 1));
        }
    }

    private void SetStart()
    {
        for (int i = 0; i < _map.Count; i++)
        {
            for (int j = 0; j < _map[0].Count; j++)
            {
                if (_map[i][j] == 'S')
                {
                    _current[0] = i;
                    _current[1] = j;
                    return;
                }
            }
        }
    }

    private void Walk()
    {
        do
        {
            var c = _map[_current[0]][_current[1]];
            if (c == '.' || c == 'A') //remove 'A' later
            {
                return;
            }
            _map[_current[0]][_current[1]] = 'A';
            var d = _directions[c];
            foreach (var dir in d)
            {
                int[] nextPoint = new int[] { _current[0], _current[1] };
                switch (dir)
                {
                    case 0: //Up
                        if (IsValidMove(_current[0] - 1, _current[1], 2))
                        {
                            nextPoint[0]--;
                        } 
                        break;
                    case 1: //Right
                        if (IsValidMove(_current[0], _current[1] + 1, 3))
                        {
                            nextPoint[1]++;
                        }

                        break;
                    case 2: //Down
                        if (IsValidMove(_current[0] + 1, _current[1], 0))
                        {
                            nextPoint[0]++;
                        }

                        break;
                    case 3: //Left
                        if (IsValidMove(_current[0], _current[1] - 1, 1))
                        {
                            nextPoint[1]--;
                        }
                        break;
                }
                if (nextPoint[0] != _current[0] || nextPoint[1] != _current[1])
                {
                    _current[0] = nextPoint[0];
                    _current[1] = nextPoint[1];
                    break;
                }
            }
            _pathLength++;
        }
        while (_start[0] != _current[0] || _start[1] != _current[1]);
    }

    private bool IsValidMove(int y, int x, short d)
    {
        if (y < _map.Count && y >= 0 && x < _map[0].Count && x >= 0)
        {
            var c = _map[y][x];
            if (c == '.' || c == 'A')
            {
                return false;
            }
            if (_directions[c].Contains(d))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsAbleToSpread(int y, int x)
    {
        if (y < _map.Count && y >= 0 && x < _map[0].Count && x >= 0)
        {
            var c = _map[y][x];
            if (c == '.')
            {
                return true;
            }
        }
        return false;
    }

    private bool ValidSqueezDir(int y0, int x0, int y1, int x1)
    {
        if (y0 < _map.Count && y0 >= 0 && x0 < _map[0].Count && x0 >= 0
            && y1 < _map.Count && y1 >= 0 && x1 < _map[0].Count && x1 >= 0)
        {
            return true;
        }
        return false;
    }
    private bool IsAbleToSqueez(int[] c, int d)
    {
        return IsAbleToSqueez(c[0], c[1], c[2], c[3], d);
    }

    private int[] Rotate(int y0, int x0, int dir)
    {
        if (dir == 0 || dir == 2)
        {
            return new int[4] {y0, x0, y0, x0 + 1};
        }
        else
        {
            return new int[4] { y0, x0, y0 + 1, x0 };
        }
    }

    private bool IsAbleToSqueez(int y0, int x0, int y1, int x1, int dir)
    {
        if (dir == 0) //top
        {
            if (ValidSqueezDir(y0 - 1, x0, y1 - 1, x1))
            {
                var c0 = _map[y0 - 1][x0];
                var c1 = _map[y1 - 1][x1];

                if (!_directions.ContainsKey(c0) || !_directions.ContainsKey(c1))
                {
                    return true;
                }

                var d0 = _directions[c0];
                var d1 = _directions[c1];

                return !(d0.Contains(1) && d1.Contains(3));
            }
            return false;
        }
        if (dir == 1) //right
        {
            if (ValidSqueezDir(y0, x0 + 1, y1, x1 + 1))
            {
                var c0 = _map[y0][x0 + 1];
                var c1 = _map[y1][x1 + 1];

                if (!_directions.ContainsKey(c0) || !_directions.ContainsKey(c1))
                {
                    return true;
                }

                var d0 = _directions[c0];
                var d1 = _directions[c1];

                return !(d0.Contains(2) && d1.Contains(0));
            }
            return false;
        }
        if (dir == 2)//down
        {
            if (ValidSqueezDir(y0 + 1, x0, y1 + 1, x1))
            {
                var c0 = _map[y0 + 1][x0];
                var c1 = _map[y1 + 1][x1];

                if (!_directions.ContainsKey(c0) || !_directions.ContainsKey(c1))
                {
                    return true;
                }

                var d0 = _directions[c0];
                var d1 = _directions[c1];

                return !(d0.Contains(1) && d1.Contains(3));
            }
            return false;
        }
        if (dir == 3) //left
        {
            if (ValidSqueezDir(y0, x0 - 1, y1, x1 - 1))
            {
                var c0 = _map[y0][x0 - 1];
                var c1 = _map[y1][x1 - 1];

                if (!_directions.ContainsKey(c0) || !_directions.ContainsKey(c1))
                {
                    return true;
                }

                var d0 = _directions[c0];
                var d1 = _directions[c1];

                return !(d0.Contains(2) && d1.Contains(0));
            }
            return false;
        }
        return false;
    }
}
