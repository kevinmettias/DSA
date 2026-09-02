using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumPointsActivatedWithOneAddition;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumPointsActivatedWithOneAdditionSolution's,
// the same methods the Tests project proves correct. Coordinates are drawn from
// a range far smaller than the point count so x- and y-values repeat heavily,
// forcing real union-find merging into a handful of large components instead of
// every point landing in its own singleton.
[MemoryDiagnoser]
public class MaximumPointsActivatedWithOneAdditionBenchmarks
{
    private const int Seed = 3873; // LC problem number
    private const int CoordinateRangeExclusive = 500;

    [Params(200, 5_000)]
    public int PointCount;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var seen = new HashSet<(int X, int Y)>();
        var points = new List<int[]>(PointCount);

        while (points.Count < PointCount)
        {
            var x = random.Next(-CoordinateRangeExclusive, CoordinateRangeExclusive);
            var y = random.Next(-CoordinateRangeExclusive, CoordinateRangeExclusive);

            if (seen.Add((x, y)))
            {
                points.Add([x, y]);
            }
        }

        _points = [.. points];
    }

    [Benchmark(Baseline = true)]
    public int BruteForceUnionFind() =>
        MaximumPointsActivatedWithOneAdditionSolution.MaxActivatedByBruteForceUnionFind(_points);

    [Benchmark]
    public int KeyedDisjointSet() =>
        MaximumPointsActivatedWithOneAdditionSolution.MaxActivatedByKeyedDisjointSet(_points);
}
