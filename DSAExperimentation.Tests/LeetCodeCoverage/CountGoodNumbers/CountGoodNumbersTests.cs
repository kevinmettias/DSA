using DSAExperimentation.LeetCode.CountGoodNumbers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountGoodNumbers;

// Harness only. Both counting strategies are CountGoodNumbersSolution's - the
// repeated-multiplication arm that used to live untested as the benchmark baseline,
// and the exponentiation-by-squaring arm - pinned here to LeetCode's published
// examples plus the two smallest lengths that separate the even-index and
// odd-index digit alphabets (length = 2 is 5 * 4, length = 3 is 5^2 * 4).
public sealed class CountGoodNumbersTests
{
    public static TheoryData<long, int> Examples =>
        new()
        {
            { 1, 5 },
            { 2, 20 },
            { 3, 100 },
            { 4, 400 },
            { 5, 2_000 },
            { 50, 564_908_303 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodNumbersByRepeatedMultiplication_LeetCodeExamples_ReturnsExpectedCount(
        long length, int expected) =>
        Assert.Equal(expected, CountGoodNumbersSolution.CountGoodNumbersByRepeatedMultiplication(length));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodNumbersByExponentiationBySquaring_LeetCodeExamples_ReturnsExpectedCount(
        long length, int expected) =>
        Assert.Equal(expected, CountGoodNumbersSolution.CountGoodNumbersByExponentiationBySquaring(length));
}
