using BenchmarkDotNet.Attributes;
using RepoDynamicArray = DSAExperimentation.DataStructures.DynamicArray.DynamicArray<string?>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design an Ordered Stream (LC 1656): a textbook List<string?> + cursor implementation
// vs. this repo's own DynamicArray<string?> doing the same pre-filled-slots-plus-cursor
// bookkeeping, the same "BCL List vs. repo DynamicArray" comparison
// DesignBrowserHistoryBenchmarks already makes. Ids arrive in a fixed shuffled (not
// ascending) order so every Insert does real cursor-walking work instead of the
// trivial one-id-at-a-time-in-order case.
[MemoryDiagnoser]
public class DesignAnOrderedStreamBenchmarks
{
    [Params(200, 5_000)]
    public int Size;

    private int[] _order = null!;

    [GlobalSetup]
    public void Setup()
    {
        var order = Enumerable.Range(1, Size).ToArray();
        var random = new Random(1);
        for (var i = order.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (order[i], order[j]) = (order[j], order[i]);
        }

        _order = order;
    }

    [Benchmark(Baseline = true)]
    public int ListBacked()
    {
        var values = new List<string?>(new string?[Size]);
        var ptr = 0;
        var emitted = 0;

        foreach (var id in _order)
        {
            values[id - 1] = $"v{id}";

            while (ptr < values.Count && values[ptr] is not null)
            {
                emitted++;
                ptr++;
            }
        }

        return emitted;
    }

    [Benchmark]
    public int DynamicArrayBacked()
    {
        var values = new RepoDynamicArray();
        for (var i = 0; i < Size; i++)
        {
            values.Add(null);
        }

        var ptr = 0;
        var emitted = 0;

        foreach (var id in _order)
        {
            values.Set(id - 1, $"v{id}");

            while (ptr < values.Count && values.Get(ptr) is not null)
            {
                emitted++;
                ptr++;
            }
        }

        return emitted;
    }
}
