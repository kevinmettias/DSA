using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinarySearchTreeIterator;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is BinarySearchTreeIteratorSolution's left-spine
// stack, the same class BinarySearchTreeIteratorSolutionTests proves correct against.
// The input is Fixtures.CompleteSearchTrees' complete binary tree whose node
// values are their in-order ranks 0..n-1 - so it is the binary search tree LC 173
// promises, not just its shape. [Benchmark] drains a fresh
// iterator end to end so every next()/hasNext() pair across the tree is charged,
// not just the first, and returns every value next() reported, in order.
public class BinarySearchTreeIteratorBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    // Every value the drain reports; sized in setup so the drain allocates nothing.
    private int[] _drained = [];

    [Params(200, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _root = CompleteSearchTrees.InOrderRanked(NodeCount);
        _drained = new int[NodeCount];
    }

    [Benchmark(Baseline = true)]
    public int[] DrainInOrder()
    {
        var iterator = BinarySearchTreeIteratorSolution.CreateByLeftSpineStack(_root);
        var next = 0;

        while (iterator.HasNext())
        {
            _drained[next++] = iterator.Next();
        }

        return _drained;
    }
}
