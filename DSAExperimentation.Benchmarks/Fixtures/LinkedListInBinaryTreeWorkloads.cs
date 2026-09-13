using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1367 - the needle that goes with
// BinaryTrees.Skewed(nodeCount). It matches the skewed chain's first half
// (0, 1, 2, ...) and then deliberately breaks, so the one real candidate forces
// genuine matching depth (and, for the array-slice arm, real slicing) instead of
// failing at the first comparison everywhere.
internal static class LinkedListInBinaryTreeWorkloads
{
    private const int MatchDepthDivisor = 2;
    private const int MismatchValue = -1;

    public static int[] BuildNeedleValues(int nodeCount) =>
        [.. Enumerable.Range(0, nodeCount / MatchDepthDivisor), MismatchValue];

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
}
