namespace AdventOfCode;
public class Day8: IRunnable
{
    const string FilePath = "data/day8_data.txt";

    private Dictionary<string, string[]> _map = new();
    private string _directions;
    private List<string> _startPoints = new();

    private void ProcessFile()
    {
        try
        {
            using var sr = new StreamReader(FilePath);
            string line;
            int lineNumber = 0;

            while ((line = sr.ReadLine()) != null)
            {
                if (lineNumber == 0)
                {
                    _directions = line;
                    lineNumber++;
                    continue;
                }
                if (line == "") 
                    continue;

                var parts = line.Split('=');
                var start = parts[0].Trim();
                _map.Add(start, parts[1].Split(new char[] { ' ', '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries));

                if(start[2] == 'A')
                {
                    _startPoints.Add(start);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }    

    private long WalkThrough()
    {
        var count = 0L;
        var currentLoc = "AAA";
        var c = 1;

        var allCounts = new long[_startPoints.Count];

        /*while (count == 0 || !_startPoints[c].EndsWith('Z'))
        {
            var indx = (int)(count % _directions.Length);
            var dir = _directions[indx] == 'L' ? 0 : 1;

            *//*for (int i = 0; i < _startPoints.Count; i++)
            {
                var nextLoc = _map[_startPoints[i]];
                _startPoints[i] = nextLoc[dir];
            }*//*

            var nextLoc = _map[_startPoints[c]];
            _startPoints[c] = nextLoc[dir];

            count++;
        }*/

        while (!_startPoints.All(s => s.EndsWith('Z')))
        {
            var indx = (int)(count % _directions.Length);
            var dir = _directions[indx] == 'L' ? 0 : 1;

            for (int i = 0; i < _startPoints.Count; i++)
            {
                if (_startPoints[i].EndsWith('Z')) continue;

                var nextLoc = _map[_startPoints[i]];
                _startPoints[i] = nextLoc[dir];

                if (_startPoints[i].EndsWith('Z'))
                {
                    allCounts[i] = count + 1;
                    Console.WriteLine(allCounts[i]);
                }
            }
            count++;
        }

        var ans = 1L;
        for (int i = 0; i < allCounts.Length; i++)
        {
            ans = BiggestMultiple(allCounts[i], ans);
        }

        return ans;
    }
    private long BiggestMultiple(long a, long b)
    {
        var lowest = LowestMultiple(a, b);
        return (a * b / lowest);
    }

    private long LowestMultiple(long a, long b)
    {
        if (a < b)
        {
            var temp = b;
            b = a;
            a = temp;
        }

        var lowest = 1L;

        while (b != 0)
        {
            long r = a % b;
            a = b;
            b = r;
        }
        return a;
    }

    public void Run()
    {
        ProcessFile();
        var ans = WalkThrough();
        Console.WriteLine(ans);
    }
}
