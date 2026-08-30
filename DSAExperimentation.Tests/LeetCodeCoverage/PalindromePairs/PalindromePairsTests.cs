using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePairs;

// LeetCode 336. Palindrome Pairs: for every word and every split point, this repo's
// own HashMap<TKey,TValue> answers "does the exact reversed complement exist?" in
// O(1) - the same complement-lookup shape TwoSumTests already uses, applied to
// string prefixes/suffixes instead of numeric complements. O(n*k^2) total (k = max
// word length) instead of the O(n^2*k) brute force that concatenates and checks
// every ordered pair directly.
public sealed partial class PalindromePairsTests
{
    [Fact]
    public void FindPalindromePairs_ClassicExample_ReturnsAllValidPairs()
        => Assert.Equal(
            // "dcba"+"abcd", "abcd"+"dcba", "s"+"lls", "lls"+"sssll" (llssssll) all read the same forwards and backwards.
            new HashSet<(int, int)> { (0, 1), (1, 0), (3, 2), (2, 4) },
            FindPalindromePairs(["abcd", "dcba", "lls", "s", "sssll"]).ToHashSet());

    [Fact]
    public void FindPalindromePairs_ShortWords_ReturnsAllValidPairs()
        => Assert.Equal(
            new HashSet<(int, int)> { (0, 1), (1, 0) },
            FindPalindromePairs(["bat", "tab", "cat"]).ToHashSet());

    [Fact]
    public void FindPalindromePairs_EmptyStringWord_ReturnsPairsWithEmptyWord()
        => Assert.Equal(
            new HashSet<(int, int)> { (0, 1), (1, 0) },
            FindPalindromePairs(["a", ""]).ToHashSet());

    private static List<(int First, int Second)> FindPalindromePairs(string[] words)
    {
        var indexOf = new HashMap<string, int>();
        for (var i = 0; i < words.Length; i++)
        {
            indexOf.Set(words[i], i);
        }

        var pairs = new List<(int, int)>();
        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            for (var cut = 0; cut <= word.Length; cut++)
            {
                var prefix = word[..cut];
                var suffix = word[cut..];

                if (IsPalindrome(prefix)
                    && indexOf.TryGetValue(Reverse(suffix), out var suffixMatch)
                    && suffixMatch != i)
                {
                    pairs.Add((suffixMatch, i));
                }

                if (cut != word.Length
                    && IsPalindrome(suffix)
                    && indexOf.TryGetValue(Reverse(prefix), out var prefixMatch)
                    && prefixMatch != i)
                {
                    pairs.Add((i, prefixMatch));
                }
            }
        }

        return pairs;
    }

    private static bool IsPalindrome(string s)
    {
        var left = 0;
        var right = s.Length - 1;
        while (left < right)
        {
            if (s[left++] != s[right--])
            {
                return false;
            }
        }

        return true;
    }

    private static string Reverse(string s)
    {
        var chars = s.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}
