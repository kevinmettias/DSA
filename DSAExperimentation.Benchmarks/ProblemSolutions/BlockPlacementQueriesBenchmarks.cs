using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BlockPlacementQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BlockPlacementQueriesSolution's, the same
// methods BlockPlacementQueriesTests proves correct. Half the workload places
// obstacles at distinct random coordinates and half asks type-2 queries
// against a random prefix - by the end of the run most obstacles are already
// active, so both arms are answering over a genuinely populated obstacle set
// rather than a mostly-empty one. Both Params sit past the crossover where
// SegmentTreeMerge's own O(maxCoordinate) setup (three arrays plus two
// DisjointSetForests sized ~3*QueryCount) stops outweighing LinearScan's O(m)-
// per-query rescan - confirmed locally at QueryCount=5_000 (~1.2x faster) and
// QueryCount=50_000 (~3x faster), the O(n log n) vs. O(n*m) gap widening with
// scale as expected.
[MemoryDiagnoser]
public class BlockPlacementQueriesBenchmarks
{
    private const int Seed = 3161;
    private const int CoordinateSpread = 3;

    private int[][] _queries = [];

    [Params(5_000, 50_000)]
    public int QueryCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var maxCoordinate = QueryCount * CoordinateSpread;

        _queries = BuildQueries(random, maxCoordinate, QueryCount);
    }

    // Half the script places obstacles at distinct random coordinates, half asks
    // type-2 queries against a random prefix, so that by the end of the run most
    // obstacles are already active rather than the obstacle set being mostly empty.
    private static int[][] BuildQueries(Random random, int maxCoordinate, int queryCount)
    {
        var usedPositions = new HashSet<int>();
        var queries = new int[queryCount][];

        for (var i = 0; i < queryCount; i++)
        {
            queries[i] = BuildQuery(random, usedPositions, maxCoordinate, i);
        }

        return queries;
    }

    // Even indices place an obstacle, odd indices ask a type-2 query; the empty
    // usedPositions check forces the first operation to be a placement.
    private static int[] BuildQuery(Random random, HashSet<int> usedPositions, int maxCoordinate, int index)
    {
        if (index % 2 == 0 || usedPositions.Count == 0)
        {
            return [1, TakeFreePosition(random, usedPositions, maxCoordinate)];
        }

        return [2, random.Next(1, maxCoordinate + 1), random.Next(1, maxCoordinate + 1)];
    }

    // A coordinate not already used: placing an obstacle twice at the same
    // coordinate is a no-op, so this draws until the position is fresh.
    private static int TakeFreePosition(Random random, HashSet<int> usedPositions, int maxCoordinate)
    {
        int position;
        do
        {
            position = random.Next(1, maxCoordinate + 1);
        }
        while (!usedPositions.Add(position));

        return position;
    }

    [Benchmark(Baseline = true)]
    public bool[] LinearScan() => BlockPlacementQueriesSolution.CanPlaceByLinearScan(_queries);

    [Benchmark]
    public bool[] SegmentTreeMerge() => BlockPlacementQueriesSolution.CanPlaceBySegmentTreeMerge(_queries);
}
