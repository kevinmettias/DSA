using DSAExperimentation.LeetCode.DesignAnOrderedStream;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAnOrderedStreamSolution's, the same classes
// DesignAnOrderedStreamSolutionTests proves correct - the textbook List<string?> + cursor
// baseline against this repo's own DynamicArray<string?> doing the same
// pre-filled-slots-plus-cursor bookkeeping, the same "array Representation
// primitive vs. the BCL equivalent" comparison DesignBrowserHistoryBenchmarks
// already makes. Ids arrive in a fixed shuffled (not ascending) order so every
// Insert does real cursor-walking work instead of the trivial
// one-id-at-a-time-in-order case; [GlobalSetup] materializes that order and the
// values, so shuffling and string formatting are charged to setup rather than to
// the replay.
public class DesignAnOrderedStreamBenchmarks
{
    // Fixed so both arms replay the identical arrival order every run.
    private const int ArrivalSeed = 1;

    private int[] _order = [];

    private string[] _values = [];

    // The chunk every Insert hands back, in arrival order; sized in setup so the replay
    // allocates nothing beyond what the strategy itself returns.
    private List<string>[] _chunks = [];
    [Params(200, 5_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _order = ShuffledIds(Size, ArrivalSeed);
        _values = new string[Size];

        for (var i = 0; i < Size; i++)
        {
            _values[i] = $"v{i + 1}";
        }

        _chunks = new List<string>[Size];
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

    [Benchmark(Baseline = true)]
    public List<string>[] ListBacked() => Replay(new DesignAnOrderedStreamSolution.OrderedStreamByListBacked(Size));

    [Benchmark]
    public List<string>[] DynamicArrayBacked() => Replay(new DesignAnOrderedStreamSolution.OrderedStreamByDynamicArrayBacked(Size));

    private List<string>[] Replay(DesignAnOrderedStreamSolution.IOrderedStream stream)
    {
        for (var i = 0; i < _order.Length; i++)
        {
            var id = _order[i];
            _chunks[i] = stream.Insert(id, _values[id - 1]);
        }

        return _chunks;
    }
}
