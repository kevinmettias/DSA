using DSAExperimentation.LeetCode.Solutions.FindXValueOfArrayII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindXValueOfArrayII;

// Harness only. Both strategies are FindXValueOfArrayIISolution's - this file just
// pins them to LeetCode's published examples, and to one case of its own at k = 1,
// the least modulus, where every product is 0 mod 1 and so every prefix of
// nums[start..] counts: 3 prefixes from index 0, then 2 from index 1.
public sealed partial class FindXValueOfArrayIISolutionTests
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
            {
                [1, 2, 3], 1,
                new[] { new[] { 0, 5, 0, 0 }, new[] { 1, 4, 1, 0 } },
                [3, 2]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void XValueCountsByBruteForce_LeetCodeExamples_ReturnsCountsPerQuery(
        int[] nums, int modulus, int[][] queries, int[] expected)
    {
        var actual = FindXValueOfArrayIISolution.XValueCountsByBruteForce(nums, modulus, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void XValueCountsBySegmentTreeAutomaton_LeetCodeExamples_ReturnsCountsPerQuery(
        int[] nums, int modulus, int[][] queries, int[] expected)
    {
        var actual = FindXValueOfArrayIISolution.XValueCountsBySegmentTreeAutomaton(nums, modulus, queries);

        Assert.Equal(expected, actual);
    }
}
