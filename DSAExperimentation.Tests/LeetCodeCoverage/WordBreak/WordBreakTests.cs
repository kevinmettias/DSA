using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordBreak;

public sealed partial class WordBreakTests
{
    [Theory]
    [InlineData("leetcode", new[] { "leet", "code" }, true)]
    [InlineData("catsandog", new[] { "cats", "dog", "sand", "and", "cat" }, false)]
    public void WordBreak_LeetCodeExamples_ReturnsWhetherSegmentable(string s, string[] words, bool expected)
        => Assert.Equal(expected, CanBreak(s, words));

    private static bool CanBreak(string s, string[] words)
    {
        var trie = new Trie<bool>(); foreach (var word in words) trie.Set(word, true);
        return Memoizer.Memoize<int, bool>(0, From);
        bool From(int start, Func<int, bool> can)
        {
            if (start == s.Length) return true;
            for (var end = start + 1; end <= s.Length; end++)
            {
                var piece = s[start..end];
                if (!trie.HasPrefix(piece)) break;
                if (trie.HasKey(piece) && can(end)) return true;
            }
            return false;
        }
    }
}
