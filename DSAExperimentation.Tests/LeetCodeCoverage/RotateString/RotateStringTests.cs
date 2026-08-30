using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RotateString;

// LeetCode 796. Rotate String: goal is a rotation of s iff they're the same length
// and goal occurs somewhere inside s+s - checked here with this repo's own KMP
// PrefixFunctionSearch instead of a naive substring scan.
public sealed partial class RotateStringTests
{
    [Fact]
    public void CanRotate_GoalIsRotationOfS_ReturnsTrue()
    {
        Assert.True(CanRotate("abcde", "cdeab"));
    }

    [Fact]
    public void CanRotate_GoalIsNotRotationOfS_ReturnsFalse()
    {
        Assert.False(CanRotate("abcde", "abced"));
    }

    [Fact]
    public void CanRotate_DifferentLengths_ReturnsFalse()
    {
        Assert.False(CanRotate("abc", "abcd"));
    }

    private static bool CanRotate(string s, string goal)
    {
        if (s.Length != goal.Length)
        {
            return false;
        }

        var doubled = s + s;
        return PrefixFunctionSearch.FindAll(doubled, goal).Count > 0;
    }
}
