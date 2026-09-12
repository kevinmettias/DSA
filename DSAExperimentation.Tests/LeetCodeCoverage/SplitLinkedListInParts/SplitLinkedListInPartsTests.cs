using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.SplitLinkedListInParts;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SplitLinkedListInParts;

// Harness only. Both strategies are SplitLinkedListInPartsSolution's - this file
// just pins them to LeetCode's published examples, including the case where k
// exceeds the list length and trailing parts must be null.
public sealed class SplitLinkedListInPartsTests
{
    public static TheoryData<int[], int, int[]?[]> Examples =>
        new()
        {
            { [1, 2, 3], 5, [[1], [2], [3], null, null] },
            { [1, 2, 3, 4, 5, 6, 7, 8, 9, 10], 3, [[1, 2, 3, 4], [5, 6, 7], [8, 9, 10]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SplitListToPartsByArrayRebuild_LeetCodeExamples_SplitsEvenlyWithEarlyPartsAbsorbingRemainder(
        int[] values, int k, int[]?[] expected) =>
        AssertParts(SplitLinkedListInPartsSolution.SplitListToPartsByArrayRebuild(Build(values), k), expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void SplitListToPartsByInPlaceRewire_LeetCodeExamples_SplitsEvenlyWithEarlyPartsAbsorbingRemainder(
        int[] values, int k, int[]?[] expected) =>
        AssertParts(SplitLinkedListInPartsSolution.SplitListToPartsByInPlaceRewire(Build(values), k), expected);

    private static void AssertParts(SinglyLinkedListNode<int>?[] parts, int[]?[] expected)
    {
        Assert.Equal(expected.Length, parts.Length);

        for (var i = 0; i < expected.Length; i++)
        {
            if (expected[i] is null)
            {
                Assert.Null(parts[i]);
            }
            else
            {
                Assert.Equal(expected[i], ToArray(parts[i]));
            }
        }
    }

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
