using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.RepeatedStringMatch;

// LeetCode 686. Repeated String Match: the minimum repeat count is never more than
// ceil(targetPattern.Text.Length / repeatedUnit.Text.Length) + 1 (the "+1" covers a
// match straddling a repetition boundary), so both strategies repeat `repeatedUnit` up
// to that many times and ask whether `targetPattern` occurs in the repeated string,
// returning the first repeat count that works.
//
// The two strategies differ only in the substring search each candidate is checked
// with - the BCL's own string.Contains, or this repo's KMP-based
// PrefixFunctionSearch. Each writes the two-candidate loop out itself, so the
// baseline never runs through code that could call the library's search
// (ARCHITECTURE 17.5). Both searches are ordinal, so neither answer depends on the
// machine's culture, and an empty pattern occurs in every candidate.
internal static class RepeatedStringMatchSolution
{
    // The textbook answer: BCL string.Contains, deliberately left as the arm
    // MinRepeatsByPrefixFunctionSearch is measured against.
    public static int MinRepeatsByStringContains(RepeatedUnit repeatedUnit, TargetPattern targetPattern)
    {
        var minRepeats = FewestRepeatsToCover(repeatedUnit, targetPattern);

        for (var repeats = minRepeats; repeats <= minRepeats + 1; repeats++)
        {
            var candidate = Repeated(repeatedUnit, repeats);

            if (candidate.Contains(targetPattern.Text, StringComparison.Ordinal))
            {
                return repeats;
            }
        }

        return LeetCodeAnswer.None;
    }

    // The same two candidates, each checked with this repo's KMP-based search, asked only
    // whether any occurrence exists rather than where they all are.
    public static int MinRepeatsByPrefixFunctionSearch(RepeatedUnit repeatedUnit, TargetPattern targetPattern)
    {
        var minRepeats = FewestRepeatsToCover(repeatedUnit, targetPattern);

        for (var repeats = minRepeats; repeats <= minRepeats + 1; repeats++)
        {
            var candidate = Repeated(repeatedUnit, repeats);

            if (PrefixFunctionSearch.FindAll(candidate, targetPattern.Text).Count > 0)
            {
                return repeats;
            }
        }

        return LeetCodeAnswer.None;
    }

    // The fewest repeats long enough to hold the pattern at all; one more covers a match
    // straddling a repetition boundary.
    private static int FewestRepeatsToCover(RepeatedUnit repeatedUnit, TargetPattern targetPattern) =>
        (int)Math.Ceiling((double)targetPattern.Text.Length / repeatedUnit.Text.Length);

    private static string Repeated(RepeatedUnit repeatedUnit, int repeats)
    {
        var repeatedSegments = Enumerable.Repeat(repeatedUnit.Text, repeats);

        return string.Concat(repeatedSegments);
    }

    // LC 686's two operands, named for the roles they play here rather than left as two
    // adjacent `string` positions a caller could hand over the wrong way round with the
    // compiler none the wiser. `repeatedUnit` is the unit being repeated and
    // `targetPattern` the pattern the repeated text is searched for - repeating the
    // target until it holds the unit is a different question, so a swap is a silently
    // wrong answer.
    internal readonly record struct RepeatedUnit(string Text);

    internal readonly record struct TargetPattern(string Text);
}
