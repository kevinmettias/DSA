using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1367 - a skewed right-only chain and a needle that
// follows it for a real depth and then breaks, so the one genuine candidate forces
// genuine matching depth (and, for the array-slice arm, real slicing) instead of
// failing at the first comparison everywhere. Every value sits in LC 1367's [1, 100]:
// the chain counts 1, 2, ..., 100 and holds 100 from there down, and the needle is
// LC's full 100 nodes - the chain's first 99 values, then a value the chain does not
// carry at that depth. Only the root holds the needle's first value, so it is the one
// real candidate.
internal static class LinkedListInBinaryTreeWorkloads
{
    private const int MaxNodeValue = 100;
    private const int MatchDepth = MaxNodeValue - 1;

    // Any value but the chain's MaxNodeValue at depth MatchDepth breaks the match there.
    private const int BreakingValue = 1;

    public static BinaryTreeNode<int> BuildChain(int nodeCount)
    {
        var root = new BinaryTreeNode<int>(ChainValueAt(0));
        var current = root;

        for (var depth = 1; depth < nodeCount; depth++)
        {
            var next = new BinaryTreeNode<int>(ChainValueAt(depth));
            current.Right = next;
            current = next;
        }

        return root;
    }

    public static int[] BuildNeedleValues() => [.. Enumerable.Range(1, MatchDepth), BreakingValue];

    public static SinglyLinkedListNode<int> BuildNeedle(int[] values)
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

    private static int ChainValueAt(int depth) => Math.Min(depth + 1, MaxNodeValue);
}
