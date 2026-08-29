namespace DSAExperimentation.Tests.LeetCodeCoverage.WordBreakII;

public sealed class WordBreakIITests
{
    [Fact]
    public void WordBreak_MemoizedSuffixSearch_ReturnsAllSentences()
    {
        var result = WordBreak("catsanddog", ["cat", "cats", "and", "sand", "dog"]);
        Assert.Equal(["cat sand dog", "cats and dog"], result.OrderBy(s => s).ToArray());
    }

    private static IList<string> WordBreak(string s, IList<string> wordDict)
    {
        var words = wordDict.ToHashSet(); var memo = new Dictionary<int, List<string>>();
        List<string> From(int start)
        {
            if (memo.TryGetValue(start, out var cached)) return cached;
            if (start == s.Length) return [string.Empty];
            var result = new List<string>();
            for (var end = start + 1; end <= s.Length; end++)
            {
                var word = s[start..end]; if (!words.Contains(word)) continue;
                foreach (var suffix in From(end)) result.Add(suffix.Length == 0 ? word : word + " " + suffix);
            }
            return memo[start] = result;
        }
        return From(0);
    }
}
