using DSAExperimentation.LeetCode.KthSmallestInLexicographicalOrder;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSmallestInLexicographicalOrder;

// Harness only. Both strategies are KthSmallestInLexicographicalOrderSolution's -
// this file just pins them to LeetCode's published example plus the first/last
// elements of that same lexicographical order.
public sealed class KthSmallestInLexicographicalOrderTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 13, 2, 10 },
            { 13, 1, 1 },
            { 13, 13, 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindKthNumberByGenerateAndSort_LeetCodeExamples_ReturnsExpectedValue(
        int upperBound, int rank, int expected)
    {
        var actual = KthSmallestInLexicographicalOrderSolution.FindKthNumberByGenerateAndSort(upperBound, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindKthNumberByDepthFirstTraversal_LeetCodeExamples_ReturnsExpectedValue(
        int upperBound, int rank, int expected)
    {
        var actual = KthSmallestInLexicographicalOrderSolution.FindKthNumberByDepthFirstTraversal(upperBound, rank);

        Assert.Equal(expected, actual);
    }
}
