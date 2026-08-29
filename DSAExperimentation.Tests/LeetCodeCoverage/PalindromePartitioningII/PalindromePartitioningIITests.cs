using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePartitioningII;

public sealed partial class PalindromePartitioningIITests
{
    [Theory]
    [InlineData("aab", 1)]
    [InlineData("a", 0)]
    public void MinCut_LeetCodeExamples_ReturnsMinimumCuts(string s, int expected) => Assert.Equal(expected, MinCut(s));
    private static int MinCut(string s) { return Memoizer.Memoize<int, int>(0, CutFrom) - 1; int CutFrom(int start, Func<int, int> cut) { if (start == s.Length) return 0; var best = int.MaxValue / 2; for (var end = start; end < s.Length; end++) if (IsPalindrome(s, start, end)) best = Math.Min(best, 1 + cut(end + 1)); return best; } }
    private static bool IsPalindrome(string s, int l, int r) { while (l < r) if (s[l++] != s[r--]) return false; return true; }
}
