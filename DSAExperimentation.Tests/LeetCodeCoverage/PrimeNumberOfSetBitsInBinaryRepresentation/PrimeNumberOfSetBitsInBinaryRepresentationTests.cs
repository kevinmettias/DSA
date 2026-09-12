using DSAExperimentation.LeetCode.PrimeNumberOfSetBitsInBinaryRepresentation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrimeNumberOfSetBitsInBinaryRepresentation;

// Harness only: both strategies live in PrimeNumberOfSetBitsInBinaryRepresentationSolution
// and are asserted against the same examples.
public sealed class PrimeNumberOfSetBitsInBinaryRepresentationTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 6, 10, 4 },
            { 10, 15, 5 },
            { 3, 3, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPrimeSetBitsByTrialDivision_LeetCodeExamples_ReturnsExpectedCount(
        int left, int right, int expected) =>
        Assert.Equal(
            expected,
            PrimeNumberOfSetBitsInBinaryRepresentationSolution.CountPrimeSetBitsByTrialDivision(left, right));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPrimeSetBitsByPrecomputedSet_LeetCodeExamples_ReturnsExpectedCount(
        int left, int right, int expected) =>
        Assert.Equal(
            expected,
            PrimeNumberOfSetBitsInBinaryRepresentationSolution.CountPrimeSetBitsByPrecomputedSet(left, right));
}
