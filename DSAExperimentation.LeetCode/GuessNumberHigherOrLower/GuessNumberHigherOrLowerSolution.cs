using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.GuessNumberHigherOrLower;

// LeetCode 374. Guess Number Higher or Lower: LeetCode's guess(num) oracle returns
// -1/1/0 for "pick is lower/higher/equal to num". This repo models the oracle as a
// `pick` threshold rather than a live judge API - the same shape
// FirstBadVersionSolution already uses for isBadVersion - so both strategies only ever
// ask "how does pick compare to this candidate?".
internal static class GuessNumberHigherOrLowerSolution
{
    // The textbook baseline: probe every candidate from 1 upward until guess() reports
    // a match - O(n) worst case. Deliberately BCL-only; the oracle check is the only
    // thing either strategy is allowed to know about the input's shape.
    public static int GuessNumberByLinearScan(int n, int pick)
    {
        for (var candidate = 1; candidate <= n; candidate++)
        {
            if (Guess(candidate, pick) == 0)
            {
                return candidate;
            }
        }

        return LeetCodeAnswer.None;
    }

    private static int Guess(int candidate, int pick) => pick.CompareTo(candidate);

    // This repo's own BinarySearch.Find over a virtual sequence of the candidates
    // 1..n - the sequence is never materialized, n can be as large as 2^31 - 1 without
    // allocating anything - routed through guess() via a custom IComparer, finding pick
    // in O(log n) probes instead of an O(n) linear scan.
    public static int GuessNumberByBinarySearch(int n, int pick)
    {
        var sequence = new NumberLineSequence(n);
        var comparer = new GuessComparer(pick);

        return BinarySearch.Find<int, NumberLineSequence>(sequence, target: 0, comparer)!.Value + 1;
    }

    // Get(index) is the 1-based candidate itself.
    private readonly struct NumberLineSequence(int length) : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => index + 1;
    }

    // Compare(candidate, _) must return negative when the search should move right
    // (candidate too low) and positive when it should move left (candidate too high) -
    // exactly -guess(candidate), since guess() returns 1 when candidate is too low
    // (guess higher) and -1 when too high (guess lower). The second Find parameter is
    // unused: guess() is the sole oracle, the same way FirstBadVersion never touches a
    // real target value either. A witness for this problem alone.
    private sealed class GuessComparer(int pick) : IComparer<int>
    {
        public int Compare(int candidate, int target) => -Guess(candidate, pick);
    }
}
