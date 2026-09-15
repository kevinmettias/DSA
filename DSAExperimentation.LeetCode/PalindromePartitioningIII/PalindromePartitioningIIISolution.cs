using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.PalindromePartitioningIII;

// LeetCode 1278. Palindrome Partitioning III: split s into exactly k non-empty
// contiguous substrings and change the fewest characters so every piece is a
// palindrome.
//
// Both strategies explore the same decision tree over the state
// (Position, PartitionsLeft) - "where the next piece starts, and how many pieces are
// still owed" - and price each candidate piece with the same two-pointer mismatch
// count. They differ only in whether that state is ever re-solved:
//
// - MinChangesByNaiveRecursion is the textbook recursion with no cache, so a state
//   reached along several distinct choice paths is recomputed once per path. This is
//   the "what you would write without this repo" arm, deliberately plain BCL.
// - MinChangesByMemoizedRecurrence drives the identical recurrence through this
//   repo's Memoizer (the PalindromePartitioningII precedent), so each state is
//   solved exactly once.
internal static class PalindromePartitioningIIISolution
{
    // Half of int.MaxValue: "no valid partition from here", chosen so it can absorb a
    // ChangesToPalindrome addition without overflowing before Math.Min discards it.
    private const int Unreachable = int.MaxValue / 2;

    public static int MinChangesByNaiveRecursion(string s, int k) => MinChangesFrom(s, 0, k);

    public static int MinChangesByMemoizedRecurrence(string s, int k)
        => Memoizer.Memoize<(int Position, int PartitionsLeft), int>(
            (0, k),
            new CheapestSplit(s));

    // The recurrence itself, named: every candidate next piece is priced by what it
    // costs to repair plus the cheapest split of what is left behind it.
    private sealed class CheapestSplit(string s) : IRecurrence<(int Position, int PartitionsLeft), int>
    {
        public int Replay(
            (int Position, int PartitionsLeft) state, IRecurrence<(int Position, int PartitionsLeft), int> rest)
        {
            var (position, partitionsLeft) = state;

            if (partitionsLeft == 0)
            {
                return position == s.Length ? 0 : Unreachable;
            }

            if (position == s.Length)
            {
                return Unreachable;
            }

            var best = Unreachable;
            var lastEnd = s.Length - (partitionsLeft - 1);

            for (var end = position + 1; end <= lastEnd; end++)
            {
                var candidate = PieceChanges(s, position, end) + rest.Replay((end, partitionsLeft - 1), rest);
                best = Math.Min(best, candidate);
            }

            return best;
        }
    }

    // What this candidate piece - s[position..end] - costs to turn into a palindrome.
    private static int PieceChanges(string s, int position, int end) => ChangesToPalindrome(s, position, end - 1);

    private static int MinChangesFrom(string s, int position, int partitionsLeft)
    {
        if (partitionsLeft == 0)
        {
            return position == s.Length ? 0 : Unreachable;
        }

        if (position == s.Length)
        {
            return Unreachable;
        }

        var best = Unreachable;
        var lastEnd = s.Length - (partitionsLeft - 1);

        for (var end = position + 1; end <= lastEnd; end++)
        {
            var candidate = ChangesToPalindrome(s, position, end - 1) +
                MinChangesFrom(s, end, partitionsLeft - 1);
            best = Math.Min(best, candidate);
        }

        return best;
    }

    // Characters that must change to make s[l..r] a palindrome: one per mismatched
    // pair walking inwards from both ends.
    private static int ChangesToPalindrome(string s, int l, int r)
    {
        var changes = 0;

        while (l < r)
        {
            if (s[l++] != s[r--])
            {
                changes++;
            }
        }

        return changes;
    }
}
