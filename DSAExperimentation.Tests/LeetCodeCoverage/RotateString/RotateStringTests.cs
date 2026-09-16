using DSAExperimentation.LeetCode.RotateString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RotateString;

// Harness only. Both strategies are RotateStringSolution's - the restart-on-
// mismatch scan and the KMP failure-function search - pinned here to LeetCode's
// published examples plus the length mismatch and zero-shift edges the original
// coverage asserted.
public sealed partial class RotateStringTests
{
    public static TheoryData<RotationExample> Examples =>
        new()
        {
            { new RotationExample(Source: "abcde", Goal: "cdeab", Expected: true) },
            { new RotationExample(Source: "abcde", Goal: "abced", Expected: false) },
            { new RotationExample(Source: "abc", Goal: "abcd", Expected: false) },
            { new RotationExample(Source: "abcde", Goal: "abcde", Expected: true) },
            { new RotationExample(Source: "abcde", Goal: "eabcd", Expected: true) },
            { new RotationExample(Source: "a", Goal: "a", Expected: true) },
            { new RotationExample(Source: "a", Goal: "b", Expected: false) },
            { new RotationExample(Source: "abab", Goal: "baba", Expected: true) },
            { new RotationExample(Source: "aa", Goal: "aaa", Expected: false) },
            { new RotationExample(Source: "aaaab", Goal: "aabaa", Expected: true) },
            { new RotationExample(Source: "aaaab", Goal: "aaabb", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanRotateByNaiveScan_LeetCodeExamples_ReturnsWhetherGoalIsARotation(RotationExample example)
    {
        var actual = RotateStringSolution.CanRotateByNaiveScan(
            new RotateStringSolution.RotationSource(example.Source),
            new RotateStringSolution.RotationGoal(example.Goal));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanRotateByPrefixFunction_LeetCodeExamples_ReturnsWhetherGoalIsARotation(RotationExample example)
    {
        var actual = RotateStringSolution.CanRotateByPrefixFunction(
            new RotateStringSolution.RotationSource(example.Source),
            new RotateStringSolution.RotationGoal(example.Goal));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the string to rotate, the candidate rotation, and whether
    // the second is a rotation of the first. Source and goal are both `string` and
    // the ask is not symmetric, so the row names the roles rather than leaving two
    // adjacent positions the compiler would accept either way round.
    public readonly record struct RotationExample(string Source, string Goal, bool Expected);
}
