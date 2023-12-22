using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public struct Node
{
    public (int, int) Coord { get; set; }
    public int Direction { get; set; }
    public int Distance { get; set; }
}


public class Day17: IRunnable
{

    string fileName = "data/day17_data.txt";

    private List<short[]> _map = [];
    private HashSet<(int, int)> _energizedTiles = [];
    private HashSet<(int, int, int)> _cache = [];

    private List<int[]> _evaluations = [];

    private List<List<(int, int)>> _paths;

    private string _currentString = "HASH";
    private long _currentValue = 0;

    private int _finalSum = 0;

    private int _maxOneDirectionDistance = 10;
    private int _minDistanceToStop = 4; //4;

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
    private Dictionary<(int, int), float> EvaluateDistances()
    {
        var evaluations = new Dictionary<(int, int), float>();

        for (int i = 0; i < _map.Count; i++)
        {
            for (int j = 0; j < _map[0].Length; j++)
            {
                var x = Math.Abs(i - _map.Count - 1);
                var y = Math.Abs(j - _map[0].Length - 1);

                var fX = x * x;
                var fY = y * y;

                var sq = (float)Math.Sqrt(fX + fY);
                
                evaluations.Add((j, i), sq);
            }
            //evaluations.Add(arr);
        }
        return evaluations;
    }

    private float GetDistance(int x, int y)
    {
        var yDiff = Math.Abs(y - _map.Count - 1);
        var xDiff = Math.Abs(x - _map[0].Length - 1);

        var fX = xDiff * xDiff;
        var fY = yDiff * yDiff;

        return(float)Math.Sqrt(fX + fY);
    }

    private Dictionary<Node, float> SetToInfinity()
    {
        var evaluations = new Dictionary<Node, float>();

        for (int i = 0; i < _map.Count; i++)
        {
            for (int j = 0; j < _map[0].Length; j++)
            {
                var node = new Node()
                {
                    Coord = (j, i),
                    Direction = 0,
                    Distance = 1
                };

                for (int k = 0; k < 4; k++) // all directions
                {
                    node.Direction = k;
                    AddAllPossibleDistances(evaluations, node, float.MaxValue);
                }
            }
        }
        return evaluations;
    }

    private void AddAllPossibleDistances(Dictionary<Node, float> evaluations, Node node, float maxValue)
    {
        for (int i = 1; i <= _maxOneDirectionDistance; i++)
        {
            node.Distance = i;
            evaluations.Add(node, maxValue);
        }
    }

    private HashSet<Node> AStar()
    {
        //visited.Add((x, y), _evaluations[y][x]);

        //var directions = new Dictionary<(int, int), Dictionary<int, int>>();

        var priorityQueue = new PriorityQueue<Node, float>();

        var visited = new Dictionary<Node, int>();

        var cameFrom = new Dictionary<Node, Node>();
        var openSet = new List<Node>();

        var h = EvaluateDistances();

        var cellCost = SetCellCosts(_map);

        var s1 = new Node()
        {
            Coord = (0, 0),
            Direction = 1,
            Distance = 1
        };

        var s2 = new Node()
        {
            Coord = (0, 0),
            Direction = 2,
            Distance = 0
        };

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known. <- node scores
        var gScore = new Dictionary<Node, float>(); //SetToInfinity();
        gScore[s1] = 0;
        gScore[s2] = 0;

        // For node n, fScore[n] := gScore[n] + h(n).fScore[n] represents our current best guess as to
        // how cheap a path could be from start to finish if it goes through n. <- path to node scores
        var fScore = new Dictionary<Node, float>(); //SetToInfinity();

        fScore[s1] = h[(0, 0)]; // adjust heuristics to make it more efficient
        fScore[s2] = h[(0, 0)];

        priorityQueue.Enqueue(s1, 0);
        priorityQueue.Enqueue(s2, 0);

        var goalCoord = (_map[0].Length - 1, _map.Count - 1);

        var st = new Stopwatch();
        st.Start();

        var processedNodes = new Dictionary<Node, float>();

        while (priorityQueue.Count != 0)
        {
            //var current = LowestFromOpenSet(openSet, fScore);
            var current = priorityQueue.Dequeue();

            /*if (processedNodes.ContainsKey(current) && fScore[current] > processedNodes[current])
            {
                continue;
            }*/
            if (!fScore.ContainsKey(current))
            {
                fScore.Add(current, float.MaxValue);
                gScore.Add(current, float.MaxValue);
            }

            if (current.Coord == goalCoord && current.Distance >= _minDistanceToStop) //what about directions distance?
            {
                //we have reached goal
                //reconstruct path
                Console.WriteLine(st.ElapsedMilliseconds.ToString());
                return ReconstructPath(cameFrom, current, cellCost);
            }

            //openSet.Remove(current);

            //var nextNeighbors = FindPossibleCells(current.Item1, current.Item2);
            var nextNeighbors = FindPossibleCellsWithDirections(current);

            foreach (var neighbor in nextNeighbors)
            {
                var visitedNodeGScore = gScore[current] + 0 + cellCost[neighbor.Coord]; //instead of 1 it should be distance d(current, neighbor); FOR THIS IT SHOULD BE 0

                if (!fScore.ContainsKey(neighbor))
                {
                    fScore.Add(neighbor, float.MaxValue);
                    gScore.Add(neighbor, float.MaxValue);
                }

                if (visitedNodeGScore < gScore[neighbor])
                {

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = visitedNodeGScore;
                    fScore[neighbor] = visitedNodeGScore + h[neighbor.Coord];

                    priorityQueue.Enqueue(neighbor, fScore[neighbor]);
/*                    if (!processedNodes.ContainsKey(neighbor))
                    {
                        processedNodes.Add(neighbor, fScore[neighbor]);
                    }*/
                }
            }
        }

        return [];
    }

