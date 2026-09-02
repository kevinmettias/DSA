using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Lattice Points Inside a Circle (LC 2249): a full-grid baseline that scans
// every integer point in the combined bounding box of all circles and checks it
// against every circle in turn - O(totalArea * circleCount) - vs. scanning each
// circle's own local bounding box and de-duplicating the union with this repo's own
// Set<Element> (HashMap<Element,bool>-backed) - O(sum of each circle's own area).
// Circles are scattered across a bound far wider than their radius so the combined
// bounding box is much larger than the sum of the circles' own local boxes - the
// case where the per-circle scan actually wins instead of just adding Set overhead
// on top of the same amount of work.
[MemoryDiagnoser]
public class CountLatticePointsInsideACircleBenchmarks
{
    private const int RandomSeed = 2249; // LC problem number
    private const int CoordinateBound = 150;
    private const int MaxRadiusExclusive = 6;
    private const int RadiusIndex = 2;

    [Params(50, 300)]
    public int CircleCount;

    private int[][] _circles = null!;
    private int _minX;
    private int _maxX;
    private int _minY;
    private int _maxY;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _circles = Enumerable.Range(0, CircleCount)
            .Select(_ => new[]
            {
                random.Next(-CoordinateBound, CoordinateBound),
                random.Next(-CoordinateBound, CoordinateBound),
                random.Next(1, MaxRadiusExclusive),
            })
            .ToArray();

        _minX = _circles.Min(circle => circle[0] - circle[RadiusIndex]);
        _maxX = _circles.Max(circle => circle[0] + circle[RadiusIndex]);
        _minY = _circles.Min(circle => circle[1] - circle[RadiusIndex]);
        _maxY = _circles.Max(circle => circle[1] + circle[RadiusIndex]);
    }

    [Benchmark(Baseline = true)]
    public int FullGridScan()
    {
        var count = 0;

        for (var x = _minX; x <= _maxX; x++)
        {
            for (var y = _minY; y <= _maxY; y++)
            {
                if (IsInsideAnyCircle(x, y))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool IsInsideAnyCircle(int x, int y)
    {
        foreach (var circle in _circles)
        {
            var dx = x - circle[0];
            var dy = y - circle[1];
            var r = circle[RadiusIndex];

            if ((dx * dx) + (dy * dy) <= r * r)
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public int PerCircleBoundingBoxWithSet()
    {
        var points = new Set<(int X, int Y)>();

        foreach (var circle in _circles)
        {
            AddCirclePoints(circle, points);
        }

        return points.Count;
    }

    private static void AddCirclePoints(int[] circle, Set<(int X, int Y)> points)
    {
        var x = circle[0];
        var y = circle[1];
        var r = circle[RadiusIndex];

        for (var dx = -r; dx <= r; dx++)
        {
            for (var dy = -r; dy <= r; dy++)
            {
                if ((dx * dx) + (dy * dy) <= r * r)
                {
                    points.TryAdd((x + dx, y + dy));
                }
            }
        }
    }
}
