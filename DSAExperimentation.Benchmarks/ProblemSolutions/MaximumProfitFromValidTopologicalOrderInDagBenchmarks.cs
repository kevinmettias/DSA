using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumProfitFromValidTopologicalOrderInDag;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumProfitFromValidTopologicalOrderInDagSolution's, the same methods
// MaximumProfitFromValidTopologicalOrderInDagTests proves correct. The composed
// arm is handed a prebuilt PrecedenceMasks so that one-time cost is charged to
// [GlobalSetup], not to the search being measured. N is kept well under LC's own
// n <= 22: the baseline's backtracking degrades toward O(n!) as edges thin out,
// the same reason FindTheMinimumCostArrayPermutationBenchmarks caps its own
// brute-force arm at N=8.
[MemoryDiagnoser]
public class MaximumProfitFromValidTopologicalOrderInDagBenchmarks
{
    private const int Seed = 3530;

    private int[][] _edges = [];

    private int[] _score = [];
    private PrecedenceMasks _predecessors = null!;
    [Params(6, 8)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        (_edges, _score) = MaxProfitWorkloads.Build(N, Seed);
        _predecessors = PrecedenceMasks.Build(N, _edges);
    }

    [Benchmark(Baseline = true)]
    public long Backtracking() =>
        MaximumProfitFromValidTopologicalOrderInDagSolution.MaxProfitByBacktracking(N, _edges, _score);

    [Benchmark]
    public long BitmaskMemoization() =>
        MaximumProfitFromValidTopologicalOrderInDagSolution.MaxProfitByBitmaskMemoization(_predecessors, _score);
}
