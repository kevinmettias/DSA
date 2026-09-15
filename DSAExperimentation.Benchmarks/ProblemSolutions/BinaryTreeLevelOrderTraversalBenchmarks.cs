using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversal;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BinaryTreeLevelOrderTraversalSolution's, the
// same methods BinaryTreeLevelOrderTraversalTests proves correct.
[MemoryDiagnoser]
public class BinaryTreeLevelOrderTraversalBenchmarks
{
    private const int RootValue = 3;
    private const int LeftValue = 9;
    private const int RightValue = 20;
    private const int RightLeftValue = 15;
    private const int RightRightValue = 7;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = Tree();

    private static BinaryTreeNode<int> Tree() =>
        new(RootValue) { Left = new(LeftValue), Right = new(RightValue) { Left = new(RightLeftValue), Right = new(RightRightValue) } };

    [Benchmark(Baseline = true)]
    public List<List<int>> QueueLevels() => BinaryTreeLevelOrderTraversalSolution.LevelOrderByQueueLevels(_root);

    [Benchmark]
    public List<List<int>> LevelGroupedTraversal() =>
        BinaryTreeLevelOrderTraversalSolution.LevelOrderByLevelGroupedTraversal(_root);
}
