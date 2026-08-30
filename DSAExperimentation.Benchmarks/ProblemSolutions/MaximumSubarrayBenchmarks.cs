using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Subarray (LC 53): the textbook O(n^2) all-subarrays brute force vs.
// Kadane's O(n) single pass. No repo primitive applies here - this is a pure
// running-best scan over the array itself, the same "no stronger reusable
// primitive" shape already established for BestTimeToBuyAndSellStock/GasStation.
[MemoryDiagnoser]
public class MaximumSubarrayBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(53);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-50, 51)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllSubarrays()
    {
        var best = _values[0];

        for (var i = 0; i < _values.Length; i++)
        {
            var sum = 0;

            for (var j = i; j < _values.Length; j++)
            {
                sum += _values[j];
                best = Math.Max(best, sum);
            }
        }

        return best;
    }

    [Benchmark]
    public int KadaneSinglePass()
    {
        var best = _values[0];
        var current = _values[0];

        for (var i = 1; i < _values.Length; i++)
        {
            current = Math.Max(_values[i], current + _values[i]);
            best = Math.Max(best, current);
        }

        return best;
    }
}
