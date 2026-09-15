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
    // Both searches are stateless, so one instance each serves every call and the
    // benchmark arms that measure these two methods allocate nothing to pick one.
    private static readonly ISubstringSearch OrdinalContains = new OrdinalContainsSearch();
    private static readonly ISubstringSearch PrefixFunctionContains = new PrefixFunctionContainsSearch();

    // The textbook answer: BCL string.Contains, deliberately left as the arm
    // MinRepeatsByPrefixFunctionSearch is measured against.
    public static int MinRepeatsByStringContains(RepeatedUnit a, TargetPattern b) =>
        MinRepeats(a, b, OrdinalContains);

    public static int MinRepeatsByPrefixFunctionSearch(RepeatedUnit a, TargetPattern b) =>
        MinRepeats(a, b, PrefixFunctionContains);

    // The one question the two arms answer differently: whether the pattern occurs
    // anywhere in one candidate repetition. Both of its inputs are named here, and this
    // is where the contract the bare form had nowhere to write goes - the search is
    // ordinal, so its answer does not depend on the machine's culture, and an empty
    // pattern occurs in every candidate.
    private interface ISubstringSearch
    {
        bool Occurs(string candidate, TargetPattern pattern);
    }

    // The repeat-count search itself, shared by both strategies so the only thing
    // they differ in is the substring check.
    private static int MinRepeats(RepeatedUnit a, TargetPattern b, ISubstringSearch search)
    {
        var minRepeats = (int)Math.Ceiling((double)b.Text.Length / a.Text.Length);

        for (var repeats = minRepeats; repeats <= minRepeats + 1; repeats++)
        {
            var repeatedSegments = Enumerable.Repeat(a.Text, repeats);
            var candidate = string.Concat(repeatedSegments);

            if (search.Occurs(candidate, b))
            {
                return repeats;
            }
        }

        return LeetCodeAnswer.None;
    }

    // The textbook arm: the BCL's own ordinal substring search.
    private sealed class OrdinalContainsSearch : ISubstringSearch
    {
        public bool Occurs(string candidate, TargetPattern pattern) =>
            candidate.Contains(pattern.Text, StringComparison.Ordinal);
    }

    // This repo's own KMP-based PrefixFunctionSearch, asked only whether any
    // occurrence exists rather than where they all are.
    private sealed class PrefixFunctionContainsSearch : ISubstringSearch
    {
        public bool Occurs(string candidate, TargetPattern pattern) =>
            PrefixFunctionSearch.FindAll(candidate, pattern.Text).Count > 0;
    }

    // LC 686's two operands, named for the roles they play here rather than left as two
    // adjacent `string` positions a caller could hand over the wrong way round with the
    // compiler none the wiser. `a` is the unit being repeated and `b` the pattern the
    // repeated text is searched for - repeating b until it holds a is a different
    // question, so a swap is a silently wrong answer.
    internal readonly record struct RepeatedUnit(string Text);

    internal readonly record struct TargetPattern(string Text);
}
