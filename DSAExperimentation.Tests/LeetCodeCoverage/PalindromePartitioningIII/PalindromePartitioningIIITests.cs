using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePartitioningIII;

// LeetCode 1278. Palindrome Partitioning III: this repo's Memoizer
// (PalindromePartitioningIITests precedent) drives the partition-count
// recursion over state (Position, PartitionsLeft), while a plain
// two-pointer helper (the same IsPalindrome shape PalindromePartitioningTests
// already uses, generalized here to count mismatches instead of returning a
// bool) prices each candidate substring's palindrome-repair cost.
public sealed partial class PalindromePartitioningIIITests
{
    private const int Unreachable = int.MaxValue / 2;

    [Theory]
    [InlineData("abc", 2, 1)]
    [InlineData("aabbc", 3, 0)]
    [InlineData("leetcode", 8, 0)]
    public void MinChanges_LeetCodeExamples_ReturnsMinimumCharacterChanges(string s, int k, int expected)
        => Assert.Equal(expected, MinChanges(s, k));

    private static int MinChanges(string s, int k)
        => Memoizer.Memoize<(int Position, int PartitionsLeft), int>((0, k), (state, changesFrom) =>
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
                var candidate = ChangesToPalindrome(s, position, end - 1) + changesFrom((end, partitionsLeft - 1));
                best = Math.Min(best, candidate);
            }

            return best;
        });

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
