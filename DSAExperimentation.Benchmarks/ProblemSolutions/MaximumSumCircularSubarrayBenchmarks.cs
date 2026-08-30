using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Sum Circular Subarray (LC 918): the O(n^2) brute force that tries every
// circular subarray (every start index, every length 1..n, wrapping via modulo)
// vs. the O(n) two-Kadane-pass trick (MaximumSubarrayBenchmarks' own
// KadaneSinglePass, run twice - once for the max, once for the min - combined via
// total - minSum). No repo primitive applies here either, the same "no stronger
// reusable primitive" shape MaximumSubarrayBenchmarks already established for this
// technique family.
[MemoryDiagnoser]
public class MaximumSumCircularSubarrayBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(918);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-50, 51)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllCircularSubarrays()
    {
        var n = _values.Length;
        var best = _values[0];

        for (var start = 0; start < n; start++)
        {
            var sum = 0;

            for (var length = 1; length <= n; length++)
            {
                sum += _values[(start + length - 1) % n];
                best = Math.Max(best, sum);
            }
        }

        return best;
    }

    [Benchmark]
    public int TwoPassKadane()
    {
        var total = 0;
        var maxSum = _values[0];
        var currentMax = 0;
        var minSum = _values[0];
        var currentMin = 0;

        foreach (var n in _values)
        {
            currentMax = Math.Max(n, currentMax + n);
            maxSum = Math.Max(maxSum, currentMax);

            currentMin = Math.Min(n, currentMin + n);
            minSum = Math.Min(minSum, currentMin);

            total += n;
        }

        return maxSum < 0 ? maxSum : Math.Max(maxSum, total - minSum);
    }
}