    private List<Node> FindPossibleCellsWithDirections(Node current)
    {
        var list = new List<Node>();

        (int x, int y) = current.Coord;
        var dir = current.Direction;
        var dist = current.Distance;

        if (_minDistanceToStop != 0 && dist < _minDistanceToStop)
        {
            if (y - 1 >= 0 && dir == 0) // 0
            {
                AddNodeIfPossible(x, y - 1, list, dir, dist, dir);
            }
            if (y + 1 < _map.Count && dir == 2) // 2
            {
                AddNodeIfPossible(x, y + 1, list, dir, dist, dir);
            }
            if (x + 1 < _map[0].Length && dir == 1) // 1
            {
                AddNodeIfPossible(x + 1, y, list, dir, dist, dir);
            }
            if (x - 1 >= 0 && dir == 3) // 3
            {
                AddNodeIfPossible(x - 1, y, list, dir, dist, dir);
            }
            return list;
        }

        for (int i = 0; i < 4; i++) //all directions
        {
            // 0 !-> 2 
            if (dir == 0 && i == 2) continue;
            if (dir == 1 && i == 3) continue;
            if (dir == 2 && i == 0) continue;
            if (dir == 3 && i == 1) continue;

            /*if (dist == _maxOneDirectionDistance && (dir == 0 || dir == 2))
            {
                if (x + 1 < _map[0].Length && i == 1) // 1
                {
                    AddNodeIfPossible(x + 1, y, list, dir, dist, i);
                }
                if (x - 1 >= 0 && i == 3) // 3
                {
                    AddNodeIfPossible(x - 1, y, list, dir, dist, i);
                }
            }
            else if (dist == _maxOneDirectionDistance && (dir == 1 || dir == 3))
            {
                if (y - 1 >= 0 && i == 0) // 0
                {
                    AddNodeIfPossible(x, y - 1, list, dir, dist, i);
                }
                if (y + 1 < _map.Count && i == 2) // 2
                {
                    AddNodeIfPossible(x, y + 1, list, dir, dist, i);
                }
            }
            else
            {*/

            if (y - 1 >= 0 && i == 0) // 0
            {
                AddNodeIfPossible(x, y - 1, list, dir, dist, i);
            }
            if (y + 1 < _map.Count && i == 2) // 2
            {
                AddNodeIfPossible(x, y + 1, list, dir, dist, i);
            }
            if (x + 1 < _map[0].Length && i == 1) // 1
            {
                AddNodeIfPossible(x + 1, y, list, dir, dist, i);
            }
            if (x - 1 >= 0 && i == 3) // 3
            {
                AddNodeIfPossible(x - 1, y, list, dir, dist, i);
            }
            //}
        }
        return list;
    }

    private Node AddNodeIfPossible(int x, int y, List<Node> list, int dirPrev, int dist, int dirCurrent)
    {
        Node node = new Node()
        {
            Coord = (x, y),
            Direction = dirCurrent,
            Distance = dirPrev == dirCurrent ? dist + 1 : 1
        };
        if (node.Distance <= _maxOneDirectionDistance)
        {
            list.Add(node);
        }

        return node;
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

    private HashSet<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current, Dictionary<(int, int), int> cellCost)
    {
        var path = new HashSet<Node>
        {
            current
        };

        while (current.Coord != (0, 0)) // not a start node
        {
            _finalSum += cellCost[current.Coord];
            current = cameFrom[current];
            var a = path.Add(current);
            if (!a)
            {
                Console.Error.WriteLine("Duplicate!");
            }
        }

        Console.WriteLine(path.Count);

        return path;
    }

    private Node LowestFromOpenSet(List<Node> openSet, Dictionary<Node, float> fScore)
    {
        var lowest = float.MaxValue;
        var current = new Node();
        for (int i = 0; i < openSet.Count; i++)
        {
            var node = openSet[i];

            if (fScore[node] <= lowest)
            {
                lowest = fScore[node];
                current = openSet[i];
            }
        }
        return current;
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

    public void Run()
    {
        ProcessFile();
        var path = AStar();
        PrintPath(path);
        
        Console.WriteLine(_finalSum);
    }

    private void PrintPath(HashSet<Node> path)
    {
        foreach (var node in path)
        {
            (int x, int y) = node.Coord;
            _map[y][x] = (short)(node.Direction == 0 ? '^' : node.Direction == 1 ? '>' : node.Direction == 2 ? 'v' : '<');
        }
        for(int i = 0;i < _map.Count; i++)
        {
            var str = "";
            for (int j = 0; j < _map[0].Length; j++)
            {
                if (_map[i][j] > 9)
                {
                    str += (char)_map[i][j];
                }
                else
                    str += ".";//_map[i][j];
            }
            Console.WriteLine(str);
        }
        Console.WriteLine();
    }
}