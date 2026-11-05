using DSAExperimentation.LeetCode.Solutions.KthSmallestInLexicographicalOrder;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KthSmallestInLexicographicalOrder;

// Harness only. Both strategies are KthSmallestInLexicographicalOrderSolution's -
// this file just pins them to the first/last elements of example 1's lexicographical
// order, two ranks the published example does not ask for. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class KthSmallestInLexicographicalOrderSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            // Example 1's order is [1, 10, 11, 12, 13, 2, 3, ..., 9]: it opens on 1 and
            // closes on 9.
            { 13, 1, 1 },
            { 13, 13, 9 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindKthNumberByGenerateAndSort_Cases_ReturnsExpectedValue(
        int upperBound, int rank, int expected)
    {
        var actual = KthSmallestInLexicographicalOrderSolution.FindKthNumberByGenerateAndSort(upperBound, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindKthNumberByDepthFirstTraversal_Cases_ReturnsExpectedValue(
        int upperBound, int rank, int expected)
    {
        var actual = KthSmallestInLexicographicalOrderSolution.FindKthNumberByDepthFirstTraversal(upperBound, rank);

        Assert.Equal(expected, actual);
    }
}
