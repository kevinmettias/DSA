using DSAExperimentation.LeetCode.NumberOfDigitOne;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfDigitOne;

// Harness only. Both strategies are NumberOfDigitOneSolution's - this file
// just pins them to LeetCode's published examples, plus a single-digit case
// and a small n whose own digits repeat '1' (11 contributes two extra 1's on
// top of the ones already counted from 1 and 10).
public sealed class NumberOfDigitOneTests
{
    public static TheoryData<int, long> Examples =>
        new()
        {
            { 13, 6 },
            { 0, 0 },
            { 100, 21 },
            { 1, 1 },
            { 11, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDigitOneByBruteForceScan_LeetCodeExamples_ReturnsOccurrencesOfDigitOne(
        int upperBound, long expected) =>
        Assert.Equal(expected, NumberOfDigitOneSolution.CountDigitOneByBruteForceScan(upperBound));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDigitOneByDigitPositionTally_LeetCodeExamples_ReturnsOccurrencesOfDigitOne(
        int upperBound, long expected) =>
        Assert.Equal(expected, NumberOfDigitOneSolution.CountDigitOneByDigitPositionTally(upperBound));
}
