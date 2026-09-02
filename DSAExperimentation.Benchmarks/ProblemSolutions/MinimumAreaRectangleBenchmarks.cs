using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Area Rectangle (LC 939): both strategies try every pair of points as a
// candidate diagonal (O(n^2) pairs) - they differ only in how the other two
// corners are confirmed present. LinearScanLookup rescans the raw points array
// per corner check (O(n) each, O(n^3) overall). SetLookup builds this repo's own
// Set<(int X,int Y)> once up front (the same primitive PerfectRectangleTests uses
// for corner bookkeeping) and confirms each corner in O(1), the same "swap a
// linear rescan for a hash lookup" move TwoSumBenchmarks makes for its own pair.
[MemoryDiagnoser]
public class MinimumAreaRectangleBenchmarks
{
    private const int RandomSeed = 939; // LC 939
    private const int GridPadding = 2;

    [Params(60, 400)]
    public int Length;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var grid = (int)Math.Ceiling(Math.Sqrt(Length)) + GridPadding;

        var coordinates = new HashSet<(int X, int Y)>();

        while (coordinates.Count < Length)
        {
            coordinates.Add((random.Next(grid), random.Next(grid)));
        }

        _points = coordinates.Select(c => new[] { c.X, c.Y }).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanLookup()
    {
        var minArea = int.MaxValue;

        for (var i = 0; i < _points.Length; i++)
        {
            for (var j = i + 1; j < _points.Length; j++)
            {
                var (x1, y1) = (_points[i][0], _points[i][1]);
                var (x2, y2) = (_points[j][0], _points[j][1]);

                if (x1 == x2 || y1 == y2)
                {
                    continue;
                }

                if (ContainsPoint(x1, y2) && ContainsPoint(x2, y1))
                {
                    minArea = Math.Min(minArea, Math.Abs((x2 - x1) * (y2 - y1)));
                }
            }
        }

        return minArea == int.MaxValue ? 0 : minArea;
    }

    [Benchmark]
    public int SetLookup()
    {
        var seen = new Set<(int X, int Y)>();

        foreach (var point in _points)
        {
            seen.TryAdd((point[0], point[1]));
        }

        var minArea = int.MaxValue;

        for (var i = 0; i < _points.Length; i++)
        {
            for (var j = i + 1; j < _points.Length; j++)
            {
                minArea = EvaluateDiagonal(i, j, seen, minArea);
            }
        }

        return minArea == int.MaxValue ? 0 : minArea;
    }

    private int EvaluateDiagonal(int i, int j, Set<(int X, int Y)> seen, int minArea)
    {
        var (x1, y1) = (_points[i][0], _points[i][1]);
        var (x2, y2) = (_points[j][0], _points[j][1]);

        if (x1 == x2 || y1 == y2)
        {
            return minArea;
        }

        if (seen.Has((x1, y2)) && seen.Has((x2, y1)))
        {
            return Math.Min(minArea, Math.Abs((x2 - x1) * (y2 - y1)));
        }

        return minArea;
    }

    private bool ContainsPoint(int x, int y)
    {
        foreach (var point in _points)
        {
            if (point[0] == x && point[1] == y)
            {
                return true;
            }
        }

        return false;
    }
}
