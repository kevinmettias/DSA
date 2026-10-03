using DSAExperimentation.LeetCode.QueriesOnNumberOfPointsInsideACircle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are QueriesOnNumberOfPointsInsideACircleSolution's, the
// same methods QueriesOnNumberOfPointsInsideACircleSolutionTests proves correct.
// [GlobalSetup] builds the random points and queries - LeetCode's own input shape,
// which is what both arms take - so only the counting is measured; the sort the
// pruning arm needs is part of that arm's own cost, which is the comparison.
// Coordinates lie in LC 1828's own [0, 500] and radii in [1, 200), so a query's circle
// spans well under the coordinate range and the x-range prune still discards part of
// the scan - less of it than a spread wider than LeetCode poses would let it. PointCount
// stops at LC 1828's 500 points, with as many queries.
public class QueriesOnNumberOfPointsInsideACircleBenchmarks
{
    private const int RandomSeed = 1828; // LC problem number
    private const int CoordinateBoundExclusive = 501;
    private const int QueryRadiusBoundExclusive = 200;

    private int[][] _points = [];

    private int[][] _queries = [];
    [Params(50, 500)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = Enumerable.Range(0, PointCount)
            .Select(_ => new[] { random.Next(CoordinateBoundExclusive), random.Next(CoordinateBoundExclusive) })
            .ToArray();
        _queries = Enumerable.Range(0, PointCount)
            .Select(_ => new[]
            {
                random.Next(CoordinateBoundExclusive),
                random.Next(CoordinateBoundExclusive),
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
