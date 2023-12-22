using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day15: IRunnable
{

    string fileName = "data/day15_data.txt";
    private int _finalSum = 0;

    private List<char[]> _map = new List<char[]>();

    private List<(string, short)>[] _boxes = new List<(string, short)>[256];

    private string _currentString = "HASH";
    private long _currentValue = 0;

    void ProcessFile()
    {
        for (int j = 0; j < _boxes.Length; j++)
        {
            _boxes[j] = new List<(string, short)>();
        }
        try
        {
            var regex = new Regex(@"^([a-zA-Z]+)([-=])(\d*)$");

            using (var sr = new StreamReader(fileName))
            {
                string line;
                int lineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    var lines = line.Split(',');
                    foreach (var c in lines)
                    {
                        var match = regex.Match(c);

                        var label = match.Groups[1].Value;
                        var sign = char.Parse(match.Groups[2].Value);

                        var i = AddHashToCurrentValue(label);

                        OperateOnSign(i, label, sign, sign == '=' ? int.Parse(match.Groups[3].Value) : 0);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void OperateOnSign(int i, string label, char sign, int strenght)
    {
        if (sign == '=')
        {
            var val = (label, (short)strenght);
            var f = _boxes[i].FindIndex(t => t.Item1 == label);
            if (f != -1)
            {
                _boxes[i][f] = val;
            }
            else
            {
                _boxes[i].Add(val);
            }
        }
        else if (sign == '-')
        {
            var f = _boxes[i].FirstOrDefault(x => x.Item1 == label);
            if (f.Item1 != null)
            {
                _boxes[i].Remove(f);
            }
        }
    }

    private int AddHashToCurrentValue(string str)
    {
        var tempValue = 0;
        foreach (var c in str)
        {
            tempValue += c;
            tempValue *= 17;
            tempValue %= 256;
        }
        return tempValue;
    }

    public void Run()
    {
        ProcessFile();

        for (int x = 0; x < _boxes.Length; x++)
        {
            if (_boxes[x].Count != 0)
            {
                Console.Write($"Box {x}: ");
                _boxes[x].ForEach(x => Console.Write($"[{x.Item1} {x.Item2}] "));
                Console.WriteLine();
            }
        }

        for (int x = 0; x < _boxes.Length; x++)
        {
            if (_boxes[x].Count != 0)
            {
                for (int j = 0; j < _boxes[x].Count; j++)
                {
                    var a = x + 1;
                    var b = j + 1;
                    var c = _boxes[x][j].Item2;

                    var ans = a * b * (int)c;
                    _currentValue += ans;
                    //Console.WriteLine(ans);
                }
            }
        }
        Console.WriteLine();

        Console.WriteLine(_currentValue);
    }
}