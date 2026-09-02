using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ExtraCharactersInAString;

// LeetCode 2707. Extra Characters in a String: fewest leftover characters after
// segmenting s into dictionary words - the same Trie<bool> prefix-pruned
// segmentation WordBreakTests already proves out over this exact pair of
// primitives, minimizing a leftover count via Memoizer instead of returning a
// yes/no reachability.
public sealed partial class ExtraCharactersInAStringTests
{
    [Theory]
    [InlineData("leetscode", new[] { "leet", "code", "leetcode" }, 1)]
    [InlineData("sayhelloworld", new[] { "hello", "world" }, 3)]
    public void MinExtraChars_LeetCodeExamples_ReturnsFewestLeftoverCharacters(
        string s, string[] dictionary, int expected)
    {
        Assert.Equal(expected, MinExtraChars(s, dictionary));
    }

    private static int MinExtraChars(string s, string[] dictionary)
    {
        var trie = new Trie<bool>();
        foreach (var word in dictionary)
        {
            trie.Set(word, true);
        }

        return Memoizer.Memoize<int, int>(0, From);

        int From(int start, Func<int, int> min)
        {
            if (start == s.Length)
            {
                return 0;
            }

            var best = 1 + min(start + 1);

            for (var end = start + 1; end <= s.Length; end++)
            {
                var piece = s[start..end];
                if (!trie.HasPrefix(piece))
                {
                    break;
                }

                if (trie.HasKey(piece))
                {
                    best = Math.Min(best, min(end));
                }
            }

            return best;
        }
    }
}
