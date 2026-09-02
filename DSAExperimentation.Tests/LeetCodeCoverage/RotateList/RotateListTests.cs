using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RotateList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RotateList;

// Harness only. Both strategies are RotateListSolution's - this file builds
// LeetCode's published examples as linked lists and checks the resulting list's
// values.
public sealed class RotateListTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, [4, 5, 1, 2, 3] },
            { [0, 1, 2], 4, [2, 0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RotateRightByArrayRebuild_LeetCodeExamples_RotatesList(
        int[] values, int k, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(RotateListSolution.RotateRightByArrayRebuild(BuildList(values), k)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RotateRightByPointerRewire_LeetCodeExamples_RotatesList(
        int[] values, int k, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(RotateListSolution.RotateRightByPointerRewire(BuildList(values), k)));

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
