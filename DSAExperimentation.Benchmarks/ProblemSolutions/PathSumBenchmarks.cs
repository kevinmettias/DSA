using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathSumSolution's, the same methods PathSumSolutionTests
// proves correct. The tree is a seeded complete tree from PathSumWorkloads in which
// only the last root-to-leaf path, in left-to-right order, sums to the target: the
// answer is true, but the recursion, which stops at its first match, still walks
// every other path before reaching it, as the path enumeration does. NodeCount stops
// at LC 112's 5,000-node cap.
public class PathSumBenchmarks
{
    private const int RandomSeed = 112; // LC problem number
    private const int MatchingPaths = 1;

    private BinaryTreeNode<int> _root = null!;
    private int _targetSum;

    [Params(50, 500, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        (_root, _targetSum) = PathSumWorkloads.Build(NodeCount, MatchingPaths, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public bool HasPathSumByRecursion() => PathSumSolution.HasPathSumByRecursion(_root, _targetSum);

    [Benchmark]
    public bool HasPathSumByPathEnumeration() => PathSumSolution.HasPathSumByPathEnumeration(_root, _targetSum);
}
