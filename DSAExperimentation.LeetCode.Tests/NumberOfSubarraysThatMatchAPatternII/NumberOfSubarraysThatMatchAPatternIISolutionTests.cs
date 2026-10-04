using DSAExperimentation.LeetCode.NumberOfSubarraysThatMatchAPatternII;

namespace DSAExperimentation.LeetCode.Tests.NumberOfSubarraysThatMatchAPatternII;

// Harness only. Both strategies live in
// NumberOfSubarraysThatMatchAPatternIISolution - this file just pins them to
// LeetCode's published examples (identical to 3034's, since 3036 restates
// the same problem at a larger bound).
public sealed partial class NumberOfSubarraysThatMatchAPatternIISolutionTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5, 6], [1, 1], 4 },
            { [1, 4, 4, 1, 3, 5, 5, 3], [1, 0, -1], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMatchesByBruteForce_LeetCodeExamples_ReturnsSubarrayCount(int[] nums, int[] pattern, int expected)
    {
        var actual = NumberOfSubarraysThatMatchAPatternIISolution.CountMatchesByBruteForce(nums, pattern);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMatchesByZFunction_LeetCodeExamples_ReturnsSubarrayCount(
        int[] nums, int[] pattern, int expected)
    {
        var actual = NumberOfSubarraysThatMatchAPatternIISolution.CountMatchesByZFunction(nums, pattern);

        Assert.Equal(expected, actual);
    }

    // Down, level, up between consecutive elements become '0', '1', '2'.
    [Fact]
    public void EncodeDiffs_ConsecutiveSigns_BecomeOneSymbolEach() =>
        Assert.Equal("2102", NumberOfSubarraysThatMatchAPatternIISolution.EncodeDiffs([1, 4, 4, 1, 3]));

    // A pattern entry -1, 0 or 1 lands on the same symbol as the sign it stands for.
    [Fact]
    public void EncodePattern_Entries_UseTheSameAlphabetAsTheSigns() =>
        Assert.Equal("210", NumberOfSubarraysThatMatchAPatternIISolution.EncodePattern([1, 0, -1]));
}
