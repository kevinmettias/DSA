using DSAExperimentation.LeetCode.Solutions.FairDistributionOfCookies;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FairDistributionOfCookies;

// Harness only. Both the hand-rolled recursion and the Backtrack.Search composition
// are FairDistributionOfCookiesSolution's - this file pins them to one-bag-per-child
// cases (where symmetry breaking is all there is to do) and a perfectly divisible
// split (where the branch-and-bound bound is reached exactly rather than beaten).
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class FairDistributionOfCookiesSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [3, 1, 2], 3, 3 },
            { [4, 4, 4], 3, 4 },
            { [1, 2, 3, 4, 5, 6, 7, 8, 9], 3, 15 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void DistributeCookiesByRecursiveBacktracking_Cases_ReturnsFairestMaximum(
        int[] cookies, int childCount, int expected)
    {
        var actual = FairDistributionOfCookiesSolution.DistributeCookiesByRecursiveBacktracking(cookies, childCount);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void DistributeCookiesByBacktrackSearch_Cases_ReturnsFairestMaximum(
        int[] cookies, int childCount, int expected)
    {
        var actual = FairDistributionOfCookiesSolution.DistributeCookiesByBacktrackSearch(cookies, childCount);
        Assert.Equal(expected, actual);
    }
}
