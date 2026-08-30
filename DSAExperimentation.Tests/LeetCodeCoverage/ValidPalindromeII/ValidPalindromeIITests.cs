namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidPalindromeII;

// LeetCode 680. Valid Palindrome II: the same two-pointer character scan
// ValidPalindromeTests (LC 125) already uses, extended to retry once with either
// pointer skipped past the first mismatch. Bare index arithmetic over the raw
// string - ValidPalindromeTests' own precedent that no stronger reusable repo
// primitive applies beyond ordinary sequence traversal holds here too.
public sealed partial class ValidPalindromeIITests
{
    [Fact]
    public void ValidPalindrome_AlreadyAPalindrome_ReturnsTrue() =>
        Assert.True(IsValidPalindrome("aba"));

    [Fact]
    public void ValidPalindrome_OneDeletionMakesItAPalindrome_ReturnsTrue() =>
        Assert.True(IsValidPalindrome("abca"));

    [Fact]
    public void ValidPalindrome_NoSingleDeletionCanFixIt_ReturnsFalse() =>
        Assert.False(IsValidPalindrome("abcdef"));

    private static bool IsValidPalindrome(string s)
    {
        var left = 0;
        var right = s.Length - 1;

        while (left < right)
        {
            if (s[left] != s[right])
            {
                return IsPalindromeRange(s, left + 1, right) || IsPalindromeRange(s, left, right - 1);
            }

            left++;
            right--;
        }

        return true;
    }

    private static bool IsPalindromeRange(string s, int left, int right)
    {
        while (left < right)
        {
            if (s[left] != s[right])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }
}
