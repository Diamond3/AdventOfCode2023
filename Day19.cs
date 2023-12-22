namespace AdventOfCode;

public class Day19 : IRunnable
{
    string fileName = "data/day19_data.txt";

    long _finalSum = 0;

    Dictionary<string, List<string>> _flows = new();

    List<string> _possiblePaths = new();

    void ProcessFile()
    {
        try
        {
            using (var sr = new StreamReader(fileName))
            {
                var currentlyFlows = true;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == string.Empty)
                    {
                        GeneratePossiblePathsWithAccepted("in", "");

                        foreach (var path in _possiblePaths)
                        {
                            CalculatePossibleValues(path);
                        }
                        return;
                    }

                    var parts = line.Split(new char[] { '{', '}', ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    _flows[parts[0]] = new List<string>(parts[1..]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void CalculatePossibleValues(string path)
    {
        var maxValue = 4000;
        var minValue = 1;
        var eqs = path.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var xmas = new (int low, int high)[4]
        {
            (minValue, maxValue),
            (minValue, maxValue),
            (minValue, maxValue),
            (minValue, maxValue)
        };

        var dict = new Dictionary<char, int>()
        {
            { 'x', 0 },
            { 'm', 1 },
            { 'a', 2 },
            { 's', 3 }
        };

        foreach (var eq in eqs)
        {
            var l = dict[eq[0]];
            var sign = eq[1];
            var n = int.Parse(eq[2..]);

            switch (sign)
            {
                //> -> <= -> ^
                //< -> >= -> v

                // m > 2000 E (2001, 4000)
                case '>':
                    xmas[l].low = Math.Max(n + 1, xmas[l].low);
                    break;

                // m < 2000 E (1, 1999)
                case '<':
                    xmas[l].high = Math.Min(n - 1, xmas[l].high);
                    break;

                // m <= 2000 E (1, 2000)
                case '^':
                    xmas[l].high = Math.Min(n, xmas[l].high);
                    break;

                // m >= 2000 E (2000, 4000)
                case 'v':
                    xmas[l].low = Math.Max(n, xmas[l].low);
                    break;
            }
        }
        /*Console.WriteLine(path);
        Console.WriteLine(xmas[0] + " " + xmas[1] + " " + xmas[2] + " " + xmas[3]);*/

        var a = xmas[0].high - xmas[0].low + 1L;
        var b = xmas[1].high - xmas[1].low + 1L;
        var c = xmas[2].high - xmas[2].low + 1L;
        var d = xmas[3].high - xmas[3].low + 1L;

        var combined = a * b * c * d;
        //var combined = (long)xmas[0] * xmas[1] * xmas[2] * xmas[3];
        /*Console.WriteLine(combined);
        Console.WriteLine();*/

        _finalSum += combined;
    }

    private void GeneratePossiblePathsWithAccepted(string key, string v2)
    {
        if (key == "A" || key == "R")
        {
            if (key == "A")
            {
                _possiblePaths.Add(v2);
                /*var a = v2.Replace("v", ">=").Replace("^", "<="); ;
                Console.WriteLine(a);*/
            }

            return;
        }

        var current = _flows[key];
        var currentEq = v2;

        for (int i = 0; i < current.Count; i++)
        {
            var eq = current[i];
            var invertedEq = Invert(eq);
            var nextKeyOnTrue = current[i + 1];

            GeneratePossiblePathsWithAccepted(nextKeyOnTrue, string.Concat(currentEq, " ", eq));
            i++;

            var nextKeyOnFalse = current[i + 1];

            if (nextKeyOnFalse.Length > 1 && (nextKeyOnFalse[1] == '>' || nextKeyOnFalse[1] == '<'))
            {
                currentEq = string.Concat(currentEq, " ", invertedEq);
            }
            else
            {
                GeneratePossiblePathsWithAccepted(nextKeyOnFalse, string.Concat(currentEq, " ", invertedEq));
                i++;
            }
        }
    }

    private string Invert(string v)
    {
        //> -> <= -> ^
        //< -> >= -> v
        if (v[1] == '<')
        {
            return v.Replace("<", "v");
        }
        else if (v[1] == '>')
        {
            return v.Replace(">", "^");
        }

        return v;
    }

    public void Run()
    {
        ProcessFile();
        Console.WriteLine(_finalSum);
    }
}