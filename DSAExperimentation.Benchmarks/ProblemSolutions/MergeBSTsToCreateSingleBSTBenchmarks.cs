using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Merge BSTs to Create Single BST (LC 1932): every tree in the chain below is a
// tiny 2-node stub (root, one left leaf) whose leaf value equals the NEXT tree's
// root value, so merging is always successful and the only thing that changes with
// TreeCount is how many trees the "find the tree whose root matches this leaf"
// lookup has to search - a linear scan over the tree list (O(TreeCount) per leaf,
// O(TreeCount^2) total) vs. this repo's own HashMap<int,BinaryTreeNode<int>> (O(1)
// per leaf, O(TreeCount) total), the same "index it instead of rescanning" contrast
// TwoSumBenchmarks draws for its own two strategies. Splicing mutates Left/Right in
// place, so each invocation clones the template forest first - otherwise the second
// invocation of either [Benchmark] method would run against an already-merged tree.
[MemoryDiagnoser]
public class MergeBSTsToCreateSingleBSTBenchmarks
{
    [Params(50, 500)]
    public int TreeCount;

    private List<BinaryTreeNode<int>> _template = null!;

    [GlobalSetup]
    public void Setup()
    {
        // tree i: root = TreeCount - i, left leaf = TreeCount - i - 1 (the next
        // tree's root value) - a strictly descending left-leaning chain once merged,
        // so BST order holds and every tree but the last gets spliced away.
        _template = new List<BinaryTreeNode<int>>(TreeCount);

        for (var i = 0; i < TreeCount; i++)
        {
            var rootValue = TreeCount - i;
            var tree = new BinaryTreeNode<int>(rootValue);

            if (rootValue > 1)
            {
                tree.Left = new BinaryTreeNode<int>(rootValue - 1);
            }

            _template.Add(tree);
        }
    }

    [Benchmark(Baseline = true)]
    public bool LinearScanMerge()
    {
        var trees = CloneForest();
        var used = new bool[trees.Count];
        used[0] = true;

        BinaryTreeNode<int>? FindRoot(int value)
        {
            for (var i = 1; i < trees.Count; i++)
            {
                if (!used[i] && trees[i].Value == value)
                {
                    used[i] = true;
                    return trees[i];
                }
            }

            return null;
        }

        return Splice(trees[0], FindRoot) is not null;
    }

    [Benchmark]
    public bool HashMapIndexedMerge()
    {
        var trees = CloneForest();
        var rootByValue = new HashMap<int, BinaryTreeNode<int>>();

        for (var i = 1; i < trees.Count; i++)
        {
            rootByValue.Set(trees[i].Value, trees[i]);
        }

        BinaryTreeNode<int>? FindRoot(int value)
        {
            if (!rootByValue.TryGetValue(value, out var tree))
            {
                return null;
            }

            rootByValue.TryRemove(value);
            return tree;
        }

        return Splice(trees[0], FindRoot) is not null;
    }

    private List<BinaryTreeNode<int>> CloneForest()
    {
        var clones = new List<BinaryTreeNode<int>>(_template.Count);

        foreach (var tree in _template)
        {
            clones.Add(CloneNode(tree));
        }

        return clones;
    }

    private static BinaryTreeNode<int> CloneNode(BinaryTreeNode<int> node)
    {
        var clone = new BinaryTreeNode<int>(node.Value);

        if (node.Left is { } left)
        {
            clone.Left = CloneNode(left);
        }

        if (node.Right is { } right)
        {
            clone.Right = CloneNode(right);
        }

        return clone;
    }

    private static BinaryTreeNode<int>? Splice(BinaryTreeNode<int> node, Func<int, BinaryTreeNode<int>?> findRoot)
    {
        if (node.Left is null && node.Right is null && findRoot(node.Value) is { } mergeRoot)
        {
            return Splice(mergeRoot, findRoot);
        }

        if (node.Left is { } left)
        {
            node.Left = Splice(left, findRoot);
        }

        if (node.Right is { } right)
        {
            node.Right = Splice(right, findRoot);
        }

        return node;
    }
}
