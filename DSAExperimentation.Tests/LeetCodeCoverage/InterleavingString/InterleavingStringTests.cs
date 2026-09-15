using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InterleavingString;

public sealed partial class InterleavingStringTests
{
    [Theory]
    [InlineData("aabcc", "dbbca", "aadbbcbcac", true)]
    [InlineData("aabcc", "dbbca", "aadbbbaccc", false)]
    public void IsInterleave_LeetCodeExamples_ReturnsExpected(string first, string second, string target, bool expected)
    {
        var actual = IsInterleave(first, second, target);
        Assert.Equal(expected, actual);
    }

    private static bool IsInterleave(string first, string second, string target)
    {
        if (first.Length + second.Length != target.Length) return false;
        return Memoizer.Memoize<(int First, int Second), bool>((0, 0), new CanBuild(first, second, target));
    }

    // The interleaving rule, named: the target is buildable when its next character
    // matches the next of either input and the rest stays buildable. The three strings
    // are what the decision is made against, so they arrive as the constructor's
    // parameters rather than as an ambient closure.
    private sealed class CanBuild(string first, string second, string target)
        : IRecurrence<(int First, int Second), bool>
    {
        public bool Replay((int First, int Second) state, IRecurrence<(int First, int Second), bool> rest)
        {
            var (i, j) = state; var k = i + j;
            return k == target.Length
                || (i < first.Length && first[i] == target[k] && rest.Replay((i + 1, j), rest))
                || (j < second.Length && second[j] == target[k] && rest.Replay((i, j + 1), rest));
        }
    }
}
