using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace AdventOfCode;

public class Day12: IRunnable
{

    string fileName = "data/day12_data.txt";
    private long _finalSum = 0;
    private string _currentString;

    private Dictionary<(string, int, int), long> _cache = new();
    private int _partsSum = 0;
    private int _dotsSum = 0;

    private List<int> _allNums = new List<int>();

    void ProcessFile()
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
                    var springs = parts[0];
                    _partsSum = 0;
                    _cache.Clear();
                    _currentString = "";

                    //Console.WriteLine(springs);
                    var tempNums = parts[1].Split(',').Select(int.Parse).ToList();
                    _allNums.Clear();

                    var nums = new List<int>();

                    _currentString = "";
                    //nums = tempNums;

                    for (int i = 0; i < 5; i++)
                    {
                        nums.AddRange(tempNums.ToList());
                        _allNums.AddRange(tempNums.ToList());
                        _currentString += springs;
                        if (i != 4)
                        {
                            _currentString += '?';
                        }
                    }
                    _partsSum = nums.Sum();
                    _dotsSum = nums.Count - 1;

                    var c = FindVariation(_currentString.ToCharArray(), 0, nums);

                    Console.WriteLine(lineNumber);
                    _finalSum += c;
                    lineNumber++;
                    //var c = ProcessSpring(springs, nums);
                    //_finalSum += c;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private long FindVariation(char[] str, int index, List<int> nums)
    {
        var count = 0L;
        if (nums.Count == 0 || index >= str.Length) 
        {
            if (AbleToPlace(str) && FitsWithNums(str, _allNums))
            {
                //Console.WriteLine(str);
                return 1;
            }
            return 0;
        }

        var key = (new string(str[index..]), index, nums.Count());
        if (_cache.ContainsKey(key))
        {
            return _cache[key];
        }


        if (str[index] == '#')
        {
            var c1 = (char[])str.Clone();

            var added = TryToAddPattern(c1, index, nums[0]);

            //c1[index] = '#';

            if (added != -1)
            {
                count += FindVariation(c1, added + 1, nums.Skip(1).ToList());
            }
        }
        else if (str[index] == '?')
        {
            var c1 = (char[])str.Clone();

            var added = TryToAddPattern(c1, index, nums[0]);

            //c1[index] = '#';

            if (added != -1) 
            {
                count += FindVariation(c1, added + 1, nums.Skip(1).ToList());
            }
            

            var c2 = (char[])str.Clone();
            c2[index] = '.';


            count += FindVariation(c2, index + 1, nums);
        }
        else
        {
            count += FindVariation(str, index + 1, nums);
        }

        if (!_cache.ContainsKey(key))
        {
            _cache.Add(key, count);
        }
        return count;
    }

    private int TryToAddPattern(char[] c1, int index, int v)
    {
        var c = 0;
        for (int i = index; i < c1.Length; i++)
        {
            if (c1[i] == '#' || c1[i] == '?')
            {
                c1[i] = '#';
                c++;
                if (c == v)
                {
                    if (i + 1 == c1.Length)
                    {
                        return c1.Length -  1;
                    }
                    else
                    {
                        if (c1[i + 1] == '.')
                        {
                            return i + 1;
                        }
                        else if (c1[i + 1] == '?')
                        {
                            i++;
                            c1[i] = '.';
                            return i;
                        }
                        return -1;
                    }
                }
            }
            else
            {
                return -1;
            }
        }
        return -1;
    }

    private bool AbleToFit(char[] c1, List<int> nums)
    {
        var a = c1.Count(x  => x == '.');
        if (_dotsSum <= a && _partsSum <= c1.Length - a) return true;
        return false;
    }

    private bool FitsWithNums(char[] str, List<int> nums)
    {
        var c = 0;
        var indx = 0;

        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '#')
            {
                c++;
                if (indx < nums.Count && c == nums[indx])
                {
                    if (i + 1 < str.Length && str[i + 1] != '.')
                    {
                        return false;
                    }
                    indx++;
                    c = 0;
                }
            }
            else if (c != 0)
            {
                return false;
            }
        }

        return c == 0 && indx >= nums.Count;
    }

    private bool AbleToPlace(char[] str)
    {
        var isValid = true;
        for (int i = 0; i < str.Length; i++)
        {
            if (_currentString[i] != '?' && str[i] != _currentString[i])
            {
                return false;
            }
        }
        return isValid;
    }

    public void Run()
    {
        ProcessFile();
        //FindVariation("???".ToCharArray(), 0);
        Console.WriteLine(_finalSum);
    }
}