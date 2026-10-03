using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.IncrementalEvenWeightedCycleQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IncrementalEvenWeightedCycleQueriesSolution's, the
// same methods IncrementalEvenWeightedCycleQueriesSolutionTests proves correct.
// [GlobalSetup] builds one fixed, random edge stream over EdgeCount nodes so
// stream construction is charged to setup rather than to the pass each
// [Benchmark] arm measures.
public class IncrementalEvenWeightedCycleQueriesBenchmarks
{
    private const int Seed = 3887;

    private int _nodeCount;

    private int[][] _edges = [];
    [Params(500, 5_000)]
    public int EdgeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nodeCount = EdgeCount;
        _edges = IncrementalEvenWeightedCycleQueriesWorkloads.BuildEdges(_nodeCount, EdgeCount, Seed);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceBfs() =>
        IncrementalEvenWeightedCycleQueriesSolution.CountAddedEdgesByBruteForceBfs(_nodeCount, _edges);

    [Benchmark]
    public int DisjointSetPrunedBfs() =>
        IncrementalEvenWeightedCycleQueriesSolution.CountAddedEdgesByDisjointSetPrunedBfs(_nodeCount, _edges);
}
