using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.PrimePalindrome;

// LeetCode 866. Prime Palindrome: the smallest integer at or above the lower bound
// that is both a palindrome and prime.
//
// Two strategies, differing in what they enumerate:
//
// - SequentialScan walks every integer from the lower bound upwards and asks each
//   one both questions. It is the baseline - what you would write without this repo -
//   so its internals stay BCL (string reversal for the palindrome test, trial
//   division for primality).
// - PalindromeGeneration enumerates palindromes directly, building each one from its
//   first half: digits are collected front-to-back in this repo's own
//   DynamicArray<int>, then mirrored back onto the same array. Only palindromes are
//   ever trial-divided, so the long palindrome-sparse stretches cost nothing.
internal static class PrimePalindromeSolution
{
    private const int DecimalBase = 10;
    private const int SmallestPrime = 2;
    private const int OddDivisorStep = 2;
    private const long FirstOddDivisor = 3L;
    private const int MirrorStartOffset = 2;

    // Every prime palindrome below 12. No even-length palindrome other than 11 is
    // ever prime - each is a multiple of 11 - and the generator below only builds
    // odd-length candidates, so these have to be answered before it starts.
    private static readonly long[] SmallPrimePalindromes = [2, 3, 5, 7, 11];

    // Baseline: test every integer from the lower bound upwards for both properties.
    public static long SmallestPrimePalindromeBySequentialScan(int lowerBound)
    {
        var candidate = (long)lowerBound;

        // Stops at the first candidate that is both a palindrome and a prime.
        while (true)
        {
            if (IsPalindromeNumber(candidate) && IsPrime(candidate))
            {
                return candidate;
            }

            candidate++;
        }
    }

    private static bool IsPalindromeNumber(long value)
    {
        var text = value.ToString();
        var left = 0;
        var right = text.Length - 1;

        while (left < right)
        {
            if (text[left] != text[right])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }

    // Enumerate palindromes rather than integers, mirroring each candidate's first
    // half back onto itself in a DynamicArray<int>.
    public static long SmallestPrimePalindromeByPalindromeGeneration(int lowerBound)
    {
        foreach (var smallPrime in SmallPrimePalindromes)
        {
            if (lowerBound <= smallPrime)
            {
                return smallPrime;
            }
        }

        return GeneratePrimePalindrome(lowerBound);
    }

    private static long GeneratePrimePalindrome(int lowerBound)
    {
        var exponent = lowerBound.ToString().Length / AlgorithmConstants.HalvingFactor;
        var half = (int)Math.Pow(DecimalBase, exponent);

        // Stops at the first built palindrome >= the lower bound that is prime; half
        // only ever grows.
        while (true)
        {
            var candidate = BuildOddLengthPalindrome(half);

            if (candidate >= lowerBound && IsPrime(candidate))
            {
                return candidate;
            }

            half++;
        }
    }

    private static long BuildOddLengthPalindrome(int half)
    {
        var digits = new DynamicArray<int>();
        AppendHalfDigits(digits, half);
        MirrorDigits(digits);

        return ToDecimalValue(digits);
    }

    // The half's decimal digits, most significant first.
    private static void AppendHalfDigits(DynamicArray<int> digits, int half)
    {
        var remaining = half;

        while (remaining > 0)
        {
            digits.Insert(0, remaining % DecimalBase);
            remaining /= DecimalBase;
        }
    }

    // Everything but the centre digit, read back onto the end of the array so the
    // digits read the same in both directions.
    private static void MirrorDigits(DynamicArray<int> digits)
    {
        for (var i = digits.Count - MirrorStartOffset; i >= 0; i--)
        {
            digits.Add(digits.Get(i));
        }
    }

    private static long ToDecimalValue(DynamicArray<int> digits)
    {
        var value = 0L;

        for (var i = 0; i < digits.Count; i++)
        {
            value = value * DecimalBase + digits.Get(i);
        }

        return value;
    }

    private static bool IsPrime(long value)
    {
        if (value < SmallestPrime)
        {
            return false;
        }

        if (value % SmallestPrime == 0)
        {
            return value == SmallestPrime;
        }

        for (var divisor = FirstOddDivisor; divisor * divisor <= value; divisor += OddDivisorStep)
        {
            if (value % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }
}
