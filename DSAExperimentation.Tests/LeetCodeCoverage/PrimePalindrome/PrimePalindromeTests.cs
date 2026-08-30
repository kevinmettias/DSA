using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrimePalindrome;

// LeetCode 866. Prime Palindrome: generate palindrome candidates directly from
// their first half - digits collected front-to-back in this repo's own
// DynamicArray<int>, then mirrored back onto the same array - instead of scanning
// every integer >= n, so only palindromes are ever trial-divided for primality.
public sealed class PrimePalindromeTests
{
    [Theory]
    [InlineData(1, 2L)]
    [InlineData(6, 7L)]
    [InlineData(8, 11L)]
    [InlineData(13, 101L)]
    [InlineData(100, 101L)]
    public void SmallestPrimePalindrome_Examples_ReturnsSmallestPrimePalindromeAtLeastN(int n, long expected)
        => Assert.Equal(expected, SmallestPrimePalindrome(n));

    private static long SmallestPrimePalindrome(int n)
    {
        if (n <= 2)
        {
            return 2;
        }

        if (n <= 3)
        {
            return 3;
        }

        if (n <= 5)
        {
            return 5;
        }

        if (n <= 7)
        {
            return 7;
        }

        // Every even-length palindrome is a multiple of 11, so 11 is the only
        // even-length prime palindrome - every candidate the loop below builds is
        // odd-length, which would otherwise skip straight over it.
        if (n <= 11)
        {
            return 11;
        }

        var exponent = n.ToString().Length / 2;
        var half = (int)Math.Pow(10, exponent);

        while (true)
        {
            var candidate = BuildOddLengthPalindrome(half);

            if (candidate >= n && IsPrime(candidate))
            {
                return candidate;
            }

            half++;
        }
    }

    private static long BuildOddLengthPalindrome(int half)
    {
        var digits = new DynamicArray<int>();
        var remaining = half;

        while (remaining > 0)
        {
            digits.Insert(0, remaining % 10);
            remaining /= 10;
        }

        for (var i = digits.Count - 2; i >= 0; i--)
        {
            digits.Add(digits.Get(i));
        }

        var value = 0L;
        for (var i = 0; i < digits.Count; i++)
        {
            value = value * 10 + digits.Get(i);
        }

        return value;
    }

    private static bool IsPrime(long value)
    {
        if (value < 2)
        {
            return false;
        }

        if (value % 2 == 0)
        {
            return value == 2;
        }

        for (var divisor = 3L; divisor * divisor <= value; divisor += 2)
        {
            if (value % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }
}
