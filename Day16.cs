using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day16: IRunnable
{

    string fileName = "data/day16_data.txt";
    private int _finalSum = 0;

    private List<char[]> _map = [];
    private HashSet<(int, int)> _energizedTiles = [];
    private HashSet<(int, int, int)> _cache = [];

    private string _currentString = "HASH";
    private long _currentValue = 0;

    void ProcessFile()
    {
        try
        {
            var regex = new Regex(@"^([a-zA-Z]+)([-=])(\d*)$");

            using (var sr = new StreamReader(fileName))
            {
                string line;
                int lineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    _map.Add(line.ToCharArray());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    // 0 - up
    // 1 - right
    // 2 - down
    // 3 - left

    private void BeamLight(int x, int y, int direction)
    {
        if (_cache.Contains((x, y, direction)))
        {
            return;
        }
        else
        {
            _cache.Add((x, y, direction));
        }

        _energizedTiles.Add((x, y));
        //PrintMap();

        if (!IsValidDirection(x, y, direction)) return;


        if (direction == 0)
        {
            y--;
        }
        else if (direction == 1)
        {
            x++;
        }
        else if (direction == 2)
        {
            y++;
        }
        else if (direction == 3)
        {
            x--;
        }


        var nextDirs = GetNextDirections(x, y, direction);

        foreach (var dir in nextDirs)
        {
            BeamLight(x, y, dir);
        }
    }

    private void PrintMap()
    {
        for (int i = 0; i < _map.Count; i++)
        {
            var str = "";
            for (int j = 0; j < _map[0].Length; j++)
            {
                if (_energizedTiles.Contains((j, i)))
                {
                    str += "#";
                }
                else
                {
                    str += _map[i][j];
                }
            }
            Console.WriteLine(str);
        }
        Console.WriteLine();
    }

    private int[] GetNextDirections(int x, int y, int direction)
    {
        switch (_map[y][x])
        {
            case '.':
                return [direction];

            case '|':
                if (direction == 0 || direction == 2)
                {
                    return [direction];
                }

                return [(direction + 1) % 4, direction - 1];

            case '-':
                if (direction == 1 || direction == 3)
                {
                    return [direction];
                }

                return [((direction - 1) + 4) % 4, direction + 1];

            case '\\':
                if (direction == 0 || direction == 2)
                {
                    return [((direction - 1) + 4) % 4];
                }

                return [(direction + 1) % 4];

            case '/':
                if (direction == 0 || direction == 2)
                {
                    return [(direction + 1) % 4];
                }

                return [((direction - 1) + 4) % 4];
        }
        return [];
    }

    private bool IsValidDirection(int x, int y, int direction)
    {
        if (direction == 0 && y - 1 < 0)
        {
            return false;
        }
        else if (direction == 1 && x + 1 >= _map[0].Length)
        {
            return false;
        }
        else if (direction == 2 && y + 1 >= _map.Count)
        {
            return false;
        }
        else if (direction == 3 && x - 1 < 0)
        {
            return false;
        }
        return true;
    }

    public void Run()
    {
        ProcessFile();
        //PrintMap();
        var maxCount = 0;

        for (int i = 0; i < _map.Count; i++)
        {
            var nextDirs = GetNextDirections(0, i, 1);

            foreach (var dir in nextDirs)
            {
                BeamLight(0, i, dir);
            }
            if (maxCount < _energizedTiles.Count)
            {
                maxCount = _energizedTiles.Count;
            }

            _energizedTiles.Clear();
            _cache.Clear();
        }

        for (int i = 0; i < _map.Count; i++)
        {
            var nextDirs = GetNextDirections(_map[0].Length - 1, i, 3);

            foreach (var dir in nextDirs)
            {
                BeamLight(_map.Count - 1, i, dir);
            }
            if (maxCount < _energizedTiles.Count)
            {
                maxCount = _energizedTiles.Count;
            }

            _energizedTiles.Clear();
            _cache.Clear();
        }

        for (int i = 0; i < _map[0].Length; i++)
        {
            var nextDirs = GetNextDirections(i, 0, 2);

            foreach (var dir in nextDirs)
            {
                BeamLight(i, 0, dir);
            }
            if (maxCount < _energizedTiles.Count)
            {
                maxCount = _energizedTiles.Count;
            }

            _energizedTiles.Clear();
            _cache.Clear();
        }

        for (int i = 0; i < _map[0].Length; i++)
        {
            var nextDirs = GetNextDirections(i, _map.Count - 1, 2);

            foreach (var dir in nextDirs)
            {
                BeamLight(i, 0, dir);
            }
            if (maxCount < _energizedTiles.Count)
            {
                maxCount = _energizedTiles.Count;
            }

            _energizedTiles.Clear();
            _cache.Clear();
        }

        /*var nextDirs = GetNextDirections(0, 0, 1);

        foreach (var dir in nextDirs)
        {
            BeamLight(0, 0, dir);
        }*/
        //PrintMap();

        //Console.WriteLine();

        Console.WriteLine(maxCount);
    }
}