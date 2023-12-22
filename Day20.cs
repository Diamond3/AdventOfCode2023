using System.Data;
using System.Xml.Linq;

namespace AdventOfCode;

public class Day20 : IRunnable
{
    string fileName = "data/day20_data.txt";

    long _finalSum = 0;

    Dictionary<string, List<string>> _flows = new();

    List<string> _possiblePaths = new();

    Dictionary<string, IModule> _allModules = [];
    Dictionary<string, Conjunction> _conjunctions = [];
    Broadcaster _broadcaster;

    public static HashSet<string> ImportantModuleNames = [];
    public static Dictionary<string, List<int>> ImportantModuleStates = [];
    public static HashSet<string> ImportantConjunctions = new HashSet<string>()
    {
        //vt, dq, qt, nl
        "vt",
        "dq",
        "qt",
        "nl"
    };

    public static int CurrentIteration = 0;

    long _lowPulses = 0;
    long _highPulses = 0;

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
                    var parts = line.Split("->", StringSplitOptions.TrimEntries);
                    var label = parts[0];
                    var outputModules = parts[1].Split(", ");

                    if (label[0] != '%' && label[0] != '&')
                    {
                        _broadcaster = new Broadcaster()
                        {
                            OutputModules = outputModules.ToList()
                        };
                    }
                    else if (label[0] == '%')
                    {
                        var f = new FlipFlop()
                        {
                            Name = label[1..],
                            OutputModules = outputModules.ToList()
                        };
                        _allModules[label[1..]] = f;
                    }
                    else
                    {
                        var f = new Conjunction()
                        {
                            Name = label[1..],
                            OutputModules = outputModules.ToList()
                        };
                        _allModules[label[1..]] = f;
                        _conjunctions[label[1..]] = f;
                    }

                    // to output graph -> mermaid.live
                    /*foreach (var f in outputModules)
                    {
                        Console.WriteLine($"{label[1..]} --> {f}");
                    }*/

                    /*  For rx to get LOW signal, bn needs to send LOW signal
                        For bn to send LOW signal all (pl, mz, lz and zm) should be HIGH signals
                        For (pl, mz, lz and zm) to be HIGH all (vt, dq, qt, nl) should be LOW <- only need to track these!!!
                        For (vt, dq, qt, nl) to be LOW signal, following rows should be fully HIGH
                            vt-> nb hf hn ch kd cb hh kr
                            dq-> kx lv gx mh xd mt ts
                            qt-> cf vr sl jv sm hl hp ng
                            nl-> gq bv gj bh qf sk rb rd zb jp
                    */
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void SetInputsForConjunctions()
    {
        foreach (var con in _conjunctions.Keys)
        {
            //Console.Write(con + " ->");
            foreach (var key in _allModules.Keys)
            {
                if (_allModules[key] is FlipFlop f)
                {
                    if (f.OutputModules.Contains(con))
                    {
                        _conjunctions[con].InputModules.Add(key);

                        Day20.ImportantModuleNames.Add(key);  //adding all modules that needs to be tracked
                        //Console.Write(" " + key);
                    }
                }
            }

            //Console.WriteLine();
            _conjunctions[con].ClearMemory();
        }
    }

    public void Run()
    {
        ProcessFile();
        SetInputsForConjunctions();
        for (CurrentIteration = 0; CurrentIteration < 100000; CurrentIteration++)
        {
            SendPulseToBroadcaster(0);
            /*if (i % 10000 == 0)
                Console.WriteLine(i);*/
        }

        /*foreach (var con in ImportantModuleNames)
        {
            Console.Write($"{con} ->");
            if (ImportantModuleStates.ContainsKey(con))
            {
                foreach (var n in ImportantModuleStates[con])
                {
                    Console.Write($" {n}");
                }
            }
            Console.WriteLine();
        }*/
        /*Console.WriteLine();
        foreach (var con in ImportantConjunctions)
        {
            Console.Write($"{con} ->");
            if (ImportantModuleStates.ContainsKey(con))
            {
                foreach (var n in ImportantModuleStates[con])
                {
                    Console.Write($" {n}");
                }
            }
            Console.WriteLine();
        }*/

        //Repeats after
        //4003
        //3881
        //3797
        //3823


        var steps = new List<int>();
        foreach (var con in ImportantConjunctions)
        {
            var a = ImportantModuleStates[con];
            for (int i = 1; i < a.Count - 1; i++)
            {
                Console.Write($"{con} --> {a[i] - a[i - 1]} ");

                steps.Add(a[i] - a[i - 1]);
                break;
            }
            Console.WriteLine();
        }

        var ans = 1L;
        for (int i = 0; i < steps.Count; i++)
        {
            ans = BiggestMultiple(steps[i], ans);
        }

        Console.WriteLine($"Common multiple: {ans}");
        Console.WriteLine(_lowPulses * _highPulses);
    }

    private long BiggestMultiple(long a, long b)
    {
        var lowest = LowestMultiple(a, b);
        return (a * b / lowest);
    }

    private long LowestMultiple(long a, long b)
    {
        if (a < b)
        {
            var temp = b;
            b = a;
            a = temp;
        }

        while (b != 0)
        {
            long r = a % b;
            a = b;
            b = r;
        }
        return a;
    }

