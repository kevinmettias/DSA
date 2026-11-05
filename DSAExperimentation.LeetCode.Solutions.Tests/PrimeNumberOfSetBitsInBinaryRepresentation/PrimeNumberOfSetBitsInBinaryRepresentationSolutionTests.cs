using DSAExperimentation.LeetCode.Solutions.PrimeNumberOfSetBitsInBinaryRepresentation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PrimeNumberOfSetBitsInBinaryRepresentation;

// Harness only: both strategies live in PrimeNumberOfSetBitsInBinaryRepresentationSolution
// and are asserted against the same cases.
public sealed partial class PrimeNumberOfSetBitsInBinaryRepresentationSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            { 3, 3, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPrimeSetBitsByTrialDivision_Cases_ReturnsExpectedCount(
        int left, int right, int expected)
    {
        var actual = PrimeNumberOfSetBitsInBinaryRepresentationSolution.CountPrimeSetBitsByTrialDivision(left, right);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPrimeSetBitsByPrecomputedSet_Cases_ReturnsExpectedCount(
        int left, int right, int expected)
    {
        var actual = PrimeNumberOfSetBitsInBinaryRepresentationSolution.CountPrimeSetBitsByPrecomputedSet(left, right);

        Assert.Equal(expected, actual);
    }
}
