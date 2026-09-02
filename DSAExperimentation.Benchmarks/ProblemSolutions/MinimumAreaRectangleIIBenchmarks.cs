using BenchmarkDotNet.Attributes;

using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Area Rectangle II (LC 963): the brute force tries every 4-point
// combination directly (O(n^4)) and, for each, tries all 3 ways of splitting the
// 4 points into two candidate diagonals, checking the bisects-and-equal-length
// rectangle criterion by hand. DiagonalGrouping instead tries every point-PAIR
// once (O(n^2)) as a candidate diagonal and groups them by
// (2*midpointX, 2*midpointY, lengthSquared) in this repo's own
// HashMap<TKey,TValue> - any two pairs colliding on that key are already known to
// be a rectangle's two diagonals, the same "hash the derived key instead of
// rechecking geometry" move MinimumAreaRectangleBenchmarks' SetLookup makes for
// the axis-aligned version.
[MemoryDiagnoser]
public class MinimumAreaRectangleIIBenchmarks
{
    private const int RandomSeed = 963;
    private const int GridPadding = 3;

    [Params(12, 24)]
    public int Length;

    private int[][] _points = null!;

    private readonly record struct DiagonalCandidate(int First, int Second, int Third, int Fourth);

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
    public double BruteForceQuadruples()
    {
        var minArea = double.MaxValue;
        var n = _points.Length;

        for (var a = 0; a < n; a++)
        {
            for (var b = a + 1; b < n; b++)
            {
                for (var c = b + 1; c < n; c++)
                {
                    var tripleArea = BestAreaForTriple(a, b, c, n);
                    minArea = Math.Min(minArea, tripleArea);
                }
            }
        }

        return minArea == double.MaxValue ? 0.0 : minArea;
    }

    private double BestAreaForTriple(int a, int b, int c, int n)
    {
        var minArea = double.MaxValue;

        for (var d = c + 1; d < n; d++)
        {
            TryRectangle(new DiagonalCandidate(a, c, b, d), ref minArea); // diagonals (a,b) and (c,d)
            TryRectangle(new DiagonalCandidate(a, b, c, d), ref minArea); // diagonals (a,c) and (b,d)
            TryRectangle(new DiagonalCandidate(a, b, d, c), ref minArea); // diagonals (a,d) and (b,c)
        }

        return minArea;
    }

    [Benchmark]
    public double DiagonalGrouping()
    {
        var diagonalsByKey =
            new HashMap<(int SumX, int SumY, int LengthSquared), List<((int X, int Y) First, (int X, int Y) Second)>>();
        var minArea = double.MaxValue;

        for (var i = 0; i < _points.Length; i++)
        {
            var pointI = (X: _points[i][0], Y: _points[i][1]);

            for (var j = i + 1; j < _points.Length; j++)
            {
                var pointJ = (X: _points[j][0], Y: _points[j][1]);
                ProcessDiagonalCandidate(diagonalsByKey, pointI, pointJ, ref minArea);
            }
        }

        return minArea == double.MaxValue ? 0.0 : minArea;
    }

    private void ProcessDiagonalCandidate(
        HashMap<(int SumX, int SumY, int LengthSquared), List<((int X, int Y) First, (int X, int Y) Second)>> diagonalsByKey,
        (int X, int Y) pointI,
        (int X, int Y) pointJ,
        ref double minArea)
    {
        var dx = pointI.X - pointJ.X;
        var dy = pointI.Y - pointJ.Y;
        var key = (pointI.X + pointJ.X, pointI.Y + pointJ.Y, (dx * dx) + (dy * dy));

        if (!diagonalsByKey.TryGetValue(key, out var matchingDiagonals))
        {
            matchingDiagonals = [];
            diagonalsByKey.Set(key, matchingDiagonals);
        }

        foreach (var (first, second) in matchingDiagonals)
        {
            var sideA = Distance(pointI, first);
            var sideB = Distance(pointI, second);
            minArea = Math.Min(minArea, sideA * sideB);
        }

        matchingDiagonals.Add((pointI, pointJ));
    }

    // Diagonal candidates are (first, third) and (second, fourth) - a rectangle
    // iff they share a midpoint and length; area is the product of the two sides
    // meeting at the shared diagonal endpoint, per MinAreaFreeRect's own reasoning.
    private void TryRectangle(DiagonalCandidate candidate, ref double minArea)
    {
        var (first, second, third, fourth) = candidate;
        var p1 = (X: _points[first][0], Y: _points[first][1]);
        var p2 = (X: _points[second][0], Y: _points[second][1]);
        var p3 = (X: _points[third][0], Y: _points[third][1]);
        var p4 = (X: _points[fourth][0], Y: _points[fourth][1]);

        if (p1.X + p3.X != p2.X + p4.X || p1.Y + p3.Y != p2.Y + p4.Y)
        {
            return;
        }

        var diagonalOneLengthSquared = LengthSquared(p1, p3);

        if (diagonalOneLengthSquared != LengthSquared(p2, p4))
        {
            return;
        }

        var area = Distance(p1, p2) * Distance(p1, p4);
        minArea = Math.Min(minArea, area);
    }

    private static int LengthSquared((int X, int Y) a, (int X, int Y) b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return (dx * dx) + (dy * dy);
    }

    private static double Distance((int X, int Y) a, (int X, int Y) b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt((dx * dx) + (dy * dy));
    }
}
