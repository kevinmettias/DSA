using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListCycleII;

public sealed partial class LinkedListCycleIITests
{
    [Fact]
    public void DetectCycle_CyclicList_ReturnsEntryNode()
    {
        var head = new SinglyLinkedListNode<int>(3); var two = new SinglyLinkedListNode<int>(2); var zero = new SinglyLinkedListNode<int>(0); var four = new SinglyLinkedListNode<int>(-4);
        head.Next = two; two.Next = zero; zero.Next = four; four.Next = two;
        Assert.Same(two, CycleDetection.FindCycleStart(head));
    }
}
