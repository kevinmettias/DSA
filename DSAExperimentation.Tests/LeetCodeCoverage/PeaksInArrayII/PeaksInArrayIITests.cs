using DSAExperimentation.LeetCode.PeaksInArrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PeaksInArrayII;

// Harness only. Both strategies are PeaksInArrayIISolution's - this file just
// pins them to LeetCode's published examples, including the point update
// between the two type-1 queries in every example.
public sealed class PeaksInArrayIITests
{
    public static TheoryData<int[], int[][], List<long>> Examples =>
        new()
        {
            { [1, 3, 2, 4], [[1, 0, 3], [2, 1, 1], [1, 0, 3]], [2, 0] },
            { [9, 8, 9, 8], [[1, 1, 3], [2, 2, 1], [1, 0, 2]], [1, 0] },
            { [3, 6, 2, 7, 1], [[1, 1, 3], [2, 3, 0], [1, 0, 4]], [0, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPeakSubarraysByBruteForce_LeetCodeExamples_ReturnsPeakSubarrayCountPerQuery(
        int[] nums, int[][] queries, List<long> expected) =>
        Assert.Equal(expected, PeaksInArrayIISolution.CountPeakSubarraysByBruteForce(nums, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPeakSubarraysBySegmentTree_LeetCodeExamples_ReturnsPeakSubarrayCountPerQuery(
        int[] nums, int[][] queries, List<long> expected) =>
        Assert.Equal(expected, PeaksInArrayIISolution.CountPeakSubarraysBySegmentTree(nums, queries));
}
