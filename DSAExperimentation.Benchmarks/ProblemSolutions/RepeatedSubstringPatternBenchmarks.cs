using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Repeated Substring Pattern (LC 459): the textbook O(n^2) divisor scan (try every
// candidate period length that evenly divides n, verify it by direct comparison) vs.
// the O(n) single pass using this repo's own PrefixFunctionSearch.ComputeFailureFunction
// - the string repeats exactly when its final failure-function value is nonzero and
// evenly divides the string length. Text is random lowercase letters, so a genuine
// repeating period is astronomically unlikely and both strategies run to completion.
[MemoryDiagnoser]
public class RepeatedSubstringPatternBenchmarks
{
    private const int AlphabetSize = 26;
    private const int MaxPeriodDivisor = 2;

    [Params(200, 5_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _text = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public bool DivisorBruteForce()
    {
        var n = _text.Length;

        for (var period = 1; period <= n / MaxPeriodDivisor; period++)
        {
            if (n % period != 0)
            {
                continue;
            }

            if (RepeatsWithPeriod(period))
            {
                return true;
            }
        }

        return false;
    }

    private bool RepeatsWithPeriod(int period)
    {
        for (var i = period; i < _text.Length; i++)
        {
            if (_text[i] != _text[i - period])
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool KmpFailureFunction()
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction(_text);
        var longestBorder = failure[^1];
        var period = _text.Length - longestBorder;

        return longestBorder != 0 && _text.Length % period == 0;
    }
}
