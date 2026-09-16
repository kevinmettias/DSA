using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.PalindromePartitioningII;

// LeetCode 132. Palindrome Partitioning II: the minimum number of cuts so every
// remaining substring is a palindrome.
//
// MinPiecesFromSuffix memoizes "fewest palindrome pieces covering s[start..]" via
// this repo's Memoizer, minimizing one piece plus the same rule applied past every
// palindromic prefix starting at start; covering the whole string in one fewer cut
// than pieces gives the answer. Only one strategy exists here - the original benchmark's two
// [Benchmark] arms were both compile-smoke placeholders (`=> 1`), not a second real
// approach to reconcile against.
internal static class PalindromePartitioningIISolution
{
    public static int MinCutByMemoizedSuffixRecurrence(string text) =>
        Memoizer.Memoize<int, int>(0, new MinPiecesFromSuffix(text)) - 1;

    // The recurrence itself, named: one piece for every palindromic prefix of the
    // remaining suffix, plus the fewest pieces covering whatever follows it.
    private sealed class MinPiecesFromSuffix(string s) : IRecurrence<int, int>
    {
        public int Replay(int start, IRecurrence<int, int> rest)
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
                    best = Math.Min(best, 1 + rest.Replay(end + 1, rest));
                }
            }

            return best;
        }
    }

    private static bool IsPalindrome(string text, int left, int right)
    {
        while (left < right)
        {
            if (text[left++] != text[right--])
            {
                return false;
            }
        }

        return true;
    }
}
