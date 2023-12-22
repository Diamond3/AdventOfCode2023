namespace AdventOfCode;
public class Day5: IRunnable
{
    const string FilePath = "data/day5_data.txt";

    private int _scratcherIndex = 0;
    private int _scratcherCount = 0;

    private bool[] _luckyNumbers = new bool[100];
    private int[] _scratchcardsCounts = new int[300];

    private List<long> _loc;

    private long _minLoc = long.MaxValue;

    private List<bool> _checked = new();

    private int _state;

    private List<(long, long)> _pairs = new();
    //private List<long[]> _map = new();

    private List<long[]>[] _map = new List<long[]>[7];

    private void ProcessFile()
    {
        try
        {
            using var sr = new StreamReader(FilePath);
            string line;
            int lineNumber = 0;
            _loc = new();

            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;


                if (_state == 0)
                {
                    var parts = line.Split(':');
                    var nums = parts[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToList();
                    for (int i = 0; i < nums.Count; i += 2)
                    {
                        _pairs.Add((nums[i], nums[i + 1]));
                        /*for (int j = 0; j < nums[i + 1]; j++)
                        {
                            _loc.Add(nums[i] + j);
                            _checked.Add(false);
                        }*/
                    }
                    //Console.WriteLine($"Count - {_loc.Count}");
                    _state++;
                    continue;
                }

                if (!char.IsNumber(line[0]))
                {
                    _state++;
                    _map[_state - 2] = new List<long[]>();
                    //Console.WriteLine($"State: {_state}");

                    _checked = _checked.Select(x => false).ToList();
                    continue;
                }

                var numbs = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var j = numbs.Select(x => long.Parse(x)).ToArray();
                _map[_state - 2].Add(j);


                //CalculatePositions(line);
                //199602917
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
    private void CalculatePositions(string line)
    {
        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var j = parts.Select(x => long.Parse(x)).ToArray();

        for (int i = 0; i < _loc.Count; i++)
        {
            var loc = _loc[i];
            if (!_checked[i] && loc >= j[1] && loc < j[1] + j[2])
            {
                loc = j[0] + (loc - j[1]);
                //79
                //79 >= 50 && 79 < 98
                //79 = 52 + (79 - 50)
                if (_state == 8 && loc < _minLoc)
                {
                    _minLoc = loc;
                }
                _loc[i] = loc;
                _checked[i] = true;
            }

            if (_state == 8 && loc < _minLoc)
            {
                _minLoc = loc;
            }
        }

        
    }

    private void ProcessLine(string line)
    {
        var splited = line.Split(new char[] { '|', ':'});
        var count = 0;

        _luckyNumbers = new bool[100];

        foreach (var num in splited[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            _luckyNumbers[int.Parse(num) - 1] = true;
        }

        foreach(var num in splited[2].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (_luckyNumbers[int.Parse(num) - 1])
                count++;
        }

        for (int i = 0; i < count; i++)
        {
            _scratchcardsCounts[_scratcherIndex + i + 1] += _scratchcardsCounts[_scratcherIndex];
        }
    }

    public void Run()
    {
        ProcessFile();

        foreach(var pair in _pairs)
        {
            Console.WriteLine("New Seed Pair...");
            for (int i = 0; i < pair.Item2; i++)
            {
                var loc = RunThroughMap(pair.Item1 + i);

                if (loc < _minLoc)
                {
                    _minLoc = loc;
                }
            }
        }
        Console.WriteLine(_minLoc);

        string filePath = "output_day5.txt";
        File.WriteAllText(filePath, _minLoc.ToString());
        Console.WriteLine($"Number written to {filePath}");
    }

    private long RunThroughMap(long loc)
    {
        for (int x = 0; x < _map.Length; x++)
        {
            for (int i = 0; i < _map[x].Count; i++)
            {
                var j = _map[x][i];
                if (loc >= j[1] && loc < j[1] + j[2])
                {
                    loc = j[0] + (loc - j[1]);
                    //79
                    //79 >= 50 && 79 < 98
                    //79 = 52 + (79 - 50)
                    break;
                }
            }
        }
        return loc;
    }
}
