using DSAExperimentation.LeetCode.QueueReconstructionByHeight;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueueReconstructionByHeight;

// Harness only. Both strategies are QueueReconstructionByHeightSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class QueueReconstructionByHeightTests
{
    public static TheoryData<int[][], int[][]> Examples =>
        new()
        {
            {
                [[7, 0], [4, 4], [7, 1], [5, 0], [6, 1], [5, 2]],
                [[5, 0], [7, 0], [5, 2], [6, 1], [4, 4], [7, 1]]
            },
            {
                [[6, 0], [5, 0], [4, 0], [3, 2], [2, 2], [1, 4]],
                [[4, 0], [5, 0], [2, 2], [3, 2], [1, 4], [6, 0]]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReconstructQueueByArraySortListInsert_LeetCodeExamples_ReturnsValidOrdering(
        int[][] people, int[][] expected) =>
        Assert.Equal(expected, QueueReconstructionByHeightSolution.ReconstructQueueByArraySortListInsert(people));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReconstructQueueByMergeSortDynamicArrayInsert_LeetCodeExamples_ReturnsValidOrdering(
        int[][] people, int[][] expected) =>
        Assert.Equal(
            expected, QueueReconstructionByHeightSolution.ReconstructQueueByMergeSortDynamicArrayInsert(people));
}
