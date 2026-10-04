using DSAExperimentation.LeetCode.CountAlmostEqualPairsII;

namespace DSAExperimentation.LeetCode.Tests.CountAlmostEqualPairsII;

// Harness only: both strategies live in CountAlmostEqualPairsIISolution - this file
// pins them to LeetCode's published examples, and the zero-padding both are handed to
// the digits it should produce.
public sealed partial class CountAlmostEqualPairsIISolutionTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1023, 2310, 2130, 213], 4 },
            { [1, 10, 100], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBoundedSwapBruteForce_LeetCodeExamples_ReturnsAlmostEqualPairCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountAlmostEqualPairsIISolution.CountByBoundedSwapBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBoundedSwapBacktrack_LeetCodeExamples_ReturnsAlmostEqualPairCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountAlmostEqualPairsIISolution.CountByBoundedSwapBacktrack(nums));

    // nums[i] < 10^7, so every number is left-filled with zeros to seven digits: 1 needs
    // six of them, 10 five and 100 four - the padding a swap moves digits into and out
    // of in LeetCode's second example.
    [Fact]
    public void Pad_LeetCodeSecondExample_LeftFillsEveryNumberToSevenDigits() =>
        Assert.Equal(["0000001", "0000010", "0000100"], CountAlmostEqualPairsIISolution.Pad([1, 10, 100]));
}
