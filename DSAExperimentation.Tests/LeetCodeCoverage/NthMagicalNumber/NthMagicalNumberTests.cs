using DSAExperimentation.LeetCode.NthMagicalNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NthMagicalNumber;

// Harness only: both strategies live in NthMagicalNumberSolution and are asserted
// against the same examples - LeetCode's two published ones, the case where the
// second factor is a multiple of the first (inclusion-exclusion subtracts the whole
// overlap), a coprime pair that interleaves both sequences, and the factor pair the
// benchmark measures.
public sealed class NthMagicalNumberTests
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
}
