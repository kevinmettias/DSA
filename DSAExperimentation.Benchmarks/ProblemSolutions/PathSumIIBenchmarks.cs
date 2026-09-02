using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSumII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathSumIISolution's, the same methods
// PathSumIITests proves correct. The original benchmark's two arms counted
// matching paths instead of building them - weaker than the test's own helper,
// which already built LeetCode's real answer - so both arms here return the
// paths themselves, same as the tree and target below.
[MemoryDiagnoser]
public class PathSumIIBenchmarks
{
    private const int TargetPathSum = 22;

    private const int RootValue = 5;
    private const int LeftValue = 4;
    private const int LeftLeftValue = 11;
    private const int LeftLeftLeftValue = 7;
    private const int LeftLeftRightValue = 2;
    private const int RightValue = 8;
    private const int RightLeftValue = 13;
    private const int RightRightValue = 4;
    private const int RightRightLeftValue = 5;
    private const int RightRightRightValue = 1;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = new BinaryTreeNode<int>(RootValue)
    {
        Left = new BinaryTreeNode<int>(LeftValue)
        {
            Left = new BinaryTreeNode<int>(LeftLeftValue)
            {
                Left = new BinaryTreeNode<int>(LeftLeftLeftValue),
                Right = new BinaryTreeNode<int>(LeftLeftRightValue)
            }
        },
        Right = new BinaryTreeNode<int>(RightValue)
        {
            Left = new BinaryTreeNode<int>(RightLeftValue),
            Right = new BinaryTreeNode<int>(RightRightValue)
            {
                Left = new BinaryTreeNode<int>(RightRightLeftValue),
                Right = new BinaryTreeNode<int>(RightRightRightValue)
            }
        }
    };

    [Benchmark(Baseline = true)]
    public List<List<int>> RecursiveBacktrack() =>
        PathSumIISolution.FindPathsByRecursiveBacktrack(_root, TargetPathSum);

    [Benchmark]
    public List<List<int>> AllRootToLeafPaths() =>
        PathSumIISolution.FindPathsByAllRootToLeafPaths(_root, TargetPathSum);
}
