using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrefixAndSuffixSearch;

// LeetCode 745. Prefix and Suffix Search: every (prefix, suffix) substring pair of
// each word is inserted as one combined "prefix#suffix" key into this repo's own
// HashMap<string,int>, later words overwriting earlier ones on a shared key - so a
// hit always reports the largest matching word index, exactly the tie-break the
// problem wants - and f(prefix, suffix) itself becomes a single O(1)-average lookup
// instead of a per-query scan.
public sealed partial class PrefixAndSuffixSearchTests
{
    [Fact]
    public void Filter_ClassicExample_ReturnsIndexOfMatchingWord()
    {
        var filter = new WordFilter(["apple"]);

        var matchingIndex = filter.Search("a", "e");
        Assert.Equal(0, matchingIndex);

        var nonMatchingIndex = filter.Search("b", "e");
        Assert.Equal(-1, nonMatchingIndex);
    }

    [Fact]
    public void Filter_MultipleWordsShareAMatch_ReturnsLargestIndex()
    {
        var filter = new WordFilter(["apple", "orange", "apricot"]);

        var largestSharedMatchIndex = filter.Search("ap", "t");
        Assert.Equal(2, largestSharedMatchIndex);

        var uniqueMatchIndex = filter.Search("app", "e");
        Assert.Equal(0, uniqueMatchIndex);

        var nonMatchingIndex = filter.Search("or", "t");
        Assert.Equal(-1, nonMatchingIndex);
    }

    private sealed class WordFilter
    {
        private readonly HashMap<string, int> _indexByPrefixAndSuffix = new();

        public WordFilter(string[] words)
        {
            for (var index = 0; index < words.Length; index++)
            {
                var word = words[index];

                for (var prefixLength = 0; prefixLength <= word.Length; prefixLength++)
                {
                    for (var suffixLength = 0; suffixLength <= word.Length; suffixLength++)
                    {
                        var key = word[..prefixLength] + "#" + word[(word.Length - suffixLength)..];
                        _indexByPrefixAndSuffix.Set(key, index);
                    }
                }
            }
        }

        public int Search(string prefix, string suffix)
            => _indexByPrefixAndSuffix.TryGetValue(prefix + "#" + suffix, out var index) ? index : -1;
    }
}
