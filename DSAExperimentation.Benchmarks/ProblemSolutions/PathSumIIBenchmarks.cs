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
    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = Tree();

    private static BinaryTreeNode<int> Tree() => new(PathSumIIExampleTree.Root)
    {
        Left = new(PathSumIIExampleTree.Left)
        {
            Left = new(PathSumIIExampleTree.LeftLeft)
            {
                Left = new(PathSumIIExampleTree.LeftLeftLeft),
                Right = new(PathSumIIExampleTree.LeftLeftRight)
            }
        },
        Right = new(PathSumIIExampleTree.Right)
        {
            Left = new(PathSumIIExampleTree.RightLeft),
            Right = new(PathSumIIExampleTree.RightRight)
            {
                Left = new(PathSumIIExampleTree.RightRightLeft),
                Right = new(PathSumIIExampleTree.RightRightRight)
            }
        }
    };

    [Benchmark(Baseline = true)]
    public List<List<int>> RecursiveBacktrack() =>
        PathSumIISolution.FindPathsByRecursiveBacktrack(_root, PathSumIIExampleTree.TargetSum);

    [Benchmark]
    public List<List<int>> AllRootToLeafPaths() =>
        PathSumIISolution.FindPathsByAllRootToLeafPaths(_root, PathSumIIExampleTree.TargetSum);
}
