using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePartitioningIV;

// LeetCode 1745. Palindrome Partitioning IV: whether s splits into exactly
// three non-empty palindromic substrings. This repo's Memoizer drives the
// same (Position, PartitionsLeft) recursion PalindromePartitioningIIITests
// already uses (fixed here at 3 remaining partitions instead of a variable
// k), caching each state so the many different first/second cut choices that
// land on the same later position are resolved once instead of re-walked -
// IsPalindrome stays the same O(n) two-pointer helper PalindromePartitioningII/
// III both use to price a candidate substring.
public sealed partial class PalindromePartitioningIVTests
{
    [Theory]
    [InlineData("abcbdd", true)]
    [InlineData("bcbddxy", false)]
    public void CheckPartitioning_LeetCodeExamples_ReturnsWhetherThreeWaySplitExists(string s, bool expected)
        => Assert.Equal(expected, CheckPartitioning(s));

    private static bool CheckPartitioning(string s)
        => Memoizer.Memoize<(int Position, int PartitionsLeft), bool>((0, 3), (state, canSplit) =>
        {
            var (position, partitionsLeft) = state;

            if (partitionsLeft == 0)
            {
                return position == s.Length;
            }

            var lastEnd = s.Length - (partitionsLeft - 1);

            for (var end = position + 1; end <= lastEnd; end++)
            {
                if (IsPalindrome(s, position, end - 1) && canSplit((end, partitionsLeft - 1)))
                {
                    return true;
                }
            }

            return false;
        });

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
