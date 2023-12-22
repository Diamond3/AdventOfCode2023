using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day17Old: IRunnable
{

    string fileName = "data/day17_data.txt";
    private int _finalSum = 0;

    private List<short[]> _map = [];
    private HashSet<(int, int)> _energizedTiles = [];
    private HashSet<(int, int, int)> _cache = [];

    private List<int[]> _evaluations = [];

    private List<List<(int, int)>> _paths;

    private string _currentString = "HASH";
    private long _currentValue = 0;

    private int _minSum = int.MaxValue;

    void ProcessFile()
    {
        try
        {
            var regex = new Regex(@"^([a-zA-Z]+)([-=])(\d*)$");

            using (var sr = new StreamReader(fileName))
            {
                string line;
                int lineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    var ints = line.Select(c => (short)(c - '0')).ToArray();
                    _map.Add(ints);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    // 0 - up
    // 1 - right
    // 2 - down
    // 3 - left
    private Dictionary<(int, int), int> EvaluateDistances()
    {
        var evaluations = new Dictionary<(int, int), int>();

        for (int i = 0; i < _map.Count; i++)
        {
            var arr = new int[_map[0].Length];
            for (int j = 0; j < _map[0].Length; j++)
            {
                var x = Math.Abs(i - _map.Count - 1);
                var y = Math.Abs(j - _map[0].Length - 1);
                arr[j] = x + y;
                evaluations.Add((j, i), x + y);
            }
            //evaluations.Add(arr);
        }
        return evaluations;
    }

    private Dictionary<(int, int), int> SetToInfinity()
    {
        var evaluations = new Dictionary<(int, int), int>();

        for (int i = 0; i < _map.Count; i++)
        {
            for (int j = 0; j < _map[0].Length; j++)
            {
                evaluations.Add((j, i), int.MaxValue);
            }
            //evaluations.Add(arr);
        }
        return evaluations;
    }

    private HashSet<(int, int)> AStar()
    {
        //visited.Add((x, y), _evaluations[y][x]);

        var directions = new Dictionary<(int, int), Dictionary<int, int>>();

        var visited = new Dictionary<(int, int), int>();

        var cameFrom = new Dictionary<(int, int), (int, int)>();
        var openSet = new List<(int, int)>();

        var h = EvaluateDistances();

        var cellCost = SetCellCosts(_map);

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known. <- node scores
        var gScore = SetToInfinity();
        gScore[(0, 0)] = 0;

        // For node n, fScore[n] := gScore[n] + h(n).fScore[n] represents our current best guess as to
        // how cheap a path could be from start to finish if it goes through n. <- path to node scores
        var fScore = SetToInfinity();

        fScore[(0, 0)] = h[(0, 0)];

        openSet.Add((0, 0));


        var goal = (_map[0].Length - 1, _map.Count - 1);

        while (openSet.Count != 0)
        {
            var current = LowestFromOpenSet(openSet, fScore);

            if (current == goal)
            {
                //we have reached goal
                //reconstruct path
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);

            var nextNeighbors = FindPossibleCells(current.Item1, current.Item2);
            //var nextNeighbors = FindPossibleCellsWithDirections(current.Item1, current.Item2, );

            foreach (var next in nextNeighbors)
            {
                (int a, int b) = next;

                var neighbor = (a, b);

                var visitedNodeGScore = gScore[current] + 1 + cellCost[neighbor]; //instead of 1 it should be distance d(current, neighbor);

                if (visitedNodeGScore < gScore[neighbor])
                {

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = visitedNodeGScore;
                    fScore[neighbor] = visitedNodeGScore + h[neighbor];

                    //directions.

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return [];
    }

    private Dictionary<(int, int), int> SetCellCosts(List<short[]> map)
    {
        var dic = new Dictionary<(int, int), int>();
        for (int i = 0; i < map.Count; i++)
        {
            for (int j = 0; j < map[0].Length; j++)
            {
                dic.Add((j, i), map[i][j]);
            }
        }
        return dic;
    }

    private HashSet<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)> cameFrom, (int, int) current)
    {
        var path = new HashSet<(int, int)>
        {
            current
        };

        while (current != (0, 0)) // not a start node
        {
            current = cameFrom[current];
            path.Add(current);
        }

        return path;
    }

    private (int, int) LowestFromOpenSet(List<(int, int)> openSet, Dictionary<(int, int), int> fScore)
    {
        var lowest = int.MaxValue;
        var current = (0, 0);
        for (int i = 0; i < openSet.Count; i++)
        {
            (int x, int y) = openSet[i];

            if (fScore[(x, y)] < lowest)
            {
                lowest = fScore[(x, y)];
                current = openSet[i];
            }
        }
        return current;
    }

    private List<(int, int, int)> FindPossibleCellsWithDirections(int x, int y, int dir,int length)
    {
        var list = new List<(int, int, int)>();

        if (y - 1 >= 0) // 0
        {
            list.Add((x, y - 1, 0));
        }
        if (y + 1 < _map.Count) // 2
        {
            list.Add((x, y + 1, 2));
        }
        if (x + 1 < _map[0].Length) // 1
        {
            list.Add((x + 1, y, 1));
        }
        if (x - 1 >= 0) // 3
        {
            list.Add((x - 1, y, 3));
        }
        return list;
    }

    private List<(int, int)> FindPossibleCells(int x, int y)
    {
        var list = new List<(int, int)>();

        if (y - 1 >= 0)
        {
            list.Add((x, y - 1));
        }
        if (y + 1 < _map.Count)
        {
            list.Add((x, y + 1));
        }
        if (x + 1 < _map[0].Length)
        {
            list.Add((x + 1, y));
        }
        if (x - 1 >= 0)
        {
            list.Add((x - 1, y));
        }
        return list;
    }

    private void Walk(int x, int y, int currentSum, List<short[]> currentMap)
    {
        if (!IsValidCell(x, y, currentMap) || currentSum >= _minSum) return;

        currentSum += _map[y][x];
        currentMap[y][x] = 0;

        if (x == _map[0].Length - 1 && y == _map.Count - 1)
        {
            PrintMap(currentMap);
            Console.WriteLine(currentSum.ToString());
            if (currentSum < _minSum)
            {
                _minSum = currentSum;
            }
        }

        if (y - 1 >= 0)
        {
            Walk(x, y - 1, currentSum, DeepCopyList(currentMap));
        }
        if (y + 1 < _map.Count)
        {
            Walk(x, y + 1, currentSum, DeepCopyList(currentMap));
        }
        if (x + 1 < _map[0].Length)
        {
            Walk(x + 1, y, currentSum, DeepCopyList(currentMap));
        }
        if (x - 1 >= 0)
        {
            Walk(x - 1, y, currentSum, DeepCopyList(currentMap));
        }
    }

    List<short[]> DeepCopyList(List<short[]> originalList)
    {
        var newList = new List<short[]>(originalList.Count);
        foreach (short[] originalArray in originalList)
        {
            var newArray = new short[originalArray.Length];
            Array.Copy(originalArray, newArray, originalArray.Length);
            newList.Add(newArray);
        }
        return newList;
    }

    private bool IsValidCell(int x, int y, List<short[]> currentMap)
    {
        if (currentMap[y][x] == 0)
        {
            return false;
        }
        return true;
    }

    private void PrintMap(List<short[]> currentMap)
    {
        for (int i = 0; i < currentMap.Count; i++)
        {
            var str = "";
            for (int j = 0; j < currentMap[0].Length; j++)
            {
                str += (currentMap[i][j]).ToString();
            }
            Console.WriteLine(str);
        }
        Console.WriteLine();
    }

    private void PrintMap(List<int[]> currentMap)
    {
        for (int i = 0; i < currentMap.Count; i++)
        {
            var str = "";
            for (int j = 0; j < currentMap[0].Length; j++)
            {
                str += (currentMap[i][j]).ToString() + " ";
            }
            Console.WriteLine(str);
        }
        Console.WriteLine();
    }

    public void Run()
    {
        ProcessFile();
        var path = AStar();
        PrintPath(path);
        //Walk(0, 0, 0, _map);
        //EvaluateDistances();
        //PrintMap(_evaluations);
        
        Console.WriteLine(_minSum);
    }

    private void PrintPath(HashSet<(int, int)> path)
    {
        for(int i = 0;i < _map.Count; i++)
        {
            var str = "";
            for (int j = 0; j < _map[0].Length; j++)
            {
                if (path.Contains((j, i)))
                {
                    str += '#';
                }
                else
                {
                    str += '.';
                }
            }
            Console.WriteLine(str);
        }
        Console.WriteLine();
    }
}