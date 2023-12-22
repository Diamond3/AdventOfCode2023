namespace AdventOfCode;
public class Day1: IRunnable
{
    const string FilePath = "data/day1_data.txt";
    readonly Dictionary<string, int> WordsForNumbers = new Dictionary<string, int>()
    {
        { "one",  1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5 },
        { "six", 6 },
        { "seven",  7 },
        { "eight",  8 },
        { "nine",  9 }
    };

    int _finalSum = 0;
    private void ProcessFile()
    {
        try
        {
            using var sr = new StreamReader(FilePath);
            string line;
            int lineNumber = 0;

            while ((line = sr.ReadLine()) != null)
            {
                lineNumber++;
                ExtractNumbers(line);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    private void ExtractNumbers(string line)
    {
        var c = 0;
        var front = -1;
        var back = -1;
        do
        {
            front = front == -1 ? char.IsNumber(line[c]) ? line[c] - 48 : IsNumberFromFront(line, c) : front;
            back = back == -1 ? char.IsNumber(line[line.Length - c - 1]) ? line[line.Length - c - 1] - 48 : IsNumberFromBack(line, c) : back;

            c++;
        }
        while (front == -1 || back == -1);

        _finalSum += front * 10 + back;
    }

    private int IsNumberFromFront(string line, int c)
    {
        var numbLetters = "";
        for (int i = c; i < line.Length; i++)
        {
            numbLetters += line[i];
            if (numbLetters.Length > 5)
            {
                break;
            }
            if (WordsForNumbers.ContainsKey(numbLetters))
            {
                return WordsForNumbers[numbLetters];
            }
        }
        return -1;
    }

    private int IsNumberFromBack(string line, int c)
    {
        var numbLetters = "";
        for (int i = line.Length - 1 - c; i >= 0; i--)
        {
            numbLetters = line[i] + numbLetters;
            if (numbLetters.Length > 5)
            {
                break;
            }
            if (WordsForNumbers.ContainsKey(numbLetters))
            {
                return WordsForNumbers[numbLetters];
            }
        }
        return -1;
    }

    public void Run()
    {
        ProcessFile();
        Console.WriteLine(_finalSum);
    }
}
