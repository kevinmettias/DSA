using DSAExperimentation.LeetCode.KthSmallestInLexicographicalOrder;

namespace DSAExperimentation.LeetCode.Tests.KthSmallestInLexicographicalOrder;

// Harness only. Both strategies are KthSmallestInLexicographicalOrderSolution's -
// this file just pins them to LeetCode's published examples plus the first/last
// elements of example 1's lexicographical order.
public sealed partial class KthSmallestInLexicographicalOrderSolutionTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            // LeetCode examples 1 and 2.
            { 13, 2, 10 },
            { 1, 1, 1 },

            // Example 1's order is [1, 10, 11, 12, 13, 2, 3, ..., 9]: it opens on 1 and
            // closes on 9.
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
