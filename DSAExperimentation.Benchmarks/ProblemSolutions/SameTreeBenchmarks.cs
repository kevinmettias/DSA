using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.SameTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SameTreeSolution's, the same methods SameTreeSolutionTests
// proves correct. The pre-migration version carried two [Benchmark] methods that
// both called the exact same private recursive comparison - one strategy measured
// twice, not two - so the pair here is the recursive compare against the genuinely
// different iterative compare, which carries the node pairs on an explicit stack.
//
// Both trees are built separately from one gapless level-order array, its values
// drawn from a seeded Random across LC 100's [-10^4, 10^4], so they are complete,
// structurally equal and equal at every node: neither arm can short-circuit on a
// first mismatch and both walk every corresponding pair. NodeCount stops at LC
// 100's 100-node cap.
public class SameTreeBenchmarks
{
    private const int RandomSeed = 100; // LC problem number
    private const int MinValue = -10_000;
    private const int MaxValue = 10_000;

    private BinaryTreeNode<int>? _firstTree;
    private BinaryTreeNode<int>? _secondTree;

    [Params(10, 100)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var levelOrder = SeededDraws.Values(NodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));
        var wireFormat = Array.ConvertAll(levelOrder, value => (int?)value);
        _firstTree = LeetCodeWireFormat.ToBinaryTree(wireFormat);
        _secondTree = LeetCodeWireFormat.ToBinaryTree(wireFormat);
    }

    [Benchmark(Baseline = true)]
    public bool IsSameByRecursiveCompare() => SameTreeSolution.IsSameByRecursiveCompare(_firstTree, _secondTree);

    [Benchmark]
    public bool IterativeStackCompare() => SameTreeSolution.IsSameByIterativeStackCompare(_firstTree, _secondTree);
}
