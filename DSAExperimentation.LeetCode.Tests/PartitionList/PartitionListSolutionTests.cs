using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.PartitionList;

namespace DSAExperimentation.LeetCode.Tests.PartitionList;

// Harness only. Both strategies are PartitionListSolution's - this file builds
// LeetCode's published examples as linked lists and checks the resulting list's
// values.
public sealed partial class PartitionListSolutionTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 4, 3, 2, 5, 2], 3, [1, 2, 2, 4, 3, 5] },
            { [2, 1], 2, [1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionByArrayRebuild_LeetCodeExamples_PartitionsList(
        int[] values, int partitionValue, int[] expected)
    {
        var partitioned = PartitionListSolution.PartitionByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(values), partitionValue);
        var actual = LeetCodeWireFormat.FromLinkedList(partitioned);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionByPointerSplice_LeetCodeExamples_PartitionsList(
        int[] values, int partitionValue, int[] expected)
    {
        var partitioned = PartitionListSolution.PartitionByPointerSplice(
            LeetCodeWireFormat.ToLinkedList(values), partitionValue);
        var actual = LeetCodeWireFormat.FromLinkedList(partitioned);

        Assert.Equal(expected, actual);
    }
}
