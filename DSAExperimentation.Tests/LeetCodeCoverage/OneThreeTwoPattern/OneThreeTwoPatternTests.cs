using DSAExperimentation.LeetCode.OneThreeTwoPattern;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OneThreeTwoPattern;

// Harness only. Both strategies are OneThreeTwoPatternSolution's - this file just
// pins them to LeetCode's published examples.
public sealed class OneThreeTwoPatternTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [1, 2, 3, 4], false },
            { [3, 1, 4, 2], true },
            { [-1, 3, 2, 0], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPatternByBruteForce_LeetCodeExamples_MatchesExpected(int[] nums, bool expected) =>
        Assert.Equal(expected, OneThreeTwoPatternSolution.HasPatternByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPatternByMonotonicStack_LeetCodeExamples_MatchesExpected(int[] nums, bool expected) =>
        Assert.Equal(expected, OneThreeTwoPatternSolution.HasPatternByMonotonicStack(nums));
}
