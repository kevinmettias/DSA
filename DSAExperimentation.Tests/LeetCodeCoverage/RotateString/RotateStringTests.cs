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
        var canRotate = CanRotate("abcde", "cdeab");

        Assert.True(canRotate);
    }

    [Fact]
    public void CanRotate_GoalIsNotRotationOfS_ReturnsFalse()
    {
        var canRotate = CanRotate("abcde", "abced");

        Assert.False(canRotate);
    }

    [Fact]
    public void CanRotate_DifferentLengths_ReturnsFalse()
    {
        var canRotate = CanRotate("abc", "abcd");

        Assert.False(canRotate);
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
