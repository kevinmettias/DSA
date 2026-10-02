using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SameTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SameTreeSolution's, the same methods SameTreeTests
// proves correct. The pre-migration version carried two [Benchmark] methods that
// both called the exact same private recursive comparison - one strategy measured
// twice, not two - so the pair here is the recursive compare against the genuinely
// different iterative compare, which carries the node pairs on an explicit stack.
// The two trees are equal, so neither arm can short-circuit on a first mismatch and
// both walk every corresponding pair.
[MemoryDiagnoser]
public class SameTreeBenchmarks
{
    private const int LeftChildValue = 2;
    private const int RightChildValue = 3;

    private BinaryTreeNode<int> _firstTree = null!;
    private BinaryTreeNode<int> _secondTree = null!;

    [GlobalSetup]
    public void Setup()
    {
        _firstTree = Tree();
        _secondTree = Tree();
    }

    private static BinaryTreeNode<int> Tree() => new(1) { Left = new(LeftChildValue), Right = new(RightChildValue) };

    [Benchmark(Baseline = true)]
    public bool IsSameByRecursiveCompare() => SameTreeSolution.IsSameByRecursiveCompare(_firstTree, _secondTree);

    [Benchmark]
    public bool IterativeStackCompare() => SameTreeSolution.IsSameByIterativeStackCompare(_firstTree, _secondTree);
}
