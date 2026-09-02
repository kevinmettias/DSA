using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 382 - everything about the strategies themselves
// now lives in LeetCode.LinkedListRandomNode; what stays here is only how large a
// chain to build.
internal static class LinkedListRandomNodeWorkloads
{
    // An ascending 0..length-1 chain, built tail-first so the head node ends up
    // holding value 0.
    public static SinglyLinkedListNode<int> Build(int length)
    {
        SinglyLinkedListNode<int>? head = null;

        for (var value = length - 1; value >= 0; value--)
        {
            head = new SinglyLinkedListNode<int>(value) { Next = head };
        }

        return head!;
    }
}
