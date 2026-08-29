using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListCycle;

public sealed partial class LinkedListCycleTests
{
    [Fact]
    public void HasCycle_CyclicList_ReturnsTrue()
    {
        var head = new SinglyLinkedListNode<int>(3); var two = new SinglyLinkedListNode<int>(2); var zero = new SinglyLinkedListNode<int>(0); var four = new SinglyLinkedListNode<int>(-4);
        head.Next = two; two.Next = zero; zero.Next = four; four.Next = two;
        Assert.True(CycleDetection.HasCycle(head));
    }
}
