using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Solutions.MaximumProfitFromValidTopologicalOrderInDag;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: every arm is
// MaximumProfitFromValidTopologicalOrderInDagSolution's, the same methods
// CatalogCaseTests proves correct. The composed
// arms are handed a prebuilt PrecedenceMasks so that one-time cost is charged to
// [GlobalSetup], not to the search being measured. Sizes are per arm: the baseline's
// backtracking degrades toward O(n!) as edges thin out and stops at 8 nodes, the same
// reason FindTheMinimumCostArrayPermutationBenchmarks caps its own brute-force arm at
// N=8; the memo arms run on to LC 3530's n <= 22. Their states are the sets of placed
// nodes the DAG allows, so the two DAGs are its two regimes:
// - sparse: MaxProfitWorkloads keeps the DAG sparse enough for the placed sets to
//   multiply with n, while some of the 2^n stay unreachable;
// - edgeless: no edges at all, which LeetCode also allows, so all 2^n are reached. That
//   is the case DenseMemoization exists for, trading the Dictionary's per-state entries
//   for 2^n flat slots.
public class MaximumProfitFromValidTopologicalOrderInDagBenchmarks
{
    private const int Seed = 3530;
    private const string SparseDag = "sparse";
    private const string EdgelessDag = "edgeless";

    private Dictionary<int, (int[][] Edges, int[] Score, PrecedenceMasks Predecessors)> _workloadByNodeCount = [];

    public static IEnumerable<int> BacktrackingNodeCounts => [6, 8];

    public static IEnumerable<int> MemoizationNodeCounts => [.. BacktrackingNodeCounts, 15, 22];

    [Params(SparseDag, EdgelessDag)]
    public string Dag { get; set; } = SparseDag;

    // Every node count any arm runs is built here, outside the timed region; an arm looks its own up.
    // The edgeless DAG keeps the sparse one's scores and drops its edges.
    [GlobalSetup]
    public void Setup() =>
        _workloadByNodeCount = BacktrackingNodeCounts.Union(MemoizationNodeCounts).ToDictionary(
            nodeCount => nodeCount,
            nodeCount =>
            {
                var (edges, score) = MaxProfitWorkloads.Build(nodeCount, Seed);

                if (Dag == EdgelessDag)
                {
                    edges = [];
                }

                return (edges, score, PrecedenceMasks.Build(nodeCount, edges));
            });

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BacktrackingNodeCounts))]
    public long Backtracking(int nodeCount)
    {
        var workload = _workloadByNodeCount[nodeCount];

        return MaximumProfitFromValidTopologicalOrderInDagSolution.MaxProfitByBacktracking(nodeCount, workload.Edges, workload.Score);
    }

    [Benchmark]
    [ArgumentsSource(nameof(MemoizationNodeCounts))]
    public long BitmaskMemoization(int nodeCount)
    {
        var workload = _workloadByNodeCount[nodeCount];

        return MaximumProfitFromValidTopologicalOrderInDagSolution.MaxProfitByBitmaskMemoization(workload.Predecessors, workload.Score);
    }

    [Benchmark]
    [ArgumentsSource(nameof(MemoizationNodeCounts))]
    public long DenseMemoization(int nodeCount)
    {
        var workload = _workloadByNodeCount[nodeCount];

        return MaximumProfitFromValidTopologicalOrderInDagSolution.MaxProfitByDenseBitmaskMemoization(workload.Predecessors, workload.Score);
    }
}
