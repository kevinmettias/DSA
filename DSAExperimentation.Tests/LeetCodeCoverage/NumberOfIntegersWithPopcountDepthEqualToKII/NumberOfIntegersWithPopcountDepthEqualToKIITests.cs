using DSAExperimentation.LeetCode.NumberOfIntegersWithPopcountDepthEqualToKII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfIntegersWithPopcountDepthEqualToKII;

// Harness only. The mutable-range popcount-depth counting itself is
// NumberOfIntegersWithPopcountDepthEqualToKIISolution's - this file just pins
// both strategies to LeetCode's published examples, each of which mixes
// range-count and point-update queries.
public sealed partial class NumberOfIntegersWithPopcountDepthEqualToKIITests
{
    public static TheoryData<long[], long[][], int[]> Examples =>
        new()
        {
            {
                [2, 4],
                [[1, 0, 1, 1], [2, 1, 1], [1, 0, 1, 0]],
                [2, 1]
            },
            {
                [3, 5, 6],
                [[1, 0, 2, 2], [2, 1, 4], [1, 1, 2, 1], [1, 0, 1, 0]],
                [3, 1, 0]
            },
            {
                [1, 2],
                [[1, 0, 1, 1], [2, 0, 3], [1, 0, 0, 1], [1, 0, 0, 2]],
                [1, 0, 1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PopcountDepthByBruteForce_LeetCodeExamples_ReturnsCountsPerRangeQuery(
        long[] nums, long[][] queries, int[] expected)
    {
        var actual = NumberOfIntegersWithPopcountDepthEqualToKIISolution.PopcountDepthByBruteForce(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PopcountDepthByFenwickBuckets_LeetCodeExamples_ReturnsCountsPerRangeQuery(
        long[] nums, long[][] queries, int[] expected)
    {
        var actual = NumberOfIntegersWithPopcountDepthEqualToKIISolution.PopcountDepthByFenwickBuckets(nums, queries);

        Assert.Equal(expected, actual);
    }
}
