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
    // The textbook arm the memoized recurrence is measured against: the same
    // skip/consume rule rolled forward over the source, carrying one rolling row of
    // target-prefix counts instead of a Memoizer entry per (source, target) pair.
    // O(source * target) time either way, but O(target) space here against the
    // Memoizer's per-state dictionary.
    public static int CountDistinctSubsequencesByIterativeTable(SourceText source, TargetPattern target)
    {
        var matches = new int[target.Pattern.Length + 1];
        matches[0] = 1;

        foreach (var character in source.Text)
        {
            // High to low so matches[consumed - 1] is still the previous source
            // row's count - that is what turns the 2D table into one rolling row.
            for (var consumed = target.Pattern.Length; consumed >= 1; consumed--)
            {
                if (character == target.Pattern[consumed - 1])
                {
                    matches[consumed] += matches[consumed - 1];
                }
            }
        }

        return matches[target.Pattern.Length];
    }

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
