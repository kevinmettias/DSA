using DSAExperimentation.LeetCode.CircularArrayLoop;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CircularArrayLoop;

// Harness only. Both strategies are CircularArrayLoopSolution's - this file just
// pins them to LeetCode's published examples.
public sealed class CircularArrayLoopTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [1, 1, 1, 1], true },
            { [2, -1, 1, 2, 2], true },
            { [-1, -2, -3, -4, -5, 6], false },
            { [1, -1, 1, -1], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasLoopByHashSetWalk_LeetCodeExamples_MatchesExpected(int[] nums, bool expected) =>
        Assert.Equal(expected, CircularArrayLoopSolution.HasLoopByHashSetWalk(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasLoopByLinkedListFloyd_LeetCodeExamples_MatchesExpected(int[] nums, bool expected) =>
        Assert.Equal(expected, CircularArrayLoopSolution.HasLoopByLinkedListFloyd(nums));
}
