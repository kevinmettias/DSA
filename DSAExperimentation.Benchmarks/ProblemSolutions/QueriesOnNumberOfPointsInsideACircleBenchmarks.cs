using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.QueriesOnNumberOfPointsInsideACircle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are QueriesOnNumberOfPointsInsideACircleSolution's, the
// same methods QueriesOnNumberOfPointsInsideACircleTests proves correct.
// [GlobalSetup] builds the random points and queries - LeetCode's own input shape,
// which is what both arms take - so only the counting is measured; the sort the
// pruning arm needs is part of that arm's own cost, which is the comparison.
// Coordinates are spread far wider than the query radius (unlike LC 1828's own tight
// [-1000,1000]/[1,500] constraints) specifically so the x-range prune actually
// discards most points instead of barely narrowing the scan - the point this
// benchmark exists to demonstrate.
[MemoryDiagnoser]
public class QueriesOnNumberOfPointsInsideACircleBenchmarks
{
    private const int RandomSeed = 1828; // LC problem number
    private const int CoordinateBound = 50_000;
    private const int QueryRadiusBoundExclusive = 200;

    private int[][] _points = [];

    private int[][] _queries = [];
    [Params(1_000, 8_000)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = Enumerable.Range(0, PointCount)
            .Select(_ => new[] { random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound) })
            .ToArray();
        _queries = Enumerable.Range(0, PointCount)
            .Select(_ => new[]
            {
                random.Next(-CoordinateBound, CoordinateBound),
                random.Next(-CoordinateBound, CoordinateBound),
                random.Next(1, QueryRadiusBoundExclusive),
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan() =>
        QueriesOnNumberOfPointsInsideACircleSolution.CountPointsByBruteForceScan(_points, _queries);

    [Benchmark]
    public int[] SortedXWithBinarySearchPruning() =>
        QueriesOnNumberOfPointsInsideACircleSolution.CountPointsBySortedXPruning(_points, _queries);
}
