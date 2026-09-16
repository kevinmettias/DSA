using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.PalindromePartitioningIII;

// LeetCode 1278. Palindrome Partitioning III: split the text into exactly
// partitionCount non-empty contiguous substrings and change the fewest characters so
// every piece is a palindrome.
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

    public static int MinChangesByNaiveRecursion(string text, int partitionCount) =>
        MinChangesFrom(text, 0, partitionCount);

    public static int MinChangesByMemoizedRecurrence(string text, int partitionCount)
        => Memoizer.Memoize<(int Position, int PartitionsLeft), int>(
            (0, partitionCount),
            new CheapestSplit(text));

    // The recurrence itself, named: every candidate next piece is priced by what it
    // costs to repair plus the cheapest split of what is left behind it.
    private sealed class CheapestSplit(string text) : IRecurrence<(int Position, int PartitionsLeft), int>
    {
        public int Replay(
            (int Position, int PartitionsLeft) state, IRecurrence<(int Position, int PartitionsLeft), int> rest)
        {
            var (position, partitionsLeft) = state;

            if (partitionsLeft == 0)
            {
                return position == text.Length ? 0 : Unreachable;
            }

            if (position == text.Length)
            {
                return Unreachable;
            }

            var best = Unreachable;
            var lastEnd = text.Length - (partitionsLeft - 1);

            for (var end = position + 1; end <= lastEnd; end++)
            {
                var candidate = PieceChanges(text, position, end) + rest.Replay((end, partitionsLeft - 1), rest);
                best = Math.Min(best, candidate);
            }

            return best;
        }
    }

    // What this candidate piece - text[position..end] - costs to turn into a palindrome.
    private static int PieceChanges(string text, int position, int end) =>
        ChangesToPalindrome(text, position, end - 1);

    private static int MinChangesFrom(string text, int position, int partitionsLeft)
    {
        if (partitionsLeft == 0)
        {
            return position == text.Length ? 0 : Unreachable;
        }

        if (position == text.Length)
        {
            return Unreachable;
        }

        var best = Unreachable;
        var lastEnd = text.Length - (partitionsLeft - 1);

        for (var end = position + 1; end <= lastEnd; end++)
        {
            var candidate = ChangesToPalindrome(text, position, end - 1) +
                MinChangesFrom(text, end, partitionsLeft - 1);
            best = Math.Min(best, candidate);
        }

        return best;
    }

    // Characters that must change to make text[leftIndex..rightIndex] a palindrome:
    // one per mismatched pair walking inwards from both ends.
    private static int ChangesToPalindrome(string text, int leftIndex, int rightIndex)
    {
        var changes = 0;

        while (leftIndex < rightIndex)
        {
            if (text[leftIndex++] != text[rightIndex--])
            {
                changes++;
            }
        }

        return changes;
    }
}
