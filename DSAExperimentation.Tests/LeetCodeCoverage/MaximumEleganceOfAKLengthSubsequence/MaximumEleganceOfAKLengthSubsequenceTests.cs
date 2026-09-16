using DSAExperimentation.LeetCode.MaximumEleganceOfAKLengthSubsequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumEleganceOfAKLengthSubsequence;

// Harness only: the algorithms live in
// MaximumEleganceOfAKLengthSubsequenceSolution. One test method per strategy
// over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed class MaximumEleganceOfAKLengthSubsequenceTests
{
    public static TheoryData<int[][], int, long> Examples =>
        new()
        {
            { [[3, 2], [5, 1], [10, 1]], 2, 17 },
            { [[3, 1], [3, 1], [2, 2], [5, 3]], 3, 19 },
            { [[1, 1], [2, 1], [3, 1]], 3, 7 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumEleganceByBcl_LeetCodeExamples_ReturnsMaxElegance(
        int[][] items, int subsequenceLength, long expected)
    {
        var actual = MaximumEleganceOfAKLengthSubsequenceSolution.MaximumEleganceByBcl(items, subsequenceLength);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumEleganceByRepoPrimitives_LeetCodeExamples_ReturnsMaxElegance(
        int[][] items, int subsequenceLength, long expected)
    {
        var actual = MaximumEleganceOfAKLengthSubsequenceSolution.MaximumEleganceByRepoPrimitives(items, subsequenceLength);

        Assert.Equal(expected, actual);
    }
}
