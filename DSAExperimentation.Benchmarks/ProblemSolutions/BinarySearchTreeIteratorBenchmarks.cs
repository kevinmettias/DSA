using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinarySearchTreeIterator;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is BinarySearchTreeIteratorSolution's left-spine
// stack, the same class BinarySearchTreeIteratorSolutionTests proves correct against.
// The input is a complete binary tree, as Fixtures.BinaryTrees.Complete lays one
// out, whose node values are their in-order ranks 0..n-1 - so it is the binary
// search tree LC 173 promises, not just its shape. [Benchmark] drains a fresh
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
        _root = BinaryTrees.Complete(InOrderRankedLevelOrder(NodeCount));
        _drained = new int[NodeCount];
    }

    // The level-order array of the complete tree whose in-order walk reads 0, 1, ..., nodeCount - 1.
    private static int[] InOrderRankedLevelOrder(int nodeCount)
    {
        var levelOrder = new int[nodeCount];
        RankSubtree(levelOrder, index: 0, firstRank: 0);
        return levelOrder;
    }

    // Gives the subtree at heap index `index` (children at 2i + 1 and 2i + 2) the consecutive ranks
    // from firstRank on, left subtree first, and returns the rank after the last one it used.
    private static int RankSubtree(int[] levelOrder, int index, int firstRank)
    {
        if (index >= levelOrder.Length)
        {
            return firstRank;
        }

        var leftChild = (AlgorithmConstants.BranchingFactor * index) + 1;
        var ownRank = RankSubtree(levelOrder, leftChild, firstRank);
        levelOrder[index] = ownRank;
        return RankSubtree(levelOrder, leftChild + 1, ownRank + 1);
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
