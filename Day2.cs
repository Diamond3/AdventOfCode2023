namespace AdventOfCode;
public class Day2: IRunnable
{
    const string FilePath = "data/day2_data.txt";

    const char Red = 'r', Green = 'g', Blue = 'b';

    const int RedRule = 12, GreenRule = 13, BlueRule = 14;

    private int _gameSum = 0;

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
        var lineParts = line.Trim().Split(new char[] { ':', ';'});

        var gameNum = int.Parse(lineParts[0].Split(' ').Last());

        var first = true;

        var colors = new int[3];

        var validRound = true;
        foreach (var game in lineParts)
        {
            if (first)
            {
                first = false;
                continue;
            }
            var round = game.Trim().Split(',');
            foreach (var color in round)
            {
                var colorParts = color.Trim().Split(' ');
                var n = int.Parse(colorParts[0]);
                var c = colorParts[1][0];
                var indx = c == Red ? 0 : c == Green ? 1 : 2;
                if (colors[indx] < n)
                {
                    colors[indx] = n;
                }
            }
            
            /*if (colors[0] <= RedRule && colors[1] <= GreenRule && colors[2] <= BlueRule)
            {
                validRound = true;
            }
            else
            {
                validRound = false;
                break;
            }
            colors = new int[3];*/
        }
        if (validRound)
            _gameSum += colors.Aggregate(1, (acc, val) => acc * val);
    }

    public void Run()
    {
        ProcessFile();
        Console.WriteLine(_gameSum);
    }
}
