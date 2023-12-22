namespace AdventOfCode;
public class Day6: IRunnable
{
    const string FilePath = "data/day6_data.txt";

    private List<long> _times = new();
    private List<long> _dists = new();
    private long[] _best;

    private void ProcessFile()
    {
        try
        {
            using var sr = new StreamReader(FilePath);
            string line;
            int lineNumber = 0;

            while ((line = sr.ReadLine()) != null)
            {
                var num = line.Split(":")[1].Replace(" ", "");
                if (lineNumber == 1)
                {
                    _dists.Add(long.Parse(num));
                }
                else
                {
                    _times.Add(long.Parse(num));
                }
                /*foreach (var num in l[1].Replace(" ", ""))
                {
                    if (lineNumber == 1)
                    {
                        _dists.Add(int.Parse(num));
                    }
                    else
                    {
                        _times.Add(int.Parse(num));
                    }
                }*/
                lineNumber++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    private void CalculateTimes()
    {
        for (int i = 0; i < _dists.Count; i++)
        {
            for(int j = 1; j < _times[i]; j++)
            {
                var t = _times[i] - j;
                var dist = j * t;
                if (dist > _dists[i])
                {
                    _best[i]++;
                }
            }
        }
    }

    public void Run()
    {
        ProcessFile();
        _best = new long[_dists.Count];
        CalculateTimes();
        var ans = _best.Aggregate(1L, (mul, t) => mul * t);
        Console.WriteLine(ans);
        //41513103
    }
}
