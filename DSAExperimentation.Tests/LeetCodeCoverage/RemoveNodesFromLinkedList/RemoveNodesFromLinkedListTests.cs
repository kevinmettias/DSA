using DSAExperimentation.DataStructures.SinglyLinkedList;
using RepoNodeStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.SinglyLinkedList.SinglyLinkedListNode<int>>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveNodesFromLinkedList;

// LeetCode 2487. Remove Nodes From Linked List: a node survives only if its value is
// >= every value to its right, i.e. the surviving nodes form a non-increasing sequence
// left to right - the exact monotonic-stack shape AsteroidCollisionTests/
// DailyTemperaturesTests already use over this repo's own Stack<T>, here holding
// SinglyLinkedListNode<int> references instead of ints/indices: walking left to right,
// any node smaller than the incoming one gets popped (it has a greater node to its
// right, so it must be removed) before the incoming node is pushed. What remains on
// the stack, bottom to top, is exactly the answer in order - popped into an array
// (AsteroidCollisionTests' own reverse-fill precedent) and relinked in one more pass.
public sealed partial class RemoveNodesFromLinkedListTests
{
    [Fact]
    public void RemoveNodes_ClassicExampleOne_KeepsOnlyNonIncreasingSuffixValues()
        => Assert.Equal([13, 8], ToArray(RemoveNodes(BuildList([5, 2, 13, 3, 8]))));

    [Fact]
    public void RemoveNodes_AllValuesEqual_KeepsEveryNode()
        => Assert.Equal([1, 1, 1, 1], ToArray(RemoveNodes(BuildList([1, 1, 1, 1]))));

    [Fact]
    public void RemoveNodes_StrictlyIncreasingValues_KeepsOnlyTheLastNode()
        => Assert.Equal([3], ToArray(RemoveNodes(BuildList([1, 2, 3]))));

    [Fact]
    public void RemoveNodes_StrictlyDecreasingValues_KeepsEveryNode()
        => Assert.Equal([8, 3, 1], ToArray(RemoveNodes(BuildList([8, 3, 1]))));

    private static SinglyLinkedListNode<int>? RemoveNodes(SinglyLinkedListNode<int>? head)
    {
        var keep = new RepoNodeStack();

        for (var node = head; node is not null; node = node.Next)
        {
            while (keep.TryPeek(out var top) && top.Value < node.Value)
            {
                keep.TryPop(out _);
            }

            keep.Push(node);
        }

        var survivors = new SinglyLinkedListNode<int>[keep.Count];
        for (var i = survivors.Length - 1; i >= 0; i--)
        {
            keep.TryPop(out survivors[i]!);
        }

        for (var i = 0; i < survivors.Length - 1; i++)
        {
            survivors[i].Next = survivors[i + 1];
        }

        if (survivors.Length > 0)
        {
            survivors[^1].Next = null;
        }

        return survivors.Length == 0 ? null : survivors[0];
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();
        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }
}
