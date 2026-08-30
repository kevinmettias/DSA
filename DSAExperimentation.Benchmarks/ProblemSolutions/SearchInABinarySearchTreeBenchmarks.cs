using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Search in a Binary Search Tree (LC 700): the naive O(n) linear scan over every
// value (no ordering exploited) vs. the O(log n) BST descent this repo's own
// BinarySearchTree<int> gives for free by construction - a plain "compare against
// the current node, go left or right" walk over BinaryTreeNode<int>, the exact
// shape BinarySearchTree.Has already uses internally (see
// SearchInABinarySearchTreeTests for the node-returning variant this mirrors).
// Both benchmarks build their own input from the same shuffled insertion order so
// tree height stays close to O(log n) instead of the degenerate O(n)
// ascending-insertion case, the same convention DeleteNodeInABSTBenchmarks already
// uses.
[MemoryDiagnoser]
public class SearchInABinarySearchTreeBenchmarks
{
    [Params(500, 20_000)]
    public int NodeCount;

    private int[] _values = null!;
    private BinaryTreeNode<int> _root = null!;
    private int _target;

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, NodeCount).ToArray();
        var random = new Random(1);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _values = values;

        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        // presumption: allow -- values always has NodeCount >= 1 entries above, so
        // at least one Insert ran and Root is never null here.
        _root = tree.Root!;
        _target = NodeCount - 1;
    }

    [Benchmark(Baseline = true)]
    public bool LinearScan()
    {
        foreach (var value in _values)
        {
            if (value == _target)
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool BinarySearchTreeDescent()
    {
        var node = _root;

        while (node is not null && node.Value != _target)
        {
            node = _target < node.Value ? node.Left : node.Right;
        }

        return node is not null;
    }
}
