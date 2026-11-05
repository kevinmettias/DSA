using DSAExperimentation.LeetCode.Solutions.CountWaysToMakeArrayWithProduct;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountWaysToMakeArrayWithProduct;

// Harness only. Both factorization strategies are
// CountWaysToMakeArrayWithProductSolution's; this file pins them to a single
// prime-power query, which is the one case that exercises a query whose k divides
// out completely inside the trial-division loop with no leftover prime. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class CountWaysToMakeArrayWithProductSolutionTests
{
    public static TheoryData<int[][], int[]> Cases =>
        new()
        {
            { [[3, 8]], [10] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void WaysToFillArrayByTrialDivision_Cases_ReturnsExpectedCounts(
        int[][] queries, int[] expected) =>
        Assert.Equal(expected, CountWaysToMakeArrayWithProductSolution.WaysToFillArrayByTrialDivision(queries));

    [Theory]
    [MemberData(nameof(Cases))]
    public void WaysToFillArrayBySmallestPrimeFactorSieve_Cases_ReturnsExpectedCounts(
        int[][] queries, int[] expected) =>
        Assert.Equal(
            expected,
            CountWaysToMakeArrayWithProductSolution.WaysToFillArrayBySmallestPrimeFactorSieve(queries));
}
