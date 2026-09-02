using DSAExperimentation.LeetCode.LongestConsecutiveSequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestConsecutiveSequence;

// Harness only: the run-expansion strategy lives in
// LongestConsecutiveSequenceSolution - this file just pins it to LeetCode's
// published examples.
public sealed class LongestConsecutiveSequenceTests
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
}
