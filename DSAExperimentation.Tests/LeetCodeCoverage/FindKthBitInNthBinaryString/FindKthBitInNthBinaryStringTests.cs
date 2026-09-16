using DSAExperimentation.LeetCode.FindKthBitInNthBinaryString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindKthBitInNthBinaryString;

// Harness only: both strategies live in FindKthBitInNthBinaryStringSolution and are
// asserted against the same examples, which cover all three cases of the bisection -
// the middle bit, a position left of it, and a mirrored-and-inverted position right
// of it.
public sealed class FindKthBitInNthBinaryStringTests
{
    public static TheoryData<int, int, char> Examples =>
        new()
        {
            { 1, 1, '0' },
            { 2, 1, '0' },
            { 2, 3, '1' },
            { 3, 1, '0' },
            { 4, 11, '1' },
            { 3, 4, '1' },
            { 3, 5, '0' },
            { 4, 9, '0' },
            { 4, 15, '1' },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindKthBitByStringConstruction_LeetCodeExamples_ReturnsExpectedBit(int n, int k, char expected)
    {
        var actual = FindKthBitInNthBinaryStringSolution.FindKthBitByStringConstruction(n, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindKthBitByRecursiveBisection_LeetCodeExamples_ReturnsExpectedBit(int n, int k, char expected)
    {
        var actual = FindKthBitInNthBinaryStringSolution.FindKthBitByRecursiveBisection(n, k);

        Assert.Equal(expected, actual);
    }
}
