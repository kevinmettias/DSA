using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.PalindromePartitioningIV;

// LeetCode 1745. Palindrome Partitioning IV: whether s splits into exactly three
// non-empty contiguous substrings that are each a palindrome.
//
// Both strategies explore the same decision tree over the state
// (Position, PartitionsLeft) - "where the next piece starts, and how many pieces are
// still owed" - and price each candidate piece with the same two-pointer palindrome
// check. This is PalindromePartitioningIII's recurrence with the partition count
// pinned at three and the "cheapest repair" objective replaced by "does any split
// work at all", so they differ only in whether a state is ever re-solved:
//
// - CheckPartitioningByNaiveRecursion is the textbook backtracking search with no
//   cache, so a state reached along several distinct choice paths is recomputed once
//   per path. This is the "what you would write without this repo" arm, deliberately
//   plain BCL - it was previously only the benchmark's unasserted baseline.
// - CheckPartitioningByMemoizedRecurrence drives the identical recurrence through
//   this repo's Memoizer, so each state is resolved exactly once no matter how many
//   first/second cut choices land on it.
internal static class PalindromePartitioningIVSolution
{
    // LC 1745 asks for exactly three pieces; the recurrence is otherwise general.
    private const int PartitionCount = 3;

    public static bool CheckPartitioningByNaiveRecursion(string s) => CanSplit(s, 0, PartitionCount);

    public static bool CheckPartitioningByMemoizedRecurrence(string s)
        => Memoizer.Memoize<(int Position, int PartitionsLeft), bool>(
            (0, PartitionCount),
            new AnyPalindromicSplit(s));

    // The recurrence itself, named: a split works when some palindromic piece at this
    // position leaves a remainder that itself splits the remaining pieces owed.
    private sealed class AnyPalindromicSplit(string s) : IRecurrence<(int Position, int PartitionsLeft), bool>
    {
        public bool Replay(
            (int Position, int PartitionsLeft) state, IRecurrence<(int Position, int PartitionsLeft), bool> rest)
        {
            var (position, partitionsLeft) = state;

            if (partitionsLeft == 0)
            {
                return position == s.Length;
            }

            var lastEnd = LastEnd(s, partitionsLeft);

            for (var end = position + 1; end <= lastEnd; end++)
            {
                if (IsPalindrome(s, position, end - 1) && rest.Replay((end, partitionsLeft - 1), rest))
                {
                    return true;
                }
            }

            return false;
        }
    }

    private static bool CanSplit(string s, int position, int partitionsLeft)
    {
        if (partitionsLeft == 0)
        {
            return position == s.Length;
        }

        var lastEnd = LastEnd(s, partitionsLeft);

        for (var end = position + 1; end <= lastEnd; end++)
        {
            if (IsPalindrome(s, position, end - 1) && CanSplit(s, end, partitionsLeft - 1))
            {
                return true;
            }
        }

        return false;
    }

    // The last index this piece may end at and still leave one character for each of
    // the pieces still owed after it.
    private static int LastEnd(string s, int partitionsLeft) => s.Length - (partitionsLeft - 1);

    private static bool IsPalindrome(string s, int l, int r)
    {
        while (l < r)
        {
            if (s[l++] != s[r--])
            {
                return false;
            }
        }

        return true;
    }
}
