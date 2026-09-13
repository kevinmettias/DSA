using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListComponents;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListComponents;

// Harness only. Both strategies - the per-node linear scan of nums and the Set<int>
// membership walk - are LinkedListComponentsSolution's; this file states LeetCode's
// published examples once, as the list's values and nums, plus the run-counting
// edges the two arms have to agree on.
public sealed class LinkedListComponentsTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            // LC example 1: 0 and 1 are adjacent in the list, 3 is on its own.
            { [0, 1, 2, 3], [0, 1, 3], 2 },

            // LC example 2: nums is unordered, and adjacency in the list is what
            // decides - [0, 1] then [3, 4].
            { [0, 1, 2, 3, 4], [0, 3, 1, 4], 2 },

            // Every value present: the whole list is one component.
            { [0, 1, 2, 3], [3, 2, 1, 0], 1 },

            // Nothing present: no component ever opens.
            { [0, 1, 2, 3], [], 0 },

            // A single node, present.
            { [7], [7], 1 },

            // Runs opening at the head and closing at the tail, with a gap between.
            { [0, 1, 2, 3, 4], [0, 1, 4], 2 },

            // Alternating membership: every present node is its own component.
            { [0, 1, 2, 3, 4], [0, 2, 4], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumComponentsByLinearScan_LeetCodeExamples_ReturnsConnectedComponentCount(
        int[] values, int[] nums, int expected) =>
        Assert.Equal(expected, LinkedListComponentsSolution.NumComponentsByLinearScan(BuildList(values), nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumComponentsBySetMembership_LeetCodeExamples_ReturnsConnectedComponentCount(
        int[] values, int[] nums, int expected) =>
        Assert.Equal(expected, LinkedListComponentsSolution.NumComponentsBySetMembership(BuildList(values), nums));

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

    // No previous node to link on the very first iteration (tail is still null) -
    // head itself becomes that first node instead, back in BuildList.
    private static void AppendAfter(SinglyLinkedListNode<int>? tail, SinglyLinkedListNode<int> node)
    {
        if (tail is not null)
        {
            tail.Next = node;
        }
    }
}
