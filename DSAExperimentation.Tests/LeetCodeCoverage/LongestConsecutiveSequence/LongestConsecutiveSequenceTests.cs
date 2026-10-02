using DSAExperimentation.LeetCode.LongestConsecutiveSequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestConsecutiveSequence;

// Harness only: both strategies live in LongestConsecutiveSequenceSolution -
// this file just pins them to LeetCode's published examples.
public sealed partial class LongestConsecutiveSequenceTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [100, 4, 200, 1, 3, 2], 4 },
            { [0, 3, 7, 2, 5, 8, 4, 6, 0, 1], 9 },
            { [], 0 },
            { [1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestConsecutiveBySetRunExpansion_LeetCodeExamples_ReturnsLongestRun(int[] nums, int expected) =>
        Assert.Equal(expected, LongestConsecutiveSequenceSolution.LongestConsecutiveBySetRunExpansion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestConsecutiveBySortedScan_LeetCodeExamples_ReturnsLongestRun(int[] nums, int expected) =>
        Assert.Equal(expected, LongestConsecutiveSequenceSolution.LongestConsecutiveBySortedScan(nums));

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they name the same length on every example - not merely that
    // each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestConsecutive_AgreeOnEveryExample(int[] nums, int expected) =>
        Assert.Equal(
            LongestConsecutiveSequenceSolution.LongestConsecutiveBySetRunExpansion(nums),
            LongestConsecutiveSequenceSolution.LongestConsecutiveBySortedScan(nums));
}
