using System.Text;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Subtree of Another Tree (LC 572): the textbook O(n*m) "try SameTree at every
// node" recursion vs. serializing both trees to a '#'-delimited preorder string
// (every token prefixed with '#', so no two values can ever run together across a
// token boundary) and checking subRoot's serialization is a substring of root's
// via this repo's own KMP PrefixFunctionSearch.FindAll, O(n+m). Both trees are
// left-skewed chains sharing the same filler value so the naive approach can't
// short-circuit on an early mismatch - it has to walk deep into every candidate
// start before failing - and subRoot's deepest node carries a value that never
// appears in root, so neither strategy ever finds a real match and both run to
// completion.
[MemoryDiagnoser]
public class SubtreeOfAnotherTreeBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;
    private BinaryTreeNode<int> _subRoot = null!;

    [GlobalSetup]
    public void Setup()
    {
        _root = BuildLeftChain(NodeCount, lastValue: 1);
        _subRoot = BuildLeftChain(NodeCount / 2, lastValue: -1);
    }

    private static BinaryTreeNode<int> BuildLeftChain(int length, int lastValue)
    {
        var root = new BinaryTreeNode<int>(length == 1 ? lastValue : 1);
        var current = root;

        for (var i = 1; i < length; i++)
        {
            current.Left = new BinaryTreeNode<int>(i == length - 1 ? lastValue : 1);
            current = current.Left;
        }

        return root;
    }

    [Benchmark(Baseline = true)]
    public bool RecursiveCompareAtEveryNode() => IsSubtree(_root, _subRoot);

    private static bool IsSubtree(BinaryTreeNode<int>? root, BinaryTreeNode<int> subRoot)
        => root is not null && (IsSame(root, subRoot) || IsSubtree(root.Left, subRoot) || IsSubtree(root.Right, subRoot));

    private static bool IsSame(BinaryTreeNode<int>? p, BinaryTreeNode<int>? q)
        => p is null || q is null ? p is null && q is null : p.Value == q.Value && IsSame(p.Left, q.Left) && IsSame(p.Right, q.Right);

    [Benchmark]
    public bool SerializeThenKmpSearch()
        => PrefixFunctionSearch.FindAll(Serialize(_root), Serialize(_subRoot)).Count > 0;

    private static string Serialize(BinaryTreeNode<int>? node)
    {
        var builder = new StringBuilder();
        AppendPreorder(node, builder);
        return builder.ToString();
    }

    private static void AppendPreorder(BinaryTreeNode<int>? node, StringBuilder builder)
    {
        if (node is null)
        {
            builder.Append("#null");
            return;
        }

        builder.Append('#').Append(node.Value);
        AppendPreorder(node.Left, builder);
        AppendPreorder(node.Right, builder);
    }
}
