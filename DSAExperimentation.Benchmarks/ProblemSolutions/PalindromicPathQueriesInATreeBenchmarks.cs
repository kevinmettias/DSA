using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PalindromicPathQueriesInATree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromicPathQueriesInATreeSolution's, the same methods
// PalindromicPathQueriesInATreeSolutionTests proves correct. The workload is
// PalindromicPathQueryWorkloads': a long, thin tree whose random paths run a sizable
// fraction of its length, and one command per node, half updates and half queries.
//
// EulerFenwick is handed the TreeTour its hoisted overload takes, so laying the tree out
// is charged to [GlobalSetup]; no command changes the tour, so iterations share it. The
// ancestor walk roots the tree from edges[] inside the arm, one O(n) pass beside the
// path-length walk every one of its queries pays. Sizes are per arm: the walk stops at
// 10^4 nodes, where it is already quadratic, and the composed arm runs to LeetCode's
// 5 * 10^4 nodes and 5 * 10^4 commands.
public class PalindromicPathQueriesInATreeBenchmarks
{
    private const int RandomSeed = 3841; // LeetCode problem number

    private Dictionary<int, (int[][] Edges, string Letters, string[] Commands, TreeTour Tour)> _workloadsBySize = [];

    public static IEnumerable<int> AncestorWalkSizes => [1_000, 10_000];

    public static IEnumerable<int> EulerFenwickSizes => [.. AncestorWalkSizes, 50_000];

    [GlobalSetup]
    public void Setup() => _workloadsBySize = EulerFenwickSizes.ToDictionary(size => size, BuildWorkload);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(AncestorWalkSizes))]
    public bool[] AncestorWalk(int nodeCount)
    {
        var workload = _workloadsBySize[nodeCount];

        return PalindromicPathQueriesInATreeSolution.GetPalindromePathFlagsByAncestorWalk(
            nodeCount, workload.Edges, workload.Letters, workload.Commands);
    }

    [Benchmark]
    [ArgumentsSource(nameof(EulerFenwickSizes))]
    public bool[] EulerFenwick(int nodeCount)
    {
        var workload = _workloadsBySize[nodeCount];

        return PalindromicPathQueriesInATreeSolution.GetPalindromePathFlagsByEulerFenwick(
            workload.Tour, workload.Letters, workload.Commands);
    }

    private static (int[][] Edges, string Letters, string[] Commands, TreeTour Tour) BuildWorkload(int nodeCount)
    {
        var (edges, letters, commands) = PalindromicPathQueryWorkloads.Build(nodeCount, RandomSeed);

        return (edges, letters, commands, TreeTour.Build(nodeCount, edges));
    }
}
