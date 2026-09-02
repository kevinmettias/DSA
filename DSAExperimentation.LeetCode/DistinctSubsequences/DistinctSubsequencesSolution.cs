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
    public static int NumDistinctByMemoizedRecursion(string source, string target)
    {
        return Memoizer.Memoize<(int Source, int Target), int>((0, 0), Ways);

        int Ways((int Source, int Target) state, Func<(int Source, int Target), int> ways)
        {
            var (i, j) = state;

            if (j == target.Length)
            {
                return 1;
            }

            if (i == source.Length)
            {
                return 0;
            }

            var total = ways((i + 1, j));

            if (source[i] == target[j])
            {
                total += ways((i + 1, j + 1));
            }

            return total;
        }
    }
}
