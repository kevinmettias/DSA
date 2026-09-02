using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Valid Palindrome II (LC 680): the naive "try deleting each character in turn, then
// re-check the whole result" approach (O(n^2): n candidate strings, each an O(n)
// build-plus-check) vs. the O(n) two-pointer scan that only ever retries the exact two
// candidate ranges adjacent to the first mismatch. Same "bare index arithmetic, no
// repo primitive" shape as ValidPalindromeBenchmarks (LC 125) - see
// ValidPalindromeIITests for why that holds here too. _s places two differing
// characters symmetrically off-center so the initial scan runs a genuine O(n)
// distance before finding the mismatch, instead of collapsing to O(1) at either end.
[MemoryDiagnoser]
public class ValidPalindromeIIBenchmarks
{
    private const int OffsetDivisor = 3;

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup() => _s = BuildInput(Length);

    [Benchmark(Baseline = true)]
    public bool TryEachSingleDeletion()
    {
        if (IsPalindromeRange(_s, 0, _s.Length - 1))
        {
            return true;
        }

        for (var skip = 0; skip < _s.Length; skip++)
        {
            var candidate = _s.Remove(skip, 1);

            if (IsPalindromeRange(candidate, 0, candidate.Length - 1))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool MismatchSkipTwoPointer()
    {
        var left = 0;
        var right = _s.Length - 1;

        while (left < right)
        {
            if (_s[left] != _s[right])
            {
                return IsPalindromeRange(_s, left + 1, right) || IsPalindromeRange(_s, left, right - 1);
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

    private static string BuildInput(int length)
    {
        var chars = new char[length];
        Array.Fill(chars, 'a');

        var mid1 = length / OffsetDivisor;
        var mid2 = length - 1 - mid1;
        chars[mid1] = 'b';
        chars[mid2] = 'c';

        return new string(chars);
    }
}
