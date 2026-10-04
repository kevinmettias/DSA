using DSAExperimentation.LeetCode.NthMagicalNumber;

namespace DSAExperimentation.LeetCode.Tests.NthMagicalNumber;

// Harness only: both strategies live in NthMagicalNumberSolution and are asserted
// against the same examples - LeetCode's two published ones, the case where the
// second factor is a multiple of the first (inclusion-exclusion subtracts the whole
// overlap), a coprime pair that interleaves both sequences, and the factor pair the
// benchmark measures.
public sealed partial class NthMagicalNumberSolutionTests
{
    public static TheoryData<int, int, int, int> Examples =>
        new()
        {
            { 1, 2, 3, 2 },
            { 4, 2, 3, 6 },
            { 4, 2, 4, 8 },
            { 5, 2, 4, 10 },
            { 7, 3, 5, 15 },
            { 6, 6, 10, 24 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthMagicalNumberByCountScan_LeetCodeExamples_ReturnsNthMultipleOfEitherFactor(
        int rank, int firstFactor, int secondFactor, int expected)
    {
        var actual = NthMagicalNumberSolution.NthMagicalNumberByCountScan(rank, firstFactor, secondFactor);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthMagicalNumberByBinarySearch_LeetCodeExamples_ReturnsNthMultipleOfEitherFactor(
        int rank, int firstFactor, int secondFactor, int expected)
    {
        var actual = NthMagicalNumberSolution.NthMagicalNumberByBinarySearch(rank, firstFactor, secondFactor);

        Assert.Equal(expected, actual);
    }

    // LeetCode's limits, n = 10^9 and a = b = 4 * 10^4: with one factor the magical
    // numbers are its multiples, so the nth is n * a = 4 * 10^13 - far past int, which
    // the search window has to reach before the answer is reduced modulo 1e9+7. Kept
    // off the shared examples because the count scan would walk all 4 * 10^13 of them.
    [Fact]
    public void NthMagicalNumberByBinarySearch_AnswerPastIntRange_ReturnsAnswerModuloPrime()
    {
        const int Rank = 1_000_000_000;
        const int Factor = 40_000;
        const long Modulus = 1_000_000_007;

        var actual = NthMagicalNumberSolution.NthMagicalNumberByBinarySearch(Rank, Factor, Factor);

        Assert.Equal((int)((long)Rank * Factor % Modulus), actual);
    }
}
