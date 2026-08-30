using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Max Chunks To Make Sorted (LC 769): the O(n^2) brute force re-derives the prefix
// max from scratch for every candidate boundary i, vs. the O(n) single pass that
// carries the running max forward across iterations instead of re-scanning - the
// same "redo the scan vs. carry the accumulator" complexity split TwoSumBenchmarks/
// JumpGameBenchmarks already use for their own O(n^2)-vs-O(n) comparisons.
[MemoryDiagnoser]
public class MaxChunksToMakeSortedBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, Length).ToArray();
        var random = new Random(1);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _values = values;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var chunks = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            var prefixMax = 0;

            for (var j = 0; j <= i; j++)
            {
                prefixMax = Math.Max(prefixMax, _values[j]);
            }

            if (prefixMax == i)
            {
                chunks++;
            }
        }

        return chunks;
    }

    [Benchmark]
    public int RunningMaxScan()
    {
        var chunks = 0;
        var runningMax = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            runningMax = Math.Max(runningMax, _values[i]);

            if (runningMax == i)
            {
                chunks++;
            }
        }

        return chunks;
    }
}
