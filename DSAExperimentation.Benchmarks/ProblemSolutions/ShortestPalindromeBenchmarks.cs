using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shortest Palindrome (LC 214): the O(n^2) naive "try every prefix length, double-ended
// palindrome check" baseline vs. the O(n) approach that reuses this repo's own
// PrefixFunctionSearch.ComputeFailureFunction over s + '#' + reverse(s). Random lowercase
// letters give s no long palindromic prefix, so both strategies are forced through nearly
// their full worst-case scan instead of an early exit making the naive version look
// artificially competitive.
[MemoryDiagnoser]
public class ShortestPalindromeBenchmarks
{
    private const int AlphabetSize = 26;
    private const string FailureFunctionSeparator = "#";

    [Params(200, 2_000)]
    public int Length;

    private string _value = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(0, AlphabetSize));
        }

        _value = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string NaivePrefixScan()
    {
        for (var end = _value.Length; end > 0; end--)
        {
            if (IsPalindromePrefix(_value, end))
            {
                var suffix = _value[end..];
                return new string(suffix.Reverse().ToArray()) + _value;
            }
        }

        return _value;
    }

    private static bool IsPalindromePrefix(string s, int length)
    {
        var left = 0;
        var right = length - 1;

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

    [Benchmark]
    public string KmpFailureFunction()
    {
        var reversed = new string(_value.Reverse().ToArray());
        var combined = _value + FailureFunctionSeparator + reversed;
        var failure = PrefixFunctionSearch.ComputeFailureFunction(combined);
        var longestPalindromicPrefix = failure[^1];

        return new string(_value[longestPalindromicPrefix..].Reverse().ToArray()) + _value;
    }
}
