using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversalII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BinaryTreeLevelOrderTraversalIISolution's, the same
// methods BinaryTreeLevelOrderTraversalIITests proves correct.
[MemoryDiagnoser]
public class BinaryTreeLevelOrderTraversalIIBenchmarks
{
    private const int RootValue = 3;
    private const int LeftValue = 9;
    private const int RightValue = 20;
    private const int RightLeftValue = 15;
    private const int RightRightValue = 7;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() =>
        _root = new BinaryTreeNode<int>(RootValue)
        {
            Left = new(LeftValue),
            Right = new(RightValue) { Left = new(RightLeftValue), Right = new(RightRightValue) },
        };

    [Benchmark(Baseline = true)]
    public List<List<int>> QueueThenReverse() =>
        BinaryTreeLevelOrderTraversalIISolution.LevelOrderBottomByQueue(_root);

    [Benchmark]
    public List<List<int>> LevelGroupedThenReverse() =>
        BinaryTreeLevelOrderTraversalIISolution.LevelOrderBottomByLevelGroupedTraversal(_root);
}