    private void SendPulseToBroadcaster(int buttonPulse)
    {
        var hashSet = new HashSet<string>();
        var nextQueue = new Queue<string>();

        //Console.WriteLine($"button {PulseToString(buttonPulse)}-> broadcaster");
        CalculatePulse(buttonPulse);

        foreach (var m in _broadcaster.OutputModules)
        {
            //Console.WriteLine($"broadcaster {PulseToString(buttonPulse)}-> {m}");
            _allModules[m].SetInputPulse("broadcaster", buttonPulse);

            CalculatePulse(buttonPulse);

            nextQueue.Enqueue(m);

        }

        while (nextQueue.Count > 0)
        {
            hashSet.Clear();
            var queue = nextQueue;
            //var queue = ProcesAllQueueModules(nextQueue);

            while (queue.Count > 0)
            {
                var label = queue.Dequeue();

                var pulse = _allModules[label].ProcessPulse();

                if (pulse == -1)
                {
                    continue;
                }

                foreach (var m in _allModules[label].GetOutputModules())
                {
                    if (m == "bn")
                    {
                        var a = "v";
                    }
                    //Console.WriteLine($"{label} {PulseToString(pulse)}-> {(_conjunctions.ContainsKey(m) ? $"& {_conjunctions[m].GetCurrentPulse()}" : "")}{m}");

                    CalculatePulse(pulse);

                    if (!_allModules.ContainsKey(m)) 
                        continue;

                    if (m == "rx" && pulse == 0)
                    {
                        var a = 0;
                    }

                    _allModules[m].SetInputPulse(label, pulse);

                    if (!hashSet.Contains(m))
                    {
                        nextQueue.Enqueue(m);
                        //hashSet.Add(m);
                    }
                }
            }
        }
    }

    private void CalculatePulse(int buttonPulse)
    {
        if (buttonPulse == 0)
        {
            _lowPulses++;
        }
        else if (buttonPulse == 1)
        {
            _highPulses++;
        }
    }

    private Queue<string> ProcesAllQueueModules(Queue<string> queue)
    {
        var tempQueue = new Queue<string>();
        while (queue.Count > 0)
        {
            var l = queue.Dequeue();
            _allModules[l].ProcessPulse();

            tempQueue.Enqueue(l);
        }
        return tempQueue;
    }

    private string PulseToString(int pulse)
    {
        if (pulse < 0)
        {
            return "----";
        }
        if (pulse == 0)
        {
            return "-low";
        }
        return "-high";
    }
}

public interface IModule
{
    int ProcessPulse();
    void SetInputPulse(string mod, int pulse);
    int GetCurrentPulse();
    List<string> GetOutputModules();

    string GetModuleName();
}

public class FlipFlop : IModule
{
    public List<string> OutputModules = [];
    public List<string> InputModules = [];

    public Queue<int> PulseQueue = [];
    private int _lastIteration = 0;

    public string Name;

    bool _isOn = false;
    int _pulse = -1; //-1 - no pulse, 0 - low pulse, 1 - high pulse

    public int GetCurrentPulse()
    {
        return _pulse;
    }

    public int ProcessPulse()
    {
        var currentPulse = PulseQueue.Dequeue();
        if (currentPulse == 1)
        {
            _pulse = -1;
        }
        else if (currentPulse == 0 && !_isOn)
        {
            _isOn = true;
            _pulse = 1;
        }
        else if (currentPulse == 0 && _isOn)
        {
            _isOn = false;
            _pulse = 0;
        }

        /*if (_pulse == 1 && Day20.ImportantModuleNames.Contains(Name))
        {
            if (!Day20.ImportantModuleStates.ContainsKey(Name))
            {
                Day20.ImportantModuleStates[Name] = [];
            }
            var current = Day20.CurrentIteration - _lastIteration;
            Day20.ImportantModuleStates[Name].Add(current);
            _lastIteration = current;
        }*/

        return _pulse;
    }

    public List<string> GetOutputModules()
    {
        return OutputModules;
    }

    public void SetInputPulse(string mod, int pulse)
    {
        /*_currenPulse = pulse;
        _currentMod = mod;*/

        PulseQueue.Enqueue(pulse);
        //ProcessPulse();
    }

    public string GetModuleName()
    {
        return Name;
    }
}

public class Conjunction : IModule
{
    public List<string> OutputModules = [];
    public List<string> InputModules = [];
    public Dictionary<string, int> MemoryBlock = [];

    public Queue<(string, int)> PulseQueue = [];

    public string Name;

    int _pulse = 0; //-1 - no pulse, 0 - low pulse, 1 - high pulse

    int _currenPulse;
    private int _lastIteration = 0;

    public int GetCurrentPulse()
    {
        return _pulse;
    }

    public int ProcessPulse()
    {
        (string mod, int pulse) = PulseQueue.Dequeue();
        MemoryBlock[mod] = pulse;

        _pulse = MemoryBlock.Values.All(x => x == 1) ? 0 : 1;

        if (_pulse == 0 && Day20.ImportantConjunctions.Contains(Name))
        {
            if (!Day20.ImportantModuleStates.ContainsKey(Name))
            {
                Day20.ImportantModuleStates[Name] = [];
            }
            var current = Day20.CurrentIteration - _lastIteration;
            Day20.ImportantModuleStates[Name].Add(Day20.CurrentIteration);
            _lastIteration = current;
        }

        return _pulse;
    }

    public void ClearMemory()
    {
        foreach (var module in InputModules)
        {
            MemoryBlock[module] = 0;
        }
    }

    public List<string> GetOutputModules()
    {
        return OutputModules;
    }

    public void SetInputPulse(string mod, int pulse)
    {
        PulseQueue.Enqueue((mod, pulse));
        //MemoryBlock[mod] = pulse;
    }

    public string GetModuleName()
    {
        return Name;
    }
}

public class Broadcaster
{
    public List<string> OutputModules = [];
}