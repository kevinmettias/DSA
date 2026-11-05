using DSAExperimentation.LeetCode.Solutions.FindXValueOfArrayII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindXValueOfArrayII;

// Harness only. Both strategies are FindXValueOfArrayIISolution's - this file just
// pins them to to one case of its own at k = 1,
// the least modulus, where every product is 0 mod 1 and so every prefix of
// nums[start..] counts: 3 prefixes from index 0, then 2 from index 1.
public sealed partial class FindXValueOfArrayIISolutionTests
{
    public static TheoryData<int[], int, int[][], int[]> Cases =>
        new()
        {
            {
                [1, 2, 3], 1,
                new[] { new[] { 0, 5, 0, 0 }, new[] { 1, 4, 1, 0 } },
                [3, 2]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void XValueCountsByBruteForce_Cases_ReturnsCountsPerQuery(
        int[] nums, int modulus, int[][] queries, int[] expected)
    {
        var actual = FindXValueOfArrayIISolution.XValueCountsByBruteForce(nums, modulus, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void XValueCountsBySegmentTreeAutomaton_Cases_ReturnsCountsPerQuery(
        int[] nums, int modulus, int[][] queries, int[] expected)
    {
        var actual = FindXValueOfArrayIISolution.XValueCountsBySegmentTreeAutomaton(nums, modulus, queries);

        Assert.Equal(expected, actual);
    }
}
