using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.RepeatedStringMatch;

// LeetCode 686. Repeated String Match: the minimum repeat count is never more than
// ceil(b.Length / a.Length) + 1 (the "+1" covers a match straddling a repetition
// boundary), so both strategies repeat `a` up to that many times and ask whether `b`
// occurs in the repeated string, returning the first repeat count that works.
//
// The two strategies differ only in the substring search each candidate is checked
// with - the BCL's own string.Contains, or this repo's KMP-based
// PrefixFunctionSearch.
internal static class RepeatedStringMatchSolution
{
    // The textbook answer: BCL string.Contains, deliberately left as the arm
    // MinRepeatsByPrefixFunctionSearch is measured against.
    public static int MinRepeatsByStringContains(string a, string b) =>
        MinRepeats(a, b, static (candidate, pattern) => candidate.Contains(pattern, StringComparison.Ordinal));

    public static int MinRepeatsByPrefixFunctionSearch(string a, string b) =>
        MinRepeats(a, b, static (candidate, pattern) => PrefixFunctionSearch.FindAll(candidate, pattern).Count > 0);

    // The repeat-count search itself, shared by both strategies so the only thing
    // they differ in is the substring check.
    private static int MinRepeats(string a, string b, Func<string, string, bool> contains)
    {
        var minRepeats = (int)Math.Ceiling((double)b.Length / a.Length);

        for (var repeats = minRepeats; repeats <= minRepeats + 1; repeats++)
        {
            var repeatedSegments = Enumerable.Repeat(a, repeats);
            var candidate = string.Concat(repeatedSegments);

            if (contains(candidate, b))
            {
                return repeats;
            }
        }

        return LeetCodeAnswer.None;
    }
}
