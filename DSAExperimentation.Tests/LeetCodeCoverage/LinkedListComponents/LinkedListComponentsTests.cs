using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListComponents;

// LeetCode 817. Linked List Components: a single walk over this repo's own
// SinglyLinkedListNode<int>.Next, testing O(1) membership against this repo's own
// Set<int> (backed by HashMap, same ContainsDuplicateTests composition) instead of
// an O(k) linear scan of nums per node. A component starts whenever the current
// node's value is in the set and the previous node's either wasn't or didn't exist.
public sealed partial class LinkedListComponentsTests
{
    [Fact]
    public void NumComponents_TwoSeparateRuns_ReturnsTwo()
    {
        var head = BuildList([0, 1, 2, 3]);

        var count = NumComponents(head, [0, 1, 3]);

        Assert.Equal(2, count);
    }

    [Fact]
    public void NumComponents_NonConsecutiveMembersAcrossGap_ReturnsTwo()
    {
        var head = BuildList([0, 1, 2, 3, 4]);

        var count = NumComponents(head, [0, 3, 1, 4]);

        Assert.Equal(2, count);
    }

    private static int NumComponents(SinglyLinkedListNode<int>? head, int[] nums)
    {
        var present = new Set<int>();
        foreach (var n in nums)
        {
            present.TryAdd(n);
        }

        var count = 0;
        var inComponent = false;

        for (var node = head; node is not null; node = node.Next)
        {
            if (present.Has(node.Value))
            {
                if (!inComponent)
                {
                    count++;
                }

                inComponent = true;
            }
            else
            {
                inComponent = false;
            }
        }

        return count;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);
            head ??= node;

            if (tail is not null)
            {
                tail.Next = node;
            }

            tail = node;
        }

        return head;
    }
}
