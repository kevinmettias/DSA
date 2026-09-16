using DSAExperimentation.LeetCode.FindTheNthValueAfterKSeconds;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheNthValueAfterKSeconds;

// Harness only. Both strategies are FindTheNthValueAfterKSecondsSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class FindTheNthValueAfterKSecondsTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 4, 5, 56 },
            { 5, 3, 35 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValueAfterKSecondsByBruteForce_LeetCodeExamples_ReturnsLastValueModuloLargePrime(
        int arrayLength, int secondCount, int expected)
    {
        var actual = FindTheNthValueAfterKSecondsSolution.ValueAfterKSecondsByBruteForce(
            arrayLength, secondCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValueAfterKSecondsByModularBinomial_LeetCodeExamples_ReturnsLastValueModuloLargePrime(
        int arrayLength, int secondCount, int expected)
    {
        var actual = FindTheNthValueAfterKSecondsSolution.ValueAfterKSecondsByModularBinomial(
            arrayLength, secondCount);

        Assert.Equal(expected, actual);
    }
}
