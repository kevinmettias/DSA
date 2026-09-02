using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Queries on Number of Points Inside a Circle (LC 1828): a brute-force baseline that
// scans every point for every query vs. sorting the points by x once and using
// BinarySearch.LowerBound/UpperBound over an ArraySequence<int> of the sorted
// x-coordinates to narrow each query down to only the points whose x falls within
// [qx-r, qx+r] before the squared-distance check - the same approach
// QueriesOnNumberOfPointsInsideACircleTests uses. Both replay the same random
// points/queries. Coordinates are spread far wider than the query radius (unlike
// LC 1828's own tight [-1000,1000]/[1,500] constraints) specifically so the x-range
// prune actually discards most points instead of barely narrowing the scan - the
// point this benchmark exists to demonstrate.
[MemoryDiagnoser]
public class QueriesOnNumberOfPointsInsideACircleBenchmarks
{
    private const int RandomSeed = 1828; // LC problem number
    private const int CoordinateBound = 50_000;
    private const int QueryRadiusBoundExclusive = 200;
    private const int RadiusIndex = 2;

    [Params(1_000, 8_000)]
    public int PointCount;

    private int[][] _points = null!;
    private int[][] _queries = null!;

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
    public int BruteForceScan()
    {
        var total = 0;

        foreach (var query in _queries)
        {
            var x = query[0];
            var y = query[1];
            var r = query[RadiusIndex];

            foreach (var point in _points)
            {
                long dx = point[0] - x;
                long dy = point[1] - y;

                if ((dx * dx) + (dy * dy) <= (long)r * r)
                {
                    total++;
                }
            }
        }

        return total;
    }

    [Benchmark]
    public int SortedXWithBinarySearchPruning()
    {
        var sortedPoints = _points.OrderBy(point => point[0]).ToArray();
        var xs = sortedPoints.Select(point => point[0]).ToArray();
        var sequence = new ArraySequence<int>(xs);
        var total = 0;

        foreach (var query in _queries)
        {
            total += CountPointsWithinRadius(query, sortedPoints, sequence);
        }

        return total;
    }

    private static int CountPointsWithinRadius(int[] query, int[][] sortedPoints, ArraySequence<int> sequence)
    {
        var x = query[0];
        var y = query[1];
        var r = query[RadiusIndex];
        var lo = BinarySearch.LowerBound(sequence, x - r);
        var hi = BinarySearch.UpperBound(sequence, x + r);
        var count = 0;

        for (var i = lo; i < hi; i++)
        {
            var dx = sortedPoints[i][0] - x;
            var dy = sortedPoints[i][1] - y;

            if ((dx * dx) + (dy * dy) <= r * r)
            {
                count++;
            }
        }

        return count;
    }
}
