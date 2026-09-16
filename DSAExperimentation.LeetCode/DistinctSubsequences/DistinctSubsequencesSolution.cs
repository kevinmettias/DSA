using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.DistinctSubsequences;

// LeetCode 115. Distinct Subsequences: how many distinct subsequences of source
// equal target, matched characters keeping their original left-to-right order.
//
// State (i, j) - how many characters of source/target have been consumed - is
// memoized so overlapping suffixes are only counted once: at every source
// character there is always the "skip it" branch, plus a "consume it" branch
// exactly when it matches the next unconsumed target character.
internal static class DistinctSubsequencesSolution
{
    public static int CountDistinctSubsequencesByMemoizedRecursion(SourceText source, TargetPattern target) =>
        Memoizer.Memoize<(int Source, int Target), int>((0, 0), new MatchesFromConsumedPrefixes(source, target));

    // The recurrence, as a named type: a fully consumed target is one match, a fully
    // consumed source with target left is none, and otherwise every source character
    // is skipped, and also consumed when it matches the next unconsumed target one.
    private sealed class MatchesFromConsumedPrefixes(SourceText source, TargetPattern target)
        : IRecurrence<(int Source, int Target), int>
    {
        public int Replay((int Source, int Target) state, IRecurrence<(int Source, int Target), int> rest)
        {
            var (i, j) = state;

            if (j == target.Pattern.Length)
            {
                return 1;
            }

            if (i == source.Text.Length)
            {
                return 0;
            }

            var total = rest.Replay((i + 1, j), rest);

            if (source.Text[i] == target.Pattern[j])
            {
                total += rest.Replay((i + 1, j + 1), rest);
            }

            return total;
        }
    }
}
