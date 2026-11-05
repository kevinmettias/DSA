using DSAExperimentation.LeetCode.Solutions.OrderlyQueue;

namespace DSAExperimentation.LeetCode.Solutions.Tests.OrderlyQueue;

// Harness only. Both strategies are OrderlyQueueSolution's - one table of cases,
// one theory per strategy, so a failure names the strategy that broke. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class OrderlyQueueSolutionTests
{
    public static TheoryData<string, int, string> Cases =>
        new()
        {
            // A single character has one rotation and one permutation.
            { "z", 1, "z" },
            // movablePrefixLength == 1 on a two-character string: "ba" -> "ab".
            { "ba", 1, "ab" },
            // The same string as LeetCode's first example, now with movablePrefixLength > 1: the full sort wins.
            { "cba", 3, "abc" },
            // movablePrefixLength == 1 where the best rotation is neither first nor last.
            { "bca", 1, "abc" },
            // movablePrefixLength == 2 is already the "every permutation" case, not a rotation case.
            { "dcab", 2, "abcd" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestStringByBruteForceRotations_Cases_ReturnsSmallestReachableString(
        string text, int movablePrefixLength, string expected)
    {
        var actual = OrderlyQueueSolution.SmallestStringByBruteForceRotations(text, movablePrefixLength);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallestStringBySuffixArray_Cases_ReturnsSmallestReachableString(
        string text, int movablePrefixLength, string expected)
    {
        var actual = OrderlyQueueSolution.SmallestStringBySuffixArray(text, movablePrefixLength);

        Assert.Equal(expected, actual);
    }
}
