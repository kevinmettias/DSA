using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Continuous Subarray Sum (LC 523): the O(n^2) brute force (every start/end pair,
// summing as it extends) vs. this repo's O(n) HashMap<int,int> prefix-sum-remainder
// tracking (ContainsDuplicateII precedent). K is chosen larger than any possible
// total sum of the generated values, so every prefix sum's remainder is just the
// prefix sum itself - strictly increasing since every value is positive, so no two
// prefix sums (nor the seeded {0: -1} entry, since every prefix sum stays positive)
// ever collide. Both methods are therefore forced through their full worst-case scan
// on every [Params] size instead of an early match letting either return early.
[MemoryDiagnoser]
public class ContinuousSubarraySumBenchmarks
{
    private const int K = 1_000_003;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, 100)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BruteForce()
    {
        for (var start = 0; start < _values.Length; start++)
        {
            var sum = 0;
            for (var end = start; end < _values.Length; end++)
            {
                sum += _values[end];
                if (end - start >= 1 && sum % K == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Benchmark]
    public bool HashMapPrefixRemainder()
    {
        var firstIndexByRemainder = new HashMap<int, int>();
        firstIndexByRemainder.Set(0, -1);

        var prefixSum = 0;
        for (var i = 0; i < _values.Length; i++)
        {
            prefixSum += _values[i];
            var remainder = prefixSum % K;

            if (firstIndexByRemainder.TryGetValue(remainder, out var firstIndex))
            {
                if (i - firstIndex >= 2)
                {
                    return true;
                }
            }
            else
            {
                firstIndexByRemainder.Set(remainder, i);
            }
        }

        return false;
    }
}
