namespace AdventOfCode;
public class Day9: IRunnable
{
    const string FilePath = "data/day9_data.txt";

    private Dictionary<string, string[]> _map = new();
    private string _directions;
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
                _lastColumn.Clear();

                var numbs = line.Split(' ');
                var arr = new int[numbs.Length];

                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = int.Parse(numbs[i]);
                }

                CalculateNextNumber(arr);
                var a = 0;
                for (int i = _lastColumn.Count - 1; i >= 0; i--)
                {
                    var b = _lastColumn[i];
                    a = b - a;
                }
                _finalSum += a;

                Console.WriteLine($"Line: {lineNumber}");
                lineNumber++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    private void CalculateNextNumber(int[] l)
    {
        _lastColumn.Add(l.First());
        var n = GetLowerArray(l);
        if (n.All(x => x == 0))
        {
            return;
        }
        CalculateNextNumber(n);
    }

    private int[] GetLowerArray(int[] arr)
    {
        var tempArr = new int[arr.Length - 1];
        for (int i = arr.Length - 1; i > 0; i--)
        {
            tempArr[i - 1] = arr[i] - arr[i - 1];
        }
        return tempArr;
    }

    public void Run()
    {
        ProcessFile();
        Console.WriteLine(_finalSum);
    }
}
