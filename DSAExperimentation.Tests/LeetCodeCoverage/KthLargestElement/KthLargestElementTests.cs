using DSAExperimentation.LeetCode.KthLargestElement;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthLargestElement;

// Harness only. Both strategies are KthLargestElementSolution's - this file just
// pins them to LeetCode's published examples.
public sealed class KthLargestElementTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [3, 2, 1, 5, 6, 4], 2, 5 },
            { [3, 2, 3, 1, 2, 4, 5, 5, 6], 4, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindKthLargestByFullSort_LeetCodeExamples_ReturnsCorrectRank(
        int[] nums, int rank, int expected)
    {
        var actual = KthLargestElementSolution.FindKthLargestByFullSort(nums, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindKthLargestBySizeKMinHeap_LeetCodeExamples_ReturnsCorrectRank(
        int[] nums, int rank, int expected)
    {
        var actual = KthLargestElementSolution.FindKthLargestBySizeKMinHeap(nums, rank);

        Assert.Equal(expected, actual);
    }
}
