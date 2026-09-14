using DSAExperimentation.LeetCode.FairDistributionOfCookies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FairDistributionOfCookies;

// Harness only. Both the hand-rolled recursion and the Backtrack.Search composition
// are FairDistributionOfCookiesSolution's - this file pins them to LeetCode's
// published examples plus the one-bag-per-child case (where symmetry breaking is all
// there is to do) and a perfectly divisible split (where the branch-and-bound bound
// is reached exactly rather than beaten).
public sealed class FairDistributionOfCookiesTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [8, 15, 10, 20, 8], 2, 31 },
            { [6, 1, 3, 2, 2, 4, 1, 2], 3, 7 },
            { [3, 1, 2], 3, 3 },
            { [4, 4, 4], 3, 4 },
            { [1, 2, 3, 4, 5, 6, 7, 8, 9], 3, 15 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DistributeCookiesByRecursiveBacktracking_LeetCodeExamples_ReturnsFairestMaximum(
        int[] cookies, int k, int expected) =>
        Assert.Equal(
            expected,
            FairDistributionOfCookiesSolution.DistributeCookiesByRecursiveBacktracking(cookies, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DistributeCookiesByBacktrackSearch_LeetCodeExamples_ReturnsFairestMaximum(
        int[] cookies, int k, int expected) =>
        Assert.Equal(
            expected,
            FairDistributionOfCookiesSolution.DistributeCookiesByBacktrackSearch(cookies, k));
}
