using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MaximumGoodSubtreeScore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumGoodSubtreeScoreSolution's, the same methods
// MaximumGoodSubtreeScoreSolutionTests proves correct.
//
// Sizes are per arm. BruteForce is exponential in subtree size (2^n dominated by the
// root), so it stops at 20 nodes; BitmaskTreeFold reuses each child's 1,024-mask
// knapsack instead, so its cost grows with the node count rather than with the
// subsets, and it runs on to LC 3575's own bound of 500 nodes. The two are compared at
// the sizes both run.
public class MaximumGoodSubtreeScoreBenchmarks
{
    private const int TreeSeed = 3575;

    private Dictionary<int, (int[] Vals, int[] Par, RootedTreeNode Root)> _treeByNodeCount = [];

    public static IEnumerable<int> BruteForceSizes => [12, 20];

    public static IEnumerable<int> BitmaskTreeFoldSizes => [.. BruteForceSizes, 100, 500];

    // Every node count any arm runs is built here, outside the timed region; an arm looks
    // its own up.
    [GlobalSetup]
    public void Setup() => _treeByNodeCount = BitmaskTreeFoldSizes.ToDictionary(count => count, BuildTree);

    private static (int[] Vals, int[] Par, RootedTreeNode Root) BuildTree(int nodeCount)
    {
        var (vals, par) = GoodSubtreeWorkloads.Build(nodeCount, TreeSeed);

        return (vals, par, ParentArrayTree.Build(par)[0]);
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public int BruteForce(int nodeCount)
    {
        var (vals, par, _) = _treeByNodeCount[nodeCount];

        return MaximumGoodSubtreeScoreSolution.GoodSubtreeScoreSumByBruteForce(vals, par);
    }

    [Benchmark]
    [ArgumentsSource(nameof(BitmaskTreeFoldSizes))]
    public int BitmaskTreeFold(int nodeCount)
    {
        var (vals, _, root) = _treeByNodeCount[nodeCount];

        return MaximumGoodSubtreeScoreSolution.GoodSubtreeScoreSumByBitmaskTreeFold(root, vals);
    }
}
