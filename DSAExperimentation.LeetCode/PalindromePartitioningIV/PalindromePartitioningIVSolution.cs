using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.PalindromePartitioningIV;

// LeetCode 1745. Palindrome Partitioning IV: whether the text splits into exactly
// three non-empty contiguous substrings that are each a palindrome.
//
// Both strategies explore the same decision tree over the state
// (Position, PartitionsLeft) - "where the next piece starts, and how many pieces are
// still owed" - and price each candidate piece with the same two-pointer palindrome
// check. This is PalindromePartitioningIII's recurrence with the partition count
// pinned at three and the "cheapest repair" objective replaced by "does any split
// work at all", so they differ only in whether a state is ever re-solved:
//
// - CanPartitionIntoThreePalindromesByNaiveRecursion is the textbook backtracking
//   search with no cache, so a state reached along several distinct choice paths is
//   recomputed once per path. This is the "what you would write without this repo"
//   arm, deliberately plain BCL - it was previously only the benchmark's unasserted
//   baseline.
// - CanPartitionIntoThreePalindromesByMemoizedRecurrence drives the identical
//   recurrence through this repo's Memoizer, so each state is resolved exactly once
//   no matter how many first/second cut choices land on it.
internal static class PalindromePartitioningIVSolution
{
    // LC 1745 asks for exactly three pieces; the recurrence is otherwise general.
    private const int PartitionCount = 3;

    public static bool CanPartitionIntoThreePalindromesByNaiveRecursion(string text) =>
        CanSplit(text, 0, PartitionCount);

    public static bool CanPartitionIntoThreePalindromesByMemoizedRecurrence(string text)
        => Memoizer.Memoize<(int Position, int PartitionsLeft), bool>(
            (0, PartitionCount),
            new AnyPalindromicSplit(text));

    // The recurrence itself, named: a split works when some palindromic piece at this
    // position leaves a remainder that itself splits the remaining pieces owed.
    private sealed class AnyPalindromicSplit(string text) : IRecurrence<(int Position, int PartitionsLeft), bool>
    {
        public bool Replay(
            (int Position, int PartitionsLeft) state, IRecurrence<(int Position, int PartitionsLeft), bool> rest)
        {
            var (position, partitionsLeft) = state;

            if (partitionsLeft == 0)
            {
                return position == text.Length;
            }

            var lastEnd = LastEnd(text, partitionsLeft);

            for (var end = position + 1; end <= lastEnd; end++)
            {
                if (IsPalindrome(text, position, end - 1) && rest.Replay((end, partitionsLeft - 1), rest))
                {
                    return true;
                }
            }

            return false;
        }
    }

    private static bool CanSplit(string text, int position, int partitionsLeft)
    {
        if (partitionsLeft == 0)
        {
            return position == text.Length;
        }

        var lastEnd = LastEnd(text, partitionsLeft);

        for (var end = position + 1; end <= lastEnd; end++)
        {
            if (IsPalindrome(text, position, end - 1) && CanSplit(text, end, partitionsLeft - 1))
            {
                return true;
            }
        }

        return false;
    }

    // The last index this piece may end at and still leave one character for each of
    // the pieces still owed after it.
    private static int LastEnd(string text, int partitionsLeft) => text.Length - (partitionsLeft - 1);

    private static bool IsPalindrome(string text, int leftIndex, int rightIndex)
    {
        while (leftIndex < rightIndex)
        {
            if (text[leftIndex++] != text[rightIndex--])
            {
                return false;
            }
        }

        return true;
    }
}
