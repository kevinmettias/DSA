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
                if (j != i && ContainsNaive(new Haystack(words[j]), new Needle(words[i])))
                {
                    contained.Add(words[i]);
                    break;
                }
            }
        }

        return contained;
    }

    private static bool ContainsNaive(Haystack text, Needle pattern)
    {
        for (var start = 0; start + pattern.Text.Length <= text.Text.Length; start++)
        {
            if (MatchesAt(text, pattern, start))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchesAt(Haystack text, Needle pattern, int start)
    {
        for (var offset = 0; offset < pattern.Text.Length; offset++)
        {
            if (text.Text[start + offset] != pattern.Text[offset])
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

    // The two ends of a containment test, named for the roles they play here rather than
    // left as two adjacent `string` positions a caller could hand over the wrong way
    // round with the compiler none the wiser. The text is what is searched; the needle
    // is what is searched for - a one-directional relation, since the scan is bounded by
    // the needle's length and indexes the text by the needle's offset.
    private readonly record struct Haystack(string Text);

    private readonly record struct Needle(string Text);
}
