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

    // The textbook answer: BCL string.Contains, deliberately left as the arm
    // MaxRepeatingByPrefixFunctionSearch is measured against.
    public static int MaxRepeatingByStringContains(string sequence, string word) =>
        MaxRepeating(sequence, word, static (text, candidate) => text.Contains(candidate, StringComparison.Ordinal));

    public static int MaxRepeatingByPrefixFunctionSearch(string sequence, string word) =>
        MaxRepeating(sequence, word, static (text, candidate) => PrefixFunctionSearch.FindAll(text, candidate).Count > 0);

    // The repeat-count walk itself, shared by both strategies so the only thing
    // they differ in is the substring check. The length guard is what makes the
    // loop terminate: a candidate longer than sequence can never occur in it.
    private static int MaxRepeating(string sequence, string word, Func<string, string, bool> contains)
    {
        var repeats = 0;
        var candidate = EmptyCandidate;

        while (true)
        {
            var next = candidate + word;

            if (next.Length > sequence.Length || !contains(sequence, next))
            {
                return repeats;
            }

            candidate = next;
            repeats++;
        }
    }
}
