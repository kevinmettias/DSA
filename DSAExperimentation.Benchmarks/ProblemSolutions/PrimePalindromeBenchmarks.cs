using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Prime Palindrome (LC 866): sequentially trial-dividing every integer >= N for
// palindrome-ness and primality vs. generating only palindrome candidates -
// mirroring each one back onto itself in this repo's own DynamicArray<int> - and
// trial-dividing just those. N=999 lands on a large palindrome-sparse stretch (the
// next prime palindrome is 10301: no 4-digit palindrome is ever prime, since every
// even-length palindrome is a multiple of 11), so the sequential scan pays for
// ~9,300 candidates the generator never visits.
[MemoryDiagnoser]
public class PrimePalindromeBenchmarks
{
    [Params(13, 999)]
    public int N;

    [Benchmark(Baseline = true)]
    public long SequentialScan()
    {
        var candidate = (long)N;

        while (true)
        {
            if (IsPalindromeNumber(candidate) && IsPrime(candidate))
            {
                return candidate;
            }

            candidate++;
        }
    }

    [Benchmark]
    public long PalindromeGeneration() => SmallestPrimePalindrome(N);

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
