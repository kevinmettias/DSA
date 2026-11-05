using DSAExperimentation.LeetCode.Solutions.NumberOfDigitOne;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfDigitOne;

// Harness only. Both strategies are NumberOfDigitOneSolution's - this file
// just pins them to a single-digit case
// and a small n whose own digits repeat '1' (11 contributes two extra 1's on
// top of the ones already counted from 1 and 10).
public sealed partial class NumberOfDigitOneSolutionTests
{
    public static TheoryData<int, long> Cases =>
        new()
        {
            { 1, 1 },
            { 11, 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountDigitOneByBruteForceScan_Cases_ReturnsOccurrencesOfDigitOne(
        int upperBound, long expected) =>
        Assert.Equal(expected, NumberOfDigitOneSolution.CountDigitOneByBruteForceScan(upperBound));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountDigitOneByDigitPositionTally_Cases_ReturnsOccurrencesOfDigitOne(
        int upperBound, long expected) =>
        Assert.Equal(expected, NumberOfDigitOneSolution.CountDigitOneByDigitPositionTally(upperBound));
}
