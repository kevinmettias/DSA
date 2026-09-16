using DSAExperimentation.LeetCode.CountValidSequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountValidSequences;

// Harness only. The stars-and-bars derivation and both nCr strategies are
// CountValidSequencesSolution's; this file just pins them to LeetCode's
// published examples.
public sealed partial class CountValidSequencesTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 5, 3, 3 },
            { 3, 2, 2 },
            { 5, 5, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByDirectBinomial_LeetCodeExamples_ReturnsCountOfEvenProductSequences(
        int targetSum, int length, int expected)
    {
        var actual = CountValidSequencesSolution.CountByDirectBinomial(targetSum, length);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByPrecomputedFactorials_LeetCodeExamples_ReturnsCountOfEvenProductSequences(
        int targetSum, int length, int expected)
    {
        var actual = CountValidSequencesSolution.CountByPrecomputedFactorials(targetSum, length);
        Assert.Equal(expected, actual);
    }
}
