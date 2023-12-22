namespace AdventOfCode;
public class Day3: IRunnable
{
    const string FilePath = "data/day3_data.txt";

    private int _gameSum = 0;

    private List<string> _board;

    private void ProcessFile()
    {
        try
        {
            using var sr = new StreamReader(FilePath);
            string line;
            int lineNumber = 0;
            _board = new();

            while ((line = sr.ReadLine()) != null)
            {
                lineNumber++;
                _board.Add(line);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    private void RunProcessForGears()
    {
        for (int i = 0; i < _board.Count; i++)
        {
            for (int j = 0; j < _board[0].Length; j++)
            {
                if (_board[i][j] == '*')
                {
                    var set = new HashSet<(int, int)>();
                    
                    if (ScanLeftForNumbStart(i - 1, j - 1, out var a1) == true)
                    {
                        set.Add((i - 1, a1));
                    }
                    if (ScanLeftForNumbStart(i - 1, j, out var a2) == true)
                    {
                        set.Add((i - 1, a2));
                    }
                    if (ScanLeftForNumbStart(i - 1, j + 1, out var a3) == true)
                    {
                        set.Add((i - 1, a3));
                    }

                    if (ScanLeftForNumbStart(i, j - 1, out var b1) == true)
                    {
                        set.Add((i, b1));
                    }
                    if (ScanLeftForNumbStart(i, j + 1, out var b2) == true)
                    {
                        set.Add((i, b2));
                    }

                    if (ScanLeftForNumbStart(i + 1, j - 1, out var c1) == true)
                    {
                        set.Add((i + 1, c1));
                    }
                    if (ScanLeftForNumbStart(i + 1, j, out var c2) == true)
                    {
                        set.Add((i + 1, c2));
                    }
                    if (ScanLeftForNumbStart(i + 1, j + 1, out var c3) == true)
                    {
                        set.Add((i + 1, c3));
                    }

                    if (set.Count > 1)
                    {
                        FillNumber(i, set);
                    }
                }
            }
        }
    }

    private void FillNumber(int i, HashSet<(int, int)> set)
    {
        var a = int.Parse(BuildNumberFromStart(set.First().Item1, set.First().Item2)!);
        var b = int.Parse(BuildNumberFromStart(set.Last().Item1, set.Last().Item2)!);
        Console.WriteLine(a);
        Console.WriteLine(b);
        Console.WriteLine();
        _gameSum += a * b;
    }

    private string GetNumberInRow(int i, int j)
    {
        var a = ScanLeftForNumbStart(i, j, out var x);
        if (a == true)
        {
            return BuildNumberFromStart(i, x) ?? "";
        }
        return "";
    }

    private string BuildNumberFromStart(int i, int j)
    {
        string str = "";
        for (int x = j; x < _board[0].Length; x++)
        {
            if (char.IsNumber(_board[i][x]))
            {
                str += _board[i][x];
            }
            else
            {
                break;
            }
        }
        return str;
    }

    private void RunProcessForFindingParts()
    {
        for (int i = 0; i < _board.Count; i++)
        {
            var validNum = false;
            var currentNumber = "";
            for (int j = 0; j < _board[0].Length; j++)
            {
                if (char.IsNumber(_board[i][j]))
                {
                    if (currentNumber == "")
                    {
                        validNum |= ValidPart(i, j - 1);
                    }

                    currentNumber += _board[i][j];
                    validNum |= ValidPart(i, j);
                }
                if (j + 1 >= _board[0].Length || !char.IsNumber(_board[i][j]))
                {
                    validNum |= ValidPart(i, j);
                    if (validNum && currentNumber != "")
                    {
                        _gameSum += int.Parse(currentNumber);
                    }
                    validNum = false;
                    currentNumber = "";
                }

            }
        }
    }

    private bool ValidPart(int i, int j)
    {
        if (j < 0 || j >= _board[i].Length)
            return false;

        var valid = false;

        if (i - 1 >= 0)
            valid |= !IsDotOrNumber(_board[i - 1][j]);

        if (i + 1 < _board.Count)
            valid |= !IsDotOrNumber(_board[i + 1][j]);

        valid |= !IsDotOrNumber(_board[i][j]);

        return valid;
    }

    private bool? ScanLeftForNumbStart(int i, int j, out int x)
    {
        x = -1;
        if (i < 0 || i >= _board.Count || j < 0) return null;
        if (j >= _board[0].Length) j = _board[0].Length - 1;

        bool? hasNumber = null;
        for (x = j; x >= 0 ; x--)
        {
            if (!char.IsNumber(_board[i][x]))
            {
                x++;
                return hasNumber;
            }
            else
            {
                hasNumber = true;
            }
        }
        x++;
        return hasNumber;
    }

    private bool IsDotOrNumber(char c)
    {
        return c == '.' || char.IsNumber(c);
    }

    public void Run()
    {
        ProcessFile();
        //RunProcessForFindingParts();
        RunProcessForGears();
        Console.WriteLine(_gameSum);
    }
}
