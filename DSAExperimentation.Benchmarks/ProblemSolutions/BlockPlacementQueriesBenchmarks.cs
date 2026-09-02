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

    [Params(5_000, 50_000)]
    public int QueryCount;

    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var maxCoordinate = QueryCount * CoordinateSpread;
        var usedPositions = new HashSet<int>();
        var queries = new int[QueryCount][];

        for (var i = 0; i < QueryCount; i++)
        {
            if (i % 2 == 0 || usedPositions.Count == 0)
            {
                int position;
                do
                {
                    position = random.Next(1, maxCoordinate + 1);
                }
                while (!usedPositions.Add(position));

                queries[i] = [1, position];
            }
            else
            {
                queries[i] = [2, random.Next(1, maxCoordinate + 1), random.Next(1, maxCoordinate + 1)];
            }
        }

        _queries = queries;
    }

    [Benchmark(Baseline = true)]
    public bool[] LinearScan() => BlockPlacementQueriesSolution.CanPlaceByLinearScan(_queries);

    [Benchmark]
    public bool[] SegmentTreeMerge() => BlockPlacementQueriesSolution.CanPlaceBySegmentTreeMerge(_queries);
}
