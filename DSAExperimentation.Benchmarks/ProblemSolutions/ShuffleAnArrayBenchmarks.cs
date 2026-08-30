using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shuffle an Array (LC 384): the naive "remove a random remaining element" approach
// (a List<int>.RemoveAt shifts every trailing element on almost every draw - O(n^2)
// worst case) vs. in-place Fisher-Yates over this repo's own DynamicArray<int> -
// Get/Set swap each position from the end down to index 1, O(n), no auxiliary
// "remaining pool" collection at all.
[MemoryDiagnoser]
public class ShuffleAnArrayBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _original = null!;

    [GlobalSetup]
    public void Setup() => _original = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int[] RemoveRandomRemaining()
    {
        var random = new Random(1);
        var remaining = new List<int>(_original);
        var result = new int[remaining.Count];

        for (var i = 0; i < result.Length; i++)
        {
            var index = random.Next(remaining.Count);
            result[i] = remaining[index];
            remaining.RemoveAt(index);
        }

        return result;
    }

    [Benchmark]
    public int[] FisherYatesDynamicArray()
    {
        var random = new Random(1);
        var values = new DynamicArray<int>();
        foreach (var value in _original)
        {
            values.Add(value);
        }

        for (var i = values.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            var temp = values.Get(i);
            values.Set(i, values.Get(j));
            values.Set(j, temp);
        }

        var result = new int[values.Count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = values.Get(i);
        }

        return result;
    }
}
