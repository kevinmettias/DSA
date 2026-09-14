using DSAExperimentation.LeetCode.CountWaysToMakeArrayWithProduct;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountWaysToMakeArrayWithProduct;

// Harness only. Both factorization strategies are
// CountWaysToMakeArrayWithProductSolution's; this file pins them to LeetCode's
// published examples plus a single prime-power query, which is the one case that
// exercises a query whose k divides out completely inside the trial-division loop
// with no leftover prime.
public sealed class CountWaysToMakeArrayWithProductTests
{
    public static TheoryData<int[][], int[]> Examples =>
        new()
        {
            { [[2, 6], [5, 1], [73, 660]], [4, 1, 50_734_910] },
            { [[1, 1], [2, 2], [3, 3], [4, 4], [5, 5]], [1, 2, 3, 10, 5] },
            { [[3, 8]], [10] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaysToFillArrayByTrialDivision_LeetCodeExamples_ReturnsExpectedCounts(
        int[][] queries, int[] expected) =>
        Assert.Equal(expected, CountWaysToMakeArrayWithProductSolution.WaysToFillArrayByTrialDivision(queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaysToFillArrayBySmallestPrimeFactorSieve_LeetCodeExamples_ReturnsExpectedCounts(
        int[][] queries, int[] expected) =>
        Assert.Equal(
            expected,
            CountWaysToMakeArrayWithProductSolution.WaysToFillArrayBySmallestPrimeFactorSieve(queries));
}
