using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathSumSolution's, the same methods PathSumTests
// proves correct.
[MemoryDiagnoser]
public class PathSumBenchmarks
{
    // LeetCode 112's own example tree: the root-to-leaf path RootValue -> LeftValue
    // -> LeftLeftValue -> LeftLeftRightValue (5 -> 4 -> 11 -> 2) sums to TargetSum,
    // the only accepting path among the tree's four root-to-leaf paths.
    private const int TargetSum = 22;
    private const int RootValue = 5;
    private const int LeftValue = 4;
    private const int LeftLeftValue = 11;
    private const int LeftLeftLeftValue = 7;
    private const int LeftLeftRightValue = 2;
    private const int RightValue = 8;
    private const int RightLeftValue = 13;
    private const int RightRightValue = 4;
    private const int RightRightRightValue = 1;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = Tree();

    [Benchmark(Baseline = true)]
    public bool RecursivePathSum() => PathSumSolution.HasPathSumByRecursion(_root, TargetSum);

    [Benchmark]
    public bool AllRootToLeafPathsSum() => PathSumSolution.HasPathSumByPathEnumeration(_root, TargetSum);

    private static BinaryTreeNode<int> Tree() => new(RootValue)
    {
        Left = new(LeftValue)
        {
            Left = new(LeftLeftValue)
            {
                Left = new(LeftLeftLeftValue),
                Right = new(LeftLeftRightValue)
            }
        },
        Right = new(RightValue)
        {
            Left = new(RightLeftValue),
            Right = new(RightRightValue)
            {
                Right = new(RightRightRightValue)
            }
        }
    };
}
