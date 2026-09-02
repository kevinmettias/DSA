using DSAExperimentation.LeetCode.MostFrequentPrime;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MostFrequentPrime;

// Harness only: the directional walk and both primality strategies live in
// MostFrequentPrimeSolution - this file just pins them to LeetCode's
// published examples.
public sealed class MostFrequentPrimeTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 1], [9, 9], [1, 1]], 19 },
            { [[7]], -1 },
            { [[9, 7, 8], [4, 6, 5], [2, 8, 6]], 97 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MostFrequentPrimeByTrialDivision_LeetCodeExamples_ReturnsMostFrequentQualifyingPrime(
        int[][] mat, int expected) =>
        Assert.Equal(expected, MostFrequentPrimeSolution.MostFrequentPrimeByTrialDivision(mat));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MostFrequentPrimeBySieve_LeetCodeExamples_ReturnsMostFrequentQualifyingPrime(
        int[][] mat, int expected) =>
        Assert.Equal(expected, MostFrequentPrimeSolution.MostFrequentPrimeBySieve(mat));
}
