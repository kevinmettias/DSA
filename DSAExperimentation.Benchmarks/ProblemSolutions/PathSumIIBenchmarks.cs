using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSumII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathSumIISolution's, the same methods
// PathSumIISolutionTests proves correct. The original benchmark's two arms counted
// matching paths instead of building them - weaker than the test's own helper,
// which already built LeetCode's real answer - so both arms here return the
// paths themselves.
//
// The tree is a seeded complete tree from PathSumWorkloads in which exactly
// MatchingPaths root-to-leaf paths sum to the target, so the answer is never empty
// while every other path still has to be walked and rejected. NodeCount stops at
// LC 113's 5,000-node cap.
public class PathSumIIBenchmarks
{
    private const int RandomSeed = 113; // LC problem number
    private const int MatchingPaths = 8;

    private BinaryTreeNode<int> _root = null!;
    private int _targetSum;

    [Params(50, 500, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        (_root, _targetSum) = PathSumWorkloads.Build(NodeCount, MatchingPaths, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public List<List<int>> RecursiveBacktrack() =>
        PathSumIISolution.FindPathsByRecursiveBacktrack(_root, _targetSum);

    [Benchmark]
    public List<List<int>> AllRootToLeafPaths() =>
        PathSumIISolution.FindPathsByAllRootToLeafPaths(_root, _targetSum);
}
