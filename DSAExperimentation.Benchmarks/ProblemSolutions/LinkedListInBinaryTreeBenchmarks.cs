using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Linked List in Binary Tree (LC 1367): LeetCode hands you a real singly linked
// list, but re-slicing a fresh int[1..] at every recursive step is the
// array-shaped habit most first-draft solutions reach for instead - it
// reallocates on every "does the rest still match" check. Walking the given
// SinglyLinkedListNode<TValue>.Next chain directly (this repo's own primitive,
// and the one actually shaped like the problem's input) makes the same
// comparisons with zero extra allocation. Both share the identical "try every
// node as a start, recurse into both children" search shape - only how the
// remaining pattern is threaded through recursion differs. The tree is a
// skewed chain whose first half matches the needle before it deliberately
// fails, forcing real recursion depth (and, for the array variant, real
// slicing) instead of failing out on the first comparison.
[MemoryDiagnoser]
public class LinkedListInBinaryTreeBenchmarks
{
    private const int MatchDepthDivisor = 2;

    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;
    private int[] _headValues = null!;
    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup()
    {
        _root = BinaryTrees.Skewed(NodeCount);

        // Matches the skewed chain's first NodeCount/2 values (0,1,2,...), then
        // deliberately breaks - forces genuine matching depth for the one real
        // candidate instead of failing at the first comparison everywhere.
        var matchDepth = NodeCount / MatchDepthDivisor;
        _headValues = [.. Enumerable.Range(0, matchDepth), -1];
        _head = BuildList(_headValues);
    }

    [Benchmark(Baseline = true)]
    public bool ArraySliceWalk() => IsSubPath(_headValues, _root);

    [Benchmark]
    public bool LinkedNodeWalk() => IsSubPath(_head, _root);

    private static bool IsSubPath(int[] headValues, BinaryTreeNode<int>? root)
        => root is not null && (MatchesArray(headValues, root) || IsSubPath(headValues, root.Left) || IsSubPath(headValues, root.Right));

    private static bool MatchesArray(int[] headValues, BinaryTreeNode<int>? node)
        => headValues.Length == 0 || (node is not null && node.Value == headValues[0] &&
            (MatchesArray(headValues[1..], node.Left) || MatchesArray(headValues[1..], node.Right)));

    private static bool IsSubPath(SinglyLinkedListNode<int>? head, BinaryTreeNode<int>? root)
        => root is not null && (MatchesNodes(head, root) || IsSubPath(head, root.Left) || IsSubPath(head, root.Right));

    private static bool MatchesNodes(SinglyLinkedListNode<int>? head, BinaryTreeNode<int>? node)
        => head is null || (node is not null && node.Value == head.Value &&
            (MatchesNodes(head.Next, node.Left) || MatchesNodes(head.Next, node.Right)));

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        for (var i = 1; i < values.Length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(values[i]);
            tail = tail.Next;
        }

        return head;
    }
}
