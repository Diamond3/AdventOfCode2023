using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AdventOfCode;

public class Day13: IRunnable
{

    string fileName = "data/day13_data.txt";
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
                    if (line == "")
                    {
                        ProcessMap();

                        _map.Clear();
                        continue;
                    }
                    _map.Add(line.ToCharArray());
                }
            }

            ProcessMap();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void ProcessMap()
    {
        var vertical = FindVerticalMirrors(_map);
        var horizontal = FindHorizontalMirrors(_map);
        var ats = vertical + horizontal * 100;
        _finalSum += ats;
    }

    private int FindVerticalMirrors(List<char[]> map)
    {
        //go vertical
        for (int i = 0; i < map[0].Length - 1; i++)
        {
            if (MirroredVerticaly(i, i + 1))
            {
                return i + 1;
            }
        }
        return 0;
    }
    private int FindHorizontalMirrors(List<char[]> map)
    {
        //go horizontal
        for (int i = 0; i < map.Count - 1; i++)
        {
            if (MirroredHorizontaly(i, i + 1))
            {
                return i + 1;
            }
        }
        return 0;
    }

    private bool MirroredHorizontaly(int x, int y)
    {
        var isValid = true;
        var justOneInvalid = false;

        while (x >= 0 && y < _map.Count)
        {
            for (int i = 0; i < _map[0].Length; i++)
            {
                if (_map[x][i] != _map[y][i])
                {
                    if(!justOneInvalid)
                    {
                        justOneInvalid = true;
                    }
                    else
                        return false;
                }
            }
            x--;
            y++;
        }
        return justOneInvalid == isValid;
    }

    private bool MirroredVerticaly(int x, int y)
    {
        var isValid = true;
        var justOneInvalid = false;

        while (x >= 0 && y < _map[0].Length)
        {
            for (int i = 0; i < _map.Count; i++)
            {
                if (_map[i][x] != _map[i][y])
                {
                    if (!justOneInvalid)
                    {
                        justOneInvalid = true;
                    }
                    else
                        return false;
                }
            }
            x--;
            y++;
        }
        return justOneInvalid == isValid;
    }

    public void Run()
    {
        ProcessFile();
        Console.WriteLine(_finalSum);
    }
}