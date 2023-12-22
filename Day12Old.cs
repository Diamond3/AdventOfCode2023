using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AdventOfCode;

public class Day12Old: IRunnable
{

    string fileName = "data/day12_data.txt";
    int lineNumber = 0;
    List<(long, long)> data = new List<(long, long)>();
    List<long> emptyColumns = new List<long>();

    string _originalValue = "0000000";
    List<int> _replacements = new List<int> { 1, 1, 3 };
    ConcurrentDictionary<string, List<string>> _cache = new();

    static long _finalSum = 0;
    static object _sumLock = new object();

    async Task ProcessFile()
    {
        try
        {
            using (var sr = new StreamReader(fileName))
            {
                string line;
                int lineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    var parts = line.Split(' ');
                    var temp = new List<int>();
                    _cache.Clear();
                    _replacements.Clear();
                    _originalValue = "";
                    foreach (var part in parts[1].Split(','))
                    {
                        temp.Add(int.Parse(part));
                    }
                    var tempString = parts[0];

                    for (int i = 0; i < 5; i++)
                    {
                        foreach(var t in temp)
                        {
                            _replacements.Add(t);
                        }
                        _originalValue += tempString + '?';
                    }
                    _originalValue = _originalValue.Remove(_originalValue.Length - 1, 1);

                    var val = _originalValue.ToCharArray();
                    for (int i = 0; i < val.Length; i++)
                    {
                        val[i] = '.';
                    }
                    //this for cycle


                    //await NewMethod(new string(val));

/*                    var patterns = GeneratePatterns(new string(val), 0, 0, new List<string>());
                    var count = ValidPatterns(patterns);
                    _finalSum += count;*/

                    Console.WriteLine(lineNumber);
                    lineNumber++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void NewMethod(string val)
    {
        //var patterns = GeneratePatterns(new string(val), 0, 0, new List<string>());

        var rep = _replacements[0];
        var allPatterns = _replacements.Skip(0).Sum();
        var spacingCount = _replacements.Skip(0).Count() - 1;
        var minPatternLength = allPatterns + spacingCount;

        var tasks = new List<Task>();


        for (int i = 0; i <= _originalValue.Length - minPatternLength; i++)
        {
            int loopIndex = i;
            if (_originalValue[i] == '.') continue;
            tasks.Add(Task.Run(() =>
            {
                var newString = ReplaceAt(val, loopIndex, rep);
                var a = GeneratePatterns(newString, 1, loopIndex + rep + 1, new List<string>());
                lock (_sumLock)
                {
                    _finalSum += a.Count;
                }
            }));
            if (tasks.Count > 100)
            {
                //await Task.WhenAll(tasks.ToArray());
                tasks.Clear();
            }
    }
       //await Task.WhenAll(tasks.ToArray());
    }

    private long ValidPatterns(List<string> patterns)
    {
        var count = 0L;
        foreach (var pattern in patterns)
        {
            var valid = IsValidPattern(pattern);
            if (valid)
            {
                //Console.WriteLine(pattern);
                count++;
            }
        }
        return count;
    }

    private bool IsValidPattern(string pattern, int? n = null)
    {
        if (n != null)
        {
            for (int i = 0; i < n; i++)
            {
                if (pattern[i] == _originalValue[i]
                    || (pattern[i] == '#' && _originalValue[i] == '?')
                    || (pattern[i] == '.' && _originalValue[i] == '?'))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        var l = _originalValue.Length;
        for (int i = 0; i < pattern.Length; i++)
        {
            var a = pattern[i];
            var b = _originalValue[i];
            if ((pattern[i] == '.' && _originalValue[i] == '.')
                || (pattern[i] == '#' && _originalValue[i] == '#')
                || (pattern[i] == '#' && _originalValue[i] == '?')
                || (pattern[i] == '.' && _originalValue[i] == '?'))
            {
                a = b;
                continue;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public void Run()
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        ProcessFile();
        Console.WriteLine(stopwatch.ElapsedMilliseconds / 100);
        Console.WriteLine(_finalSum);
        /*var l = GeneratePatterns(_originalValue;, 0, 0, new List<string>());
        for (int i = 0; i < l.Count; i++)
        {
            Console.WriteLine(l[i]);
        }*/
        //FindCombinations(_testValue, _values);
        /*var distances = new List<int>();
        long sum = 0;
        var count = 0;

        for (int i = 0; i < data.Count; i++)
        {
            var lowest = int.MaxValue;
            var indx = i;
            for (int j = i + 1; j < data.Count; j++)
            {
                var distance = CalculateDistaces(data[i], data[j]);
                sum += distance;
                count++;
            }
        }
        Console.WriteLine(sum);
        Console.WriteLine(count);*/
    }

    private List<string> GeneratePatterns(string currentString, int replacementIndex, int position, List<string> patterns)
    {
        if (position >= currentString.Length || replacementIndex >= _replacements.Count)
        {
            if (!IsValidPattern(currentString)) return patterns;
            patterns.Add(currentString);
            return patterns;
        }
        if (!IsValidPattern(currentString, position)) 
            return patterns;
        var rep = _replacements[replacementIndex];
        var allPatterns = _replacements.Skip(replacementIndex).Sum();
        var spacingCount = _replacements.Skip(replacementIndex).Count() - 1;
        var minPatternLength = allPatterns + spacingCount;

        for (int i = position; i <= currentString.Length - minPatternLength; i++)
        {
            //if (currentString[i] == '.') continue;
            var newString = ReplaceAt(currentString, i, rep);
            //var key = newString

            if (_cache.TryGetValue(newString, out var cachedPatterns))
            {
                patterns.AddRange(cachedPatterns);
                return patterns;
            }
            else
            {
                var newPatterns = GeneratePatterns(newString, replacementIndex + 1, i + rep + 1, patterns);
                _cache.TryAdd(newString, patterns);
            }

            //patterns.AddRange(newPatterns);
/*            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }
            else
            {
                GeneratePatterns(newString, replacementIndex + 1, i + rep + 1, patterns);
                //_cache.Add(key, patterns);
            }*/
        }
        return patterns;
    }

    private string ReplaceAt(string currentString, int i, int v)
    {
        var ch = currentString.ToCharArray();
        var j = 0;
        for (j = i; j < i + v; j++)
        {
            ch[j] = '#';
        }
        if (j != ch.Length)
        {
            ch[j] = '.';
        }
        return new string(ch);
    }

    /*private string FindCombinations(string comb, List<int> counts)
    {
        if (counts.Count == 0)
        {
            return comb;
        }
        for (int i = 0; i < comb.Length; i++)
        {
            if (comb[i] == '#' || comb[i] == '?')
            {
                var newString = AddIfPossible(i, comb, counts.First());
                if (newString == null)
                {
                    continue;
                }
                else
                {
                    
                    Console.WriteLine(comb.Substring(0, i) + FindCombinations(newString, counts));
                    //FindCombinations(newString, counts.Skip(1).ToList());
                }
            }
        }
    }*/

    private string? AddIfPossible(int i, string comb, int v)
    {
        var str = comb.Substring(i).ToCharArray();
        var count = 0;
        var indx = -1;
        var addedAdditional = false;

        for (int k = 0; k < v + 1 && k < str.Length; k++)
        {
            if (k == v)
            {
                str[k] = '.';
                addedAdditional = true;
                break;
            }
            count++;
            str[k] = '#';
        }

        if (count != v)
        {
            return null;
        }

        return new string(str).Substring(0, v + (addedAdditional ? 1 : 0));
    }

   /* private string? AddIfPossible(int i, string comb, int v)
    {
        var str = comb.Substring(i).ToCharArray();
        var count = 0;
        var indx = -1;
        for (int j = 0; j < str.Length; j++)
        {
            if (str[j] == '#' || str[j] == '?')
            {
                count++;
                if (indx == -1)
                {
                    indx = j;
                }
            }
            else
            {
                return null;
            }

            if (count == v && j < str.Length && str[j] == '#')
            {
                return null;
            }

            if (count == v)
            {
                break;
            }
        }

        if (count != v)
        {
            return null;
        }

        for (int k = indx; k < v; k++)
        {
            str[k] = '#';
        }
        return new string(str);
    }*/

    private int AbleToFit(int i, string comb, int v)
    {
        var str = comb.Substring(i);
        var count = 0;
        var indx = -1;
        for (int j = 0; j < str.Length; j++)
        {
            if (str[j] == '#' || str[j] == '?')
            {
                count++;
                if (indx == -1)
                {
                    indx = j;
                }
            }
            else
            {
                count = 0;
                indx = -1;
            }

            if (count == v)
            {
                return indx;
            }
        }
        return -1;
    }
}