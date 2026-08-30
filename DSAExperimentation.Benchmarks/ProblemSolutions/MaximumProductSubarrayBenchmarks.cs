using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Product Subarray (LC 152): the O(n^2) all-subarrays brute force vs.
// the O(n) single pass tracking both a running min and a running max product
// (a negative number can turn the running min into the new running max, so
// both must be tracked, not just the max). No repo primitive applies here -
// this is a pure running-best scan over the array itself, the same
// "no stronger reusable primitive" shape already established for
// MaximumSubarray/GasStation.
[MemoryDiagnoser]
public class MaximumProductSubarrayBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(152);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-10, 11)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForceAllSubarrays()
    {
        long best = _values[0];

        for (var i = 0; i < _values.Length; i++)
        {
            long product = 1;

            for (var j = i; j < _values.Length; j++)
            {
                product *= _values[j];
                best = Math.Max(best, product);
            }
        }

        return best;
    }

    [Benchmark]
    public long MinMaxSinglePass()
    {
        long min = _values[0];
        long max = _values[0];
        long best = _values[0];

        for (var i = 1; i < _values.Length; i++)
        {
            if (_values[i] < 0)
            {
                (min, max) = (max, min);
            }

            max = Math.Max(_values[i], max * _values[i]);
            min = Math.Min(_values[i], min * _values[i]);
            best = Math.Max(best, max);
        }

        return best;
    }
}
