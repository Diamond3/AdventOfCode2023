namespace AdventOfCode;
public class Day7: IRunnable
{
    const string FilePath = "data/day7_data.txt";

    private List<long> _times = new();
    private List<long> _dists = new();
    private long[] _best;

    private List<string> _combinations = new();
    private List<int> _bets = new();
    private int[] _places;
    private int[] _values; 
    //AQ234 -> 1
    //AQA34 -> 2
    //QQQAJ -> 3
    //QAQAA -> 3.5
    //AQQQQ -> 4
    //AAAAA -> 5

    private Dictionary<char, ushort> _cardToNum = new Dictionary<char, ushort>()
    {
        { 'A', 14 },
        { 'T', 10 },
        { 'J', 1 },
        { 'Q', 12 },
        { 'K', 13 }
    };

    private void ProcessFile()
    {
        try
        {
            using var sr = new StreamReader(FilePath);
            string line;
            int lineNumber = 0;

            while ((line = sr.ReadLine()) != null)
            {
                var parts = line.Split(" ");

                _combinations.Add(parts[0]);
                _bets.Add(int.Parse(parts[1]));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    private void EvaluateCombinations()
    {
        for (int i = 0; i < _combinations.Count; i++)
        {
            var combination = _combinations[i];
            var values = new ushort[15];
            foreach (var c in _combinations[i])
            {
                var n = char.IsNumber(c) ? c - 48 : _cardToNum[c];
                values[n - 1]++;
            }



            var max = (ushort)values.Aggregate(0, (sum, c) => sum + c * c);

            for (int j = 1; j < values.Length; j++)
            {
                if (values[j] != 0)
                {
                    var temp = values[j];
                    var tempJ = values[0];

                    values[j] += values[0];
                    values[0] = 0;

                    var tempMax = (ushort)values.Aggregate(0, (sum, c) => sum + c * c);

                    if (tempMax > max)
                    {
                        max = tempMax;
                    }

                    values[j] = temp;
                    values[0] = tempJ;
                }
            }

            _values[i] = max;
        }
    }

    public void Run()
    {
        ProcessFile();

        _places = new int[_bets.Count];
        for (int i = 0; i < _places.Length; i++)
        {
            _places[i] = i;
        }

        _values = new int[_bets.Count];
        EvaluateCombinations();
        Sort();
        var acc = _places.Select((val, indx) => _bets[_places[indx]] * (indx + 1)).Sum();

        for (int i = 0; i < _places.Length - 1; i++)
        {
            if (_values[_places[i]] > _values[_places[i + 1]] || (_values[_places[i]] == _values[_places[i + 1]] && FirstIsHigher(_places[i], _places[i + 1]))) 
            {
                Console.WriteLine("Failed to sort!");
                Console.WriteLine(_combinations[_places[i]]);
            }
        }

        foreach (var v in _places)
        {
            Console.WriteLine(_values[v] + " " + _combinations[v]);
        }

        Console.WriteLine(acc);
    }

    private void Sort()
    {
        for (int i = 0; i < _places.Length - 1; i++)
        {
            for (int j = 0; j < _places.Length - i - 1; j++)
            {
                if (_values[_places[j]] > _values[_places[j + 1]] 
                    || (_values[_places[j]] == _values[_places[j + 1]] 
                        && FirstIsHigher(_places[j], _places[j + 1])))
                {
                    var temp = _places[j];
                    _places[j] = _places[j + 1];
                    _places[j + 1] = temp;
                }
            }
        }
    }

    private bool FirstIsHigher(int i, int j)
    {
        for (int k = 0; k < 5; k++)
        {
            var aTemp = _combinations[i][k];

            var a = char.IsNumber(aTemp) ? aTemp - 48 : _cardToNum[aTemp];

            aTemp = _combinations[j][k];

            var b = char.IsNumber(aTemp) ? aTemp - 48 : _cardToNum[aTemp];

            if (a > b)
            {
                return true;
            }
            else if (a != b)
            {
                return false;
            }
        }
        return false;
    }
}
