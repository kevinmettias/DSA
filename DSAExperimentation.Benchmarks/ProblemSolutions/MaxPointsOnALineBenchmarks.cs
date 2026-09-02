using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Max Points on a Line (LC 149): the textbook O(n^3) every-triple-of-points
// collinearity check (cross product) vs. the O(n^2) per-anchor slope-grouping
// approach using this repo's own HashMap<(int,int),int> keyed by each point's
// GCD-reduced, sign-normalized slope relative to the anchor. _points is drawn
// uniformly at random over a wide coordinate range, so the answer stays small
// (2-3) and neither strategy gets to short-circuit on an early large find.
[MemoryDiagnoser]
public class MaxPointsOnALineBenchmarks
{
    private const int RandomSeed = 149; // LC problem number
    private const int CoordinateRange = 1_000;
    private const int MinimumPointsForTrivialResult = 2;
    private const int TrivialCollinearPairCount = 2;

    [Params(600, 1500)]
    public int Length;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = Enumerable.Range(0, Length)
            .Select(_ => new[] { random.Next(-CoordinateRange, CoordinateRange), random.Next(-CoordinateRange, CoordinateRange) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int EveryTripleCrossProduct()
    {
        var n = _points.Length;

        if (n <= MinimumPointsForTrivialResult)
        {
            return n;
        }

        var best = TrivialCollinearPairCount;

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                var collinearCount = CountCollinearWithPair(i, j);
                best = Math.Max(best, collinearCount);
            }
        }

        return best;
    }

    private int CountCollinearWithPair(int i, int j)
    {
        var count = TrivialCollinearPairCount;

        for (var k = j + 1; k < _points.Length; k++)
        {
            var cross = (long)(_points[j][0] - _points[i][0]) * (_points[k][1] - _points[i][1])
                      - (long)(_points[j][1] - _points[i][1]) * (_points[k][0] - _points[i][0]);

            if (cross == 0)
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int SlopeGroupedByHashMap()
    {
        var n = _points.Length;

        if (n <= MinimumPointsForTrivialResult)
        {
            return n;
        }

        var best = 1;

        for (var i = 0; i < n; i++)
        {
            var anchorBest = CountCollinearThroughAnchor(n, i);
            best = Math.Max(best, anchorBest);
        }

        return best;
    }

    private int CountCollinearThroughAnchor(int n, int anchorIndex)
    {
        var slopeCounts = new HashMap<(int Dx, int Dy), int>();
        var localBest = 0;

        for (var j = 0; j < n; j++)
        {
            localBest = AccumulateSlopeCount(slopeCounts, localBest, anchorIndex, j);
        }

        return localBest + 1;
    }

    private int AccumulateSlopeCount(HashMap<(int Dx, int Dy), int> slopeCounts, int localBest, int anchorIndex, int otherIndex)
    {
        if (otherIndex == anchorIndex)
        {
            return localBest;
        }

        var dx = _points[otherIndex][0] - _points[anchorIndex][0];
        var dy = _points[otherIndex][1] - _points[anchorIndex][1];
        var key = ReducedSlope(dx, dy);
        var count = slopeCounts.TryGetValue(key, out var existing) ? existing + 1 : 1;
        slopeCounts.Set(key, count);

        return Math.Max(localBest, count);
    }

    private static (int, int) ReducedSlope(int dx, int dy)
    {
        var divisor = Gcd(Math.Abs(dx), Math.Abs(dy));

        if (divisor == 0)
        {
            return (0, 0);
        }

        dx /= divisor;
        dy /= divisor;

        if (dx < 0 || (dx == 0 && dy < 0))
        {
            dx = -dx;
            dy = -dy;
        }

        return (dx, dy);
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
