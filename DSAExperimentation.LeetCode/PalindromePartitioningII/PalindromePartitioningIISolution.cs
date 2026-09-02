using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.PalindromePartitioningII;

// LeetCode 132. Palindrome Partitioning II: the minimum number of cuts so every
// remaining substring is a palindrome.
//
// MinPiecesFrom(start) memoizes "fewest palindrome pieces covering s[start..]" via
// this repo's Memoizer, minimizing 1 + MinPiecesFrom(end + 1) over every palindromic
// prefix starting at start; covering the whole string in one fewer cut than pieces
// gives the answer. Only one strategy exists here - the original benchmark's two
// [Benchmark] arms were both compile-smoke placeholders (`=> 1`), not a second real
// approach to reconcile against.
internal static class PalindromePartitioningIISolution
{
    public static int MinCutByMemoizedSuffixRecurrence(string s) =>
        Memoizer.Memoize<int, int>(0, (start, minPieces) => MinPiecesFrom(s, start, minPieces)) - 1;

    private static int MinPiecesFrom(string s, int start, Func<int, int> minPieces)
    {
        if (start == s.Length)
        {
            return 0;
        }

        var best = int.MaxValue / 2;

        for (var end = start; end < s.Length; end++)
        {
            if (IsPalindrome(s, start, end))
            {
                best = Math.Min(best, 1 + minPieces(end + 1));
            }
        }

        return best;
    }

    private static bool IsPalindrome(string s, int left, int right)
    {
        while (left < right)
        {
            if (s[left++] != s[right--])
            {
                return false;
            }
        }

        return true;
    }
}
