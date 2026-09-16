using DSAExperimentation.LeetCode.FindXValueOfArrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindXValueOfArrayII;

// Harness only. Both strategies are FindXValueOfArrayIISolution's - this file just
// pins them to LeetCode's published examples.
public sealed class FindXValueOfArrayIITests
{
    public static TheoryData<int[], int, int[][], int[]> Examples =>
        new()
        {
            {
                [1, 2, 3, 4, 5], 3,
                new[] { new[] { 2, 2, 0, 2 }, new[] { 3, 3, 3, 0 }, new[] { 0, 1, 0, 1 } },
                [2, 2, 2]
            },
            {
                [1, 2, 4, 8, 16, 32], 4,
                new[] { new[] { 0, 2, 0, 2 }, new[] { 0, 2, 0, 1 } },
                [1, 0]
            },
            {
                [1, 1, 2, 1, 1], 2,
                new[] { new[] { 2, 1, 0, 1 } },
                [5]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void XValueCountsByBruteForce_LeetCodeExamples_ReturnsCountsPerQuery(
        int[] nums, int k, int[][] queries, int[] expected)
    {
        var actual = FindXValueOfArrayIISolution.XValueCountsByBruteForce(nums, k, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void XValueCountsBySegmentTreeAutomaton_LeetCodeExamples_ReturnsCountsPerQuery(
        int[] nums, int k, int[][] queries, int[] expected)
    {
        var actual = FindXValueOfArrayIISolution.XValueCountsBySegmentTreeAutomaton(nums, k, queries);

        Assert.Equal(expected, actual);
    }
}
