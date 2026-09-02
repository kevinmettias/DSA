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
        return Memoizer.Memoize<(int First, int Second), bool>((0, 0), CanBuild);
        bool CanBuild((int First, int Second) state, Func<(int First, int Second), bool> build)
        {
            var (i, j) = state; var k = i + j;
            return k == target.Length
                || (i < first.Length && first[i] == target[k] && build((i + 1, j)))
                || (j < second.Length && second[j] == target[k] && build((i, j + 1)));
        }
    }
}
