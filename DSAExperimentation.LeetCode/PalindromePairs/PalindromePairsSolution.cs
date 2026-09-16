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
                AddPairsAtCut((word, i), cut, indexOf, pairs);
            }
        }

        return pairs;
    }

    private static void AddPairsAtCut(
        (string Word, int Index) entry, int cut, HashMap<string, int> indexOf, List<(int, int)> pairs)
    {
        var prefix = entry.Word[..cut];
        var suffix = entry.Word[cut..];
        var suffixMatch = FindComplementIndex(indexOf, suffix);

        if (IsPalindrome(prefix) && IsAnotherWord(suffixMatch, entry.Index))
        {
            pairs.Add((suffixMatch, entry.Index));
        }

        var prefixMatch = FindComplementIndex(indexOf, prefix);

        if (HasPalindromicRightSide(entry.Word, cut, suffix) && IsAnotherWord(prefixMatch, entry.Index))
        {
            pairs.Add((entry.Index, prefixMatch));
        }
    }

    // Where the reversed complement of one side sits in the list, or -1 when no such
    // word is present.
    private static int FindComplementIndex(HashMap<string, int> indexOf, string side)
    {
        if (indexOf.TryGetValue(Reverse(side), out var match))
        {
            return match;
        }

        return -1;
    }

    // A pair needs two distinct words, so the complement found must be another one.
    private static bool IsAnotherWord(int matchIndex, int wordIndex)
        => matchIndex >= 0 && matchIndex != wordIndex;

    // The cut leaves a right-hand side, and that side already reads as a palindrome,
    // so a reversed complement on the left completes the pair.
    private static bool HasPalindromicRightSide(string word, int cut, string suffix)
        => cut != word.Length && IsPalindrome(suffix);

    private static bool IsPalindrome(string text)
    {
        var left = 0;
        var right = text.Length - 1;

        while (left < right)
        {
            if (text[left++] != text[right--])
            {
                return false;
            }
        }

        return true;
    }

    private static string Reverse(string text)
    {
        var chars = text.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}
