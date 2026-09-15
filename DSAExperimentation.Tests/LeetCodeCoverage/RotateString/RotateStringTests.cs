using DSAExperimentation.LeetCode.RotateString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RotateString;

// Harness only. Both strategies are RotateStringSolution's - the restart-on-
// mismatch scan and the KMP failure-function search - pinned here to LeetCode's
// published examples plus the length mismatch and zero-shift edges the original
// coverage asserted.
public sealed class RotateStringTests
{
    public static TheoryData<string, string, bool> Examples =>
        new()
        {
            { "abcde", "cdeab", true },
            { "abcde", "abced", false },
            { "abc", "abcd", false },
            { "abcde", "abcde", true },
            { "abcde", "eabcd", true },
            { "a", "a", true },
            { "a", "b", false },
            { "abab", "baba", true },
            { "aa", "aaa", false },
            { "aaaab", "aabaa", true },
            { "aaaab", "aaabb", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanRotateByNaiveScan_LeetCodeExamples_ReturnsWhetherGoalIsARotation(
        string s, string goal, bool expected) =>
        Assert.Equal(
            expected,
            RotateStringSolution.CanRotateByNaiveScan(
                new RotateStringSolution.RotationSource(s),
                new RotateStringSolution.RotationGoal(goal)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanRotateByPrefixFunction_LeetCodeExamples_ReturnsWhetherGoalIsARotation(
        string s, string goal, bool expected) =>
        Assert.Equal(
            expected,
            RotateStringSolution.CanRotateByPrefixFunction(
                new RotateStringSolution.RotationSource(s),
                new RotateStringSolution.RotationGoal(goal)));
}
