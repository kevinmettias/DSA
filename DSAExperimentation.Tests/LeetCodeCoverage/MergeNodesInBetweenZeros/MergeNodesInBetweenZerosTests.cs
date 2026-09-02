using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeNodesInBetweenZeros;

// LeetCode 2181. Merge Nodes in Between Zeros: a single left-to-right walk over this
// repo's own SinglyLinkedListNode<int>.Next, the same dummy-head list-building shape
// AddTwoNumbersTests/RemoveZeroSumConsecutiveNodesFromLinkedListTests already use.
// The list always starts and ends with a 0 delimiter; every run of non-zero nodes
// between two delimiters collapses into one new node holding their sum.
public sealed class MergeNodesInBetweenZerosTests
{
    [Fact]
    public void MergeNodes_LeetCodeExampleOne_ReturnsFourAndEleven()
    {
        var head = BuildList([0, 3, 1, 0, 4, 5, 2, 0]);

        var merged = MergeNodes(head);

        Assert.Equal([4, 11], ToArray(merged));
    }

    [Fact]
    public void MergeNodes_LeetCodeExampleTwo_ReturnsOneAndSeven()
    {
        var head = BuildList([0, 1, 0, 3, 4, 0]);

        var merged = MergeNodes(head);

        Assert.Equal([1, 7], ToArray(merged));
    }

    [Fact]
    public void MergeNodes_SingleGroup_ReturnsOneMergedNode()
    {
        var head = BuildList([0, 5, 0]);

        var merged = MergeNodes(head);

        Assert.Equal([5], ToArray(merged));
    }

    private static SinglyLinkedListNode<int>? MergeNodes(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        var sum = 0;

        for (var node = head?.Next; node is not null; node = node.Next)
        {
            if (node.Value == 0)
            {
                tail.Next = new SinglyLinkedListNode<int>(sum);
                tail = tail.Next;
                sum = 0;
            }
            else
            {
                sum += node.Value;
            }
        }

        return dummy.Next;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);
            head ??= node;
            AppendAfter(tail, node);
            tail = node;
        }

        return head;
    }

    private static void AppendAfter(SinglyLinkedListNode<int>? tail, SinglyLinkedListNode<int> node)
    {
        if (tail is not null)
        {
            tail.Next = node;
        }
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
