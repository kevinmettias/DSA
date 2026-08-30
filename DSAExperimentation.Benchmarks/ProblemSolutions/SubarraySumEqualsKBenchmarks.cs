using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Subarray Sum Equals K (LC 560): the canonical O(n^2) brute-force double loop over
// every (start,end) pair vs. the O(n) single pass tracking prefix-sum frequency in
// this repo's own HashMap<int,int> - the same prefix-sum-frequency shape
// SubarraySumEqualsKTests proves correct, just measured here instead of asserted.
// _target is deliberately unreachable (values are small and bounded, target is far
// outside any possible running sum) so both strategies are forced through their
// full worst-case scan instead of an early exit favoring one of them.
[MemoryDiagnoser]
public class SubarraySumEqualsKBenchmarks
{
    private const int Target = 1_000_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-10, 11)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var count = 0;

        for (var start = 0; start < _values.Length; start++)
        {
            var sum = 0;
            for (var end = start; end < _values.Length; end++)
            {
                sum += _values[end];
                if (sum == Target)
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public int PrefixSumHashMap()
    {
        var countByPrefixSum = new HashMap<int, int>();
        countByPrefixSum.Set(0, 1);

        var prefixSum = 0;
        var count = 0;

        foreach (var value in _values)
        {
            prefixSum += value;

            if (countByPrefixSum.TryGetValue(prefixSum - Target, out var matches))
            {
                count += matches;
            }

            countByPrefixSum.TryGetValue(prefixSum, out var existingCount);
            countByPrefixSum.Set(prefixSum, existingCount + 1);
        }

        return count;
    }
}
