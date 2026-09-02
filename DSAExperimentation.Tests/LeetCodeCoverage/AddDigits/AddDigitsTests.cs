using DSAExperimentation.LeetCode.AddDigits;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddDigits;

// Harness only. Both strategies are AddDigitsSolution's - this file just pins them
// to LeetCode's published examples.
public sealed class AddDigitsTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 38, 2 },
            { 0, 0 },
            { 9, 9 },
            { 9999, 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddDigitsByArithmetic_LeetCodeExamples_ReturnsDigitalRoot(int num, int expected) =>
        Assert.Equal(expected, AddDigitsSolution.AddDigitsByArithmetic(num));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddDigitsByStack_LeetCodeExamples_ReturnsDigitalRoot(int num, int expected) =>
        Assert.Equal(expected, AddDigitsSolution.AddDigitsByStack(num));
}
