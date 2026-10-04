using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumProfitFromValidTopologicalOrderInDag;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumProfitFromValidTopologicalOrderInDagSolution's, the same methods
// MaximumProfitFromValidTopologicalOrderInDagSolutionTests proves correct. The composed
// arm is handed a prebuilt PrecedenceMasks so that one-time cost is charged to
// [GlobalSetup], not to the search being measured. Sizes are per arm: the baseline's
// backtracking degrades toward O(n!) as edges thin out and stops at 8 nodes, the same
// reason FindTheMinimumCostArrayPermutationBenchmarks caps its own brute-force arm at
// N=8; the memo arm runs on to LC 3530's n <= 22. Its states are the sets of placed
// nodes the DAG allows, so MaxProfitWorkloads keeps the DAG sparse enough for them to
// multiply with n - the edgeless graph LeetCode also allows has all 2^n.
public class MaximumProfitFromValidTopologicalOrderInDagBenchmarks
{
    private const int Seed = 3530;

    private Dictionary<int, (int[][] Edges, int[] Score, PrecedenceMasks Predecessors)> _workloadByNodeCount = [];

    public static IEnumerable<int> BacktrackingNodeCounts => [6, 8];

    public static IEnumerable<int> MemoizationNodeCounts => [.. BacktrackingNodeCounts, 15, 22];

    // Every node count any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _workloadByNodeCount = BacktrackingNodeCounts.Union(MemoizationNodeCounts).ToDictionary(
            nodeCount => nodeCount,
            nodeCount =>
            {
                var (edges, score) = MaxProfitWorkloads.Build(nodeCount, Seed);

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
}
