using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.MaximumRepeatingSubstring;

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
// PrefixFunctionSearch.
internal static class MaximumRepeatingSubstringSolution
{
    private const string EmptyCandidate = "";

    // Both searches are stateless, so one instance each serves every call and neither
    // benchmark arm allocates anything to pick one.
    private static readonly ISubstringSearch OrdinalSearch = new OrdinalContainsSearch();
    private static readonly ISubstringSearch PrefixSearch = new PrefixFunctionPresenceSearch();

    // The textbook answer: BCL string.Contains, deliberately left as the arm
    // MaxRepeatingByPrefixFunctionSearch is measured against.
    public static int MaxRepeatingByStringContains(Haystack sequence, RepeatedWord word) =>
        MaxRepeating(sequence, word, OrdinalSearch);

    public static int MaxRepeatingByPrefixFunctionSearch(Haystack sequence, RepeatedWord word) =>
        MaxRepeating(sequence, word, PrefixSearch);

    // The one question the two strategies answer differently - does this candidate occur
    // in this sequence at all - with both sides named, so a call cannot hand the sequence
    // and the candidate over the wrong way round. The count of occurrences is not part of
    // the question; a search that finds one is enough.
    private interface ISubstringSearch
    {
        bool Occurs(Haystack sequence, RepeatedWord candidate);
    }

    // The repeat-count walk itself, shared by both strategies so the only thing
    // they differ in is the substring check. The length guard is what makes the
    // loop terminate: a candidate longer than sequence can never occur in it.
    private static int MaxRepeating(Haystack sequence, RepeatedWord word, ISubstringSearch contains)
    {
        var repeats = 0;
        var candidate = EmptyCandidate;

        // Stops at the first repeat count whose candidate is longer than sequence or no longer occurs in it.
        while (true)
        {
            var next = candidate + word.Text;

            if (next.Length > sequence.Text.Length || !contains.Occurs(sequence, new RepeatedWord(next)))
            {
                return repeats;
            }

            candidate = next;
            repeats++;
        }
    }

    // The BCL's own ordinal substring test.
    private sealed class OrdinalContainsSearch : ISubstringSearch
    {
        public bool Occurs(Haystack sequence, RepeatedWord candidate) =>
            sequence.Text.Contains(candidate.Text, StringComparison.Ordinal);
    }

    // The repo's KMP-based search, where all this arm needs is whether it matched
    // anything: the occurrence count is discarded, so the comparison stays a yes/no.
    private sealed class PrefixFunctionPresenceSearch : ISubstringSearch
    {
        public bool Occurs(Haystack sequence, RepeatedWord candidate) =>
            PrefixFunctionSearch.FindAll(sequence.Text, candidate.Text).Count > 0;
    }

    // The two sides of the repeat-count walk, named for the roles they play here rather
    // than left as two adjacent `string` positions a caller could hand over the wrong
    // way round with the compiler none the wiser. The haystack is the string searched;
    // the repeated word is the block whose repetitions are counted in it - and the
    // question is one-directional, since asking how often the sequence repeats inside
    // the word is a different question with a different answer.
    internal readonly record struct Haystack(string Text);

    internal readonly record struct RepeatedWord(string Text);
}
