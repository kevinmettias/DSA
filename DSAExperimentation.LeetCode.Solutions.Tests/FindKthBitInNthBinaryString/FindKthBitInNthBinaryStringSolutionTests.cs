using DSAExperimentation.LeetCode.Solutions.FindKthBitInNthBinaryString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindKthBitInNthBinaryString;

// Harness only: both strategies live in FindKthBitInNthBinaryStringSolution and are
// asserted against the same cases, which cover all three cases of the bisection -
// the middle bit, a position left of it, and a mirrored-and-inverted position right
// of it.
public sealed partial class FindKthBitInNthBinaryStringSolutionTests
{
    public static TheoryData<int, int, char> Cases =>
        new()
        {
            { 1, 1, '0' },
            { 2, 1, '0' },
            { 2, 3, '1' },
            { 3, 4, '1' },
            { 4, 9, '0' },
            { 4, 15, '1' },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindKthBitByStringConstruction_Cases_ReturnsExpectedBit(
        int stringOrder, int bitPosition, char expected)
    {
        var actual = FindKthBitInNthBinaryStringSolution.FindKthBitByStringConstruction(stringOrder, bitPosition);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindKthBitByRecursiveBisection_Cases_ReturnsExpectedBit(
        int stringOrder, int bitPosition, char expected)
    {
        var actual = FindKthBitInNthBinaryStringSolution.FindKthBitByRecursiveBisection(stringOrder, bitPosition);

        Assert.Equal(expected, actual);
    }
}
