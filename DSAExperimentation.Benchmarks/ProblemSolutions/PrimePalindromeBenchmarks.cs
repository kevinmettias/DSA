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
    private const int DecimalBase = 10;
    private const int SmallestPrime = 2;
    private const int OddDivisorStep = 2;
    private const long FirstOddDivisor = 3L;
    private const int HalfLengthDivisor = 2;
    private const int MirrorStartOffset = 2;

    // Every prime palindrome below 12: no two-digit palindrome besides 11 is prime,
    // since every even-length palindrome is a multiple of 11.
    private static readonly long[] SmallPrimePalindromes = [2, 3, 5, 7, 11];

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
        foreach (var smallPrime in SmallPrimePalindromes)
        {
            if (n <= smallPrime)
            {
                return smallPrime;
            }
        }

        return GeneratePrimePalindrome(n);
    }

    private static long GeneratePrimePalindrome(int n)
    {
        var exponent = n.ToString().Length / HalfLengthDivisor;
        var half = (int)Math.Pow(DecimalBase, exponent);

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
            digits.Insert(0, remaining % DecimalBase);
            remaining /= DecimalBase;
        }

        for (var i = digits.Count - MirrorStartOffset; i >= 0; i--)
        {
            digits.Add(digits.Get(i));
        }

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
