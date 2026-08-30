using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MiddleOfTheLinkedList;

// LeetCode 876. Middle of the Linked List: Floyd's slow/fast two-pointer walk
// composed directly against this repo's own SinglyLinkedListNode<TValue>.Next -
// the same node representation CycleDetection.cs's FindMeetingPoint walks
// two-steps-per-one, applied here to land on the midpoint of an (always acyclic)
// list instead of a cycle's meeting point. CycleDetection itself doesn't apply -
// its ReferenceEquals check can only ever fire inside a real cycle - so the walk
// is written directly here, the same "no algorithm primitive needed, just the
// node representation" shape AddTwoNumbersTests' digit-wise walk already uses.
public sealed partial class MiddleOfTheLinkedListTests
{
    [Fact]
    public void MiddleNode_OddLength_ReturnsSingleMiddleNode()
        => Assert.Equal(3, MiddleNode(Build([1, 2, 3, 4, 5]))!.Value);

    [Fact]
    public void MiddleNode_EvenLength_ReturnsSecondMiddleNode()
        => Assert.Equal(4, MiddleNode(Build([1, 2, 3, 4, 5, 6]))!.Value);

    [Fact]
    public void MiddleNode_SingleNode_ReturnsThatNode()
        => Assert.Equal(1, MiddleNode(Build([1]))!.Value);

    private static SinglyLinkedListNode<int>? MiddleNode(SinglyLinkedListNode<int>? head)
    {
        var slow = head;
        var fast = head;

        while (fast?.Next is not null)
        {
            slow = slow!.Next;
            fast = fast.Next.Next;
        }

        return slow;
    }

    private static SinglyLinkedListNode<int> Build(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }
}
