using System.Reflection.Metadata;

namespace AdventOfCode;

public class Day18: IRunnable
{

    string fileName = "data/day18_data.txt";

    private List<(int, long)> _dig = [];
    private List<(long, long)> _coords = [];
    private List<(double, double)> _expandedCoords = [];

    private double _perimeter = 0d;

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
                    //Part 1
                    /*var parts = line.Split(' ');

                    var dir = parts[0] switch
                    {
                        "U" => 0,
                        "R" => 1,
                        "D" => 2,
                        _ => 3
                    };
                    var num = long.Parse(parts[1]);*/


                    //Part 2
                    var str = line.Split(new char[] { ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    var hexString = str[1..^1];
                    var num = Convert.ToInt64(hexString, 16);

                    var dir = str.Last() switch
                    {
                        '0' => 1,
                        '1' => 2,
                        '2' => 3,
                        _ => 0
                    };


                    _dig.Add((dir, num));
                    //Console.WriteLine(_dig.Last());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private (long X, long Y) Dig((long X, long Y) last, (int Dir, long Dist) dig)
    {
        switch (dig.Dir) 
        {
            case 0:
                last.Y -= dig.Dist;
                break;

            case 1:
                last.X += dig.Dist;
                break;

            case 2:
                last.Y += dig.Dist;
                break;

            case 3:
                last.X -= dig.Dist;
                break;
        }

        _perimeter += (dig.Dist) * 0.5d - 0.5d;
        return last;
    }

    public void Run()
    {
        checked
        {
            ProcessFile();
            var last = (0L, 0L);

            foreach (var dig in _dig)
            {
                last = Dig(last, dig);
                _coords.Add(last);
                Console.WriteLine(last);
            }


            ExpandCoordinates(_coords);

            var sumA = GetASumOfCoords(_expandedCoords);

            var sumB = GetBSumOfCoords(_expandedCoords);

            var final = Math.Abs((sumA - sumB)) / 2m;

            Console.WriteLine(final);
        }
    }

    // if three outside expand to middle one
    // if one is outside expand to that one
    private void ExpandCoordinates(List<(long, long)> coords)
    {
        foreach (var coord in coords)
        {
            var inPointIndx = 0;
            var outPointIndx = 0;

            var outPointsCount = 0;
            var points = GetFourPossiblePoints(coord);

            for (int i = 0; i < points.Length; i++)
            {
                if (!IsPointInside(coords, points[i]))
                {
                    outPointsCount++;
                    outPointIndx = i;
                }
                else
                {
                    inPointIndx = i;
                }
            }
            var addCoord = outPointsCount == 1 ? points[outPointIndx] : points[(inPointIndx + 2) % 4]; //add first or middle
            _expandedCoords.Add(addCoord);
        }
    }

    //Shoelace Theorem
    private decimal GetASumOfCoords(List<(double x, double y)> coords)
    {
        var sum = 0m;
        for (int i = 0; i < coords.Count; i++)
        {
            var next = i + 1 < coords.Count ? i + 1 : 0;

            sum += ((decimal)coords[i].x * (decimal)coords[next].y);
        }
        return sum;
    }

    private decimal GetBSumOfCoords(List<(double x, double y)> coords)
    {
        var sum = 0m;
        for (int i = 0; i < coords.Count; i++)
        {
            var next = i + 1 < coords.Count ? i + 1 : 0;

            sum += (decimal)coords[i].y * (decimal)coords[next].x;

        }
        return sum;
    }

    //Raycasting
    private bool IsPointInside(List<(long x, long y)> coords, (double x, double y) point)
    {
        var count = 0;
        for (int i = 0; i < coords.Count; i++)
        {
            var next = i + 1 < coords.Count ? i + 1 : 0;

            (double x1, double y1) = coords[i];
            (double x2, double y2) = coords[next];

            if (y2 - y1 != 0 && (point.y < y1) != (point.y < y2)
                && point.x < x1 + (point.y - y1) * (x2 - x1) / (y2 - y1))
            {
                count++;
            }
        }
        return count % 2 == 1;
    }

    //exp.
    //o   o
    //  p
    //o   o

    private (double x, double y)[] GetFourPossiblePoints((long x, long y) point)
    {
        var off = 0.5d;
        return
        [
            (point.x - off, point.y - off), //up-left
            (point.x + off, point.y - off), //up-right
            (point.x + off, point.y + off), //down-right
            (point.x - off, point.y + off)  //down-left
        ];
    }
}