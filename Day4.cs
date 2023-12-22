namespace AdventOfCode;
public class Day4: IRunnable
{
    const string FilePath = "data/day4_data.txt";

    private int _scratcherIndex = 0;
    private int _scratcherCount = 0;

    private bool[] _luckyNumbers = new bool[100];
    private int[] _scratchcardsCounts = new int[300];

    private void ProcessFile()
    {
        try
        {
            using var sr = new StreamReader(FilePath);
            string line;
            int lineNumber = 0;

            Array.Fill(_scratchcardsCounts, 1);

            while ((line = sr.ReadLine()) != null)
            {
                lineNumber++;
                _scratcherIndex++;

                ProcessLine(line);
                _scratcherCount += _scratchcardsCounts[_scratcherIndex];
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
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
        Console.WriteLine(_scratcherCount);
    }
}
