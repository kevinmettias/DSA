using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// N-Repeated Element in Size 2N Array (LC 961): the O(n^2) pairwise brute force
// (baseline - compare every element against every later element) vs. a single
// O(n) pass using this repo's own Set<int> - the first value TryAdd refuses is
// the repeated one, the same "have I seen this before" role Set<int> already
// plays in NumberOfProvincesBenchmarks, checked on every element here instead of
// only once at the end.
[MemoryDiagnoser]
public class NRepeatedElementInSize2NArrayBenchmarks
{
    private const int ArrayLengthMultiplier = 2;
    private const string NoRepeatedElementFoundMessage = "No repeated element found.";

    [Params(200, 20_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Length is always even (2n): n distinct values first, THEN n copies of
        // the repeated value (0) last - matching the problem's own "n + 1 unique
        // elements" shape, deliberately unshuffled. This forces PairwiseBruteForce
        // to burn a full wasted inner scan on every one of the n distinct leading
        // values (each is truly unique, so no j ever matches) before it reaches
        // the repeat, giving genuine O(n^2) worst-case behavior instead of an
        // accidental near-instant match - the same "force the full scan" intent
        // TwoSumBenchmarks achieves via an unreachable target.
        var n = Length / ArrayLengthMultiplier;
        var values = new List<int>(Length);

        for (var i = 1; i <= n; i++)
        {
            values.Add(i);
        }

        for (var i = 0; i < n; i++)
        {
            values.Add(0);
        }

        _values = [.. values];
    }

    [Benchmark(Baseline = true)]
    public int PairwiseBruteForce()
    {
        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = i + 1; j < _values.Length; j++)
            {
                if (_values[i] == _values[j])
                {
                    return _values[i];
                }
            }
        }

        throw new InvalidOperationException(NoRepeatedElementFoundMessage);
    }

    [Benchmark]
    public int SetOnePass()
    {
        var seen = new Set<int>();

        foreach (var value in _values)
        {
            if (!seen.TryAdd(value))
            {
                return value;
            }
        }

        throw new InvalidOperationException(NoRepeatedElementFoundMessage);
    }
}
