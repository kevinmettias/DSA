using DSAExperimentation.LeetCode.Solutions.CountGoodNumbers;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountGoodNumbers;

// Harness only. Both counting strategies are CountGoodNumbersSolution's - the
// repeated-multiplication arm that used to live untested as the benchmark baseline,
// and the exponentiation-by-squaring arm - pinned here to the two smallest lengths
// that separate the even-index and odd-index digit alphabets (length = 2 is 5 * 4,
// length = 3 is 5^2 * 4) and to length 5 (5^3 * 4^2). LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class CountGoodNumbersSolutionTests
{
    public static TheoryData<long, int> Cases =>
        new()
        {
            { 2, 20 },
            { 3, 100 },
            { 5, 2_000 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountGoodNumbersByRepeatedMultiplication_Cases_ReturnsExpectedCount(
        long length, int expected) =>
        Assert.Equal(expected, CountGoodNumbersSolution.CountGoodNumbersByRepeatedMultiplication(length));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountGoodNumbersByExponentiationBySquaring_Cases_ReturnsExpectedCount(
        long length, int expected) =>
        Assert.Equal(expected, CountGoodNumbersSolution.CountGoodNumbersByExponentiationBySquaring(length));
}
