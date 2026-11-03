using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.Solutions.MaximumRepeatingSubstring;

// LeetCode 1668. Maximum Repeating Substring: the largest k for which word repeated
// k times is a substring of sequence (0 when word does not occur at all).
//
// Both strategies grow a candidate word+word+... one repeat at a time and stop at
// the first repeat count the candidate no longer occurs at, so the answer is the
// last count that did - the same growing-candidate shape RepeatedStringMatch uses
// for LC 686, counting up from k = 0 until a candidate fails instead of searching
// for the first candidate that succeeds.
//
// The two strategies differ only in the substring search each candidate is checked
// with - the BCL's own string.Contains, or this repo's KMP-based
// PrefixFunctionSearch. Each writes the short walk out itself, so the baseline never
// runs through code that could call the library's search (ARCHITECTURE 17.5).
internal static class MaximumRepeatingSubstringSolution
{

    // The textbook answer: the BCL's own ordinal string.Contains, deliberately left as the
    // arm MaxRepeatingByPrefixFunctionSearch is measured against. The length guard is what
    // makes the loop terminate: a candidate longer than sequence can never occur in it.
    public static int MaxRepeatingByStringContains(Haystack sequence, RepeatedWord word)
    {
        var repeats = 0;
        var candidate = word.Text;

        while (candidate.Length <= sequence.Text.Length && sequence.Text.Contains(candidate, StringComparison.Ordinal))
        {
            repeats++;
            candidate += word.Text;
        }

        return repeats;
    }

    // The same walk, each candidate checked with this repo's KMP-based search.
    public static int MaxRepeatingByPrefixFunctionSearch(Haystack sequence, RepeatedWord word)
    {
        var repeats = 0;
        var candidate = word.Text;

        while (candidate.Length <= sequence.Text.Length && HasPrefixFunctionMatch(sequence, candidate))
        {
            repeats++;
            candidate += word.Text;
        }

        return repeats;
    }

    // All this arm needs is whether the search matched anything: the occurrence count is
    // discarded, so the comparison stays a yes/no.
    private static bool HasPrefixFunctionMatch(Haystack sequence, string candidate) =>
        PrefixFunctionSearch.FindAll(sequence.Text, candidate).Count > 0;

    // The two sides of the repeat-count walk, named for the roles they play here rather
    // than left as two adjacent `string` positions a caller could hand over the wrong
    // way round with the compiler none the wiser. The haystack is the string searched;
    // the repeated word is the block whose repetitions are counted in it - and the
    // question is one-directional, since asking how often the sequence repeats inside
    // the word is a different question with a different answer.
    internal readonly record struct Haystack(string Text);

    internal readonly record struct RepeatedWord(string Text);
}
