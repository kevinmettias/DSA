using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinarySearchTreeIterator;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is BinarySearchTreeIteratorSolution's left-spine
// stack, the same class BinarySearchTreeIteratorTests proves correct against.
// Fixtures.BinaryTrees.Balanced already builds the complete-binary-tree shape
// this needs - the algorithm walks Left/Right structurally and never inspects
// value order, so its input does not need to actually satisfy the BST property
// to time correctly. [Benchmark] drains a fresh iterator end to end so every
// next()/hasNext() pair across the tree is charged, not just the first.
[MemoryDiagnoser]
public class BinarySearchTreeIteratorBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(200, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int DrainInOrder()
    {
        var iterator = BinarySearchTreeIteratorSolution.CreateByLeftSpineStack(_root);
        var last = 0;

        while (iterator.HasNext())
        {
            last = iterator.Next();
        }

        return last;
    }
}
