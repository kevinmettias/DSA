using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathSumSolution's, the same methods PathSumTests
// proves correct.
[MemoryDiagnoser]
public class PathSumBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = Tree();

    private static BinaryTreeNode<int> Tree() => new(PathSumExampleTree.Root)
    {
        Left = new(PathSumExampleTree.Left)
        {
            Left = new(PathSumExampleTree.LeftLeft)
            {
                Left = new(PathSumExampleTree.LeftLeftLeft),
                Right = new(PathSumExampleTree.LeftLeftRight)
            }
        },
        Right = new(PathSumExampleTree.Right)
        {
            Left = new(PathSumExampleTree.RightLeft),
            Right = new(PathSumExampleTree.RightRight)
            {
                Right = new(PathSumExampleTree.RightRightRight)
            }
        }
    };

    [Benchmark(Baseline = true)]
    public bool HasPathSumByRecursion() => PathSumSolution.HasPathSumByRecursion(_root, PathSumExampleTree.TargetSum);

    [Benchmark]
    public bool HasPathSumByPathEnumeration() => PathSumSolution.HasPathSumByPathEnumeration(_root, PathSumExampleTree.TargetSum);
}
