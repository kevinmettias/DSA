using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.OddEvenLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OddEvenLinkedList;

// Harness only. Both strategies are OddEvenLinkedListSolution's - this file just
// pins them to LeetCode's published examples, plus the empty- and single-node
// edge cases neither strategy may special-case incorrectly.
public sealed class OddEvenLinkedListTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], [1, 3, 5, 2, 4] },
            { [2, 1, 3, 5, 6, 4, 7], [2, 3, 6, 7, 1, 5, 4] },
            { [], [] },
            { [42], [42] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GroupOddEvenByTwoListRebuild_LeetCodeExamples_InterleavesOddThenEvenIndices(
        int[] values, int[] expected) =>
        Assert.Equal(expected, ToArray(OddEvenLinkedListSolution.GroupOddEvenByTwoListRebuild(Build(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GroupOddEvenByInPlaceRewire_LeetCodeExamples_InterleavesOddThenEvenIndices(
        int[] values, int[] expected) =>
        Assert.Equal(expected, ToArray(OddEvenLinkedListSolution.GroupOddEvenByInPlaceRewire(Build(values))));

    private static SinglyLinkedListNode<int>? Build(int[] values)
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

        return [.. values];
    }
}
