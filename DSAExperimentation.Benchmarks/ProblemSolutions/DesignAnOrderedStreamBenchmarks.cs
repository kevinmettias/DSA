using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignAnOrderedStream.DesignAnOrderedStreamSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAnOrderedStreamSolution's, the same classes
// DesignAnOrderedStreamTests proves correct - the textbook List<string?> + cursor
// baseline against this repo's own DynamicArray<string?> doing the same
// pre-filled-slots-plus-cursor bookkeeping, the same "array Representation
// primitive vs. the BCL equivalent" comparison DesignBrowserHistoryBenchmarks
// already makes. Ids arrive in a fixed shuffled (not ascending) order so every
// Insert does real cursor-walking work instead of the trivial
// one-id-at-a-time-in-order case; [GlobalSetup] materializes that order and the
// values, so shuffling and string formatting are charged to setup rather than to
// the replay.
[MemoryDiagnoser]
public class DesignAnOrderedStreamBenchmarks
{
    // Fixed so both arms replay the identical arrival order every run.
    private const int ArrivalSeed = 1;

    [Params(200, 5_000)]
    public int Size;

    private int[] _order = null!;
    private string[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        _order = ShuffledIds(Size, ArrivalSeed);
        _values = new string[Size];

        for (var i = 0; i < Size; i++)
        {
            _values[i] = $"v{i + 1}";
        }
    }

    [Benchmark(Baseline = true)]
    public int ListBacked() => Replay(new OrderedStreamByListBacked(Size));

    [Benchmark]
    public int DynamicArrayBacked() => Replay(new OrderedStreamByDynamicArrayBacked(Size));

    private int Replay(IOrderedStream stream)
    {
        var emitted = 0;

        foreach (var id in _order)
        {
            emitted += stream.Insert(id, _values[id - 1]).Count;
        }

        return emitted;
    }

    private static int[] ShuffledIds(int size, int seed)
    {
        var ids = Enumerable.Range(1, size).ToArray();
        var random = new Random(seed);

        for (var i = ids.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (ids[i], ids[j]) = (ids[j], ids[i]);
        }

        return ids;
    }
}
