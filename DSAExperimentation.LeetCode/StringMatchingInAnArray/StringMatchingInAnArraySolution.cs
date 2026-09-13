using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.StringMatchingInAnArray;

// LeetCode 1408. String Matching in an Array: return every word that is a substring
// of some other word in the array. Both strategies run the same all-pairs sweep -
// each word against every other word, kept on the first containment found - and
// differ only in how one containment test is answered.
//
// Words are returned in input order. LC 1408 accepts any order, but input order is
// what the array-shaped sweep produces naturally and what the examples show.
internal static class StringMatchingInAnArraySolution
{
    // The textbook baseline: a raw char-index double loop per pair, restarting the
    // comparison from the pattern's beginning on every mismatch - O(text * pattern)
    // in the worst case. Deliberately written without this repo's primitives, and
    // without string.Contains, so it is the naive scan the KMP arm is measured
    // against.
    public static List<string> FindContainedWordsByNaiveScan(string[] words)
    {
        var contained = new List<string>();

        for (var i = 0; i < words.Length; i++)
        {
            for (var j = 0; j < words.Length; j++)
            {
                if (j != i && ContainsNaive(words[j], words[i]))
                {
                    contained.Add(words[i]);
                    break;
                }
            }
        }

        return contained;
    }

    private static bool ContainsNaive(string text, string pattern)
    {
        for (var start = 0; start + pattern.Length <= text.Length; start++)
        {
            if (MatchesAt(text, pattern, start))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchesAt(string text, string pattern, int start)
    {
        for (var offset = 0; offset < pattern.Length; offset++)
        {
            if (text[start + offset] != pattern[offset])
            {
                return false;
            }
        }

        return true;
    }

    // This repo's own PrefixFunctionSearch answers each containment test: KMP's
    // failure-function walk never re-scans a character of the text, giving
    // O(text + pattern) per pair instead of the naive scan's product.
    public static List<string> FindContainedWordsByPrefixFunctionSearch(string[] words)
    {
        var contained = new List<string>();

        for (var i = 0; i < words.Length; i++)
        {
            for (var j = 0; j < words.Length; j++)
            {
                if (j != i && PrefixFunctionSearch.FindAll(words[j], words[i]).Count > 0)
                {
                    contained.Add(words[i]);
                    break;
                }
            }
        }

        return contained;
    }
}
