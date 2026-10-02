using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.PalindromePartitioningII;

// LeetCode 132. Palindrome Partitioning II: the minimum number of cuts so every
// remaining substring is a palindrome.
//
// Two strategies sit here. MinCutByMemoizedSuffixRecurrence memoizes "fewest
// palindrome pieces covering s[start..]" via this repo's Memoizer, minimizing one
// piece plus the same rule applied past every palindromic prefix starting at start;
// covering the whole string in one fewer cut than pieces gives the answer.
// MinCutByIterativeDynamicProgramming rolls that cut recurrence forward bottom up,
// finding the palindromic substrings by expanding around every centre, so it neither
// re-scans a substring to test it nor pays a Memoizer dictionary probe per state -
// and it drops the memoized arm's extra palindrome-rescan factor. The original
// benchmark's two [Benchmark] arms were both compile-smoke placeholders (`=> 1`),
// not a second real approach; these are.
internal static class PalindromePartitioningIISolution
{
    // The textbook arm the memoized suffix recurrence is measured against: a
    // bottom-up cut table whose entries are relaxed once per palindrome found by
    // expanding around each of the text's centres. O(n^2) time, O(n) space, against
    // the memoized arm's re-scan of every candidate substring.
    public static int MinCutByIterativeDynamicProgramming(string text)
    {
        var cuts = new int[text.Length + 1];
        for (var start = 0; start <= text.Length; start++)
        {
            cuts[start] = start - 1;
        }

        for (var centre = 0; centre < text.Length; centre++)
        {
            // Odd-length palindromes centred on one character, then even-length ones
            // centred between two; each expands outward until it stops matching, and
            // the best cut count at the far end absorbs the one at the near end.
            for (int left = centre, right = centre;
                left >= 0 && right < text.Length && text[left] == text[right];
                left--, right++)
            {
                cuts[right + 1] = Math.Min(cuts[right + 1], cuts[left] + 1);
            }

            for (int left = centre, right = centre + 1;
                left >= 0 && right < text.Length && text[left] == text[right];
                left--, right++)
            {
                cuts[right + 1] = Math.Min(cuts[right + 1], cuts[left] + 1);
            }
        }

        return cuts[text.Length];
    }

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
