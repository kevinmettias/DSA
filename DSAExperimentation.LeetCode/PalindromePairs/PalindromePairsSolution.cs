using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PalindromePairs;

// LeetCode 336. Palindrome Pairs: for every ordered pair of distinct words, does
// words[i] + words[j] read the same forwards and backwards?
//
// FindPairsByBruteForce checks every ordered pair directly by concatenating and
// scanning, O(n^2*k). FindPairsByHashMapComplementLookup instead asks, for every
// prefix/suffix split of one word, whether the exact reversed complement is a
// distinct word in the list - the same complement-lookup shape TwoSum uses, applied
// to string prefixes/suffixes instead of numeric complements - using this repo's own
// HashMap<TKey,TValue> for the O(1) lookup, in O(n*k^2) (k = max word length).
internal static class PalindromePairsSolution
{
    // The textbook O(n^2*k) answer: try every ordered pair, concatenate, and scan
    // for a palindrome. Deliberately the naive baseline the complement lookup below
    // has to justify itself against.
    public static List<(int First, int Second)> FindPairsByBruteForce(IReadOnlyList<string> words)
    {
        var pairs = new List<(int, int)>();

        for (var i = 0; i < words.Count; i++)
        {
            for (var j = 0; j < words.Count; j++)
            {
                if (i != j && IsPalindrome(words[i] + words[j]))
                {
                    pairs.Add((i, j));
                }
            }
        }

        return pairs;
    }

    // For every word and every split point, does the exact reversed complement of
    // one side exist elsewhere in the list, with the other side already a
    // palindrome? One O(1) HashMap lookup per split answers it.
    public static List<(int First, int Second)> FindPairsByHashMapComplementLookup(IReadOnlyList<string> words)
    {
        var indexOf = new HashMap<string, int>();

        for (var i = 0; i < words.Count; i++)
        {
            indexOf.Set(words[i], i);
        }

        var pairs = new List<(int, int)>();

        for (var i = 0; i < words.Count; i++)
        {
            var word = words[i];

            for (var cut = 0; cut <= word.Length; cut++)
            {
                AddPairsAtCut(word, i, cut, indexOf, pairs);
            }
        }

        return pairs;
    }

    private static void AddPairsAtCut(
        string word, int i, int cut, HashMap<string, int> indexOf, List<(int, int)> pairs)
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
