using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubrectangleQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubrectangleQueriesSolution's, the same classes
// SubrectangleQueriesTests proves correct. [GlobalSetup] builds one fixed,
// deterministic script of overlapping subrectangle overwrites; each arm then
// replays it against a freshly constructed backing store, because the store's
// construction cost is itself half of what this comparison is about (a jagged
// int[][] versus a DynamicArray<T> of DynamicArray<T>) and because a store reused
// across invocations would carry the previous invocation's mutations forward.
[MemoryDiagnoser]
public class SubrectangleQueriesBenchmarks
{
    private const int QueryCount = 200;
    private const int UpdateSeed = 1;

    private (SubrectangleQueriesSolution.SubrectangleBounds Bounds, int Value)[] _updates = [];

    [Params(20, 100)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(UpdateSeed);
        _updates = new (SubrectangleQueriesSolution.SubrectangleBounds, int)[QueryCount];

        for (var i = 0; i < QueryCount; i++)
        {
            var row1 = random.Next(0, Size);
            var col1 = random.Next(0, Size);
            var row2 = random.Next(row1, Size);
            var col2 = random.Next(col1, Size);
            _updates[i] = (new SubrectangleQueriesSolution.SubrectangleBounds(row1, col1, row2, col2), i);
        }
    }

    [Benchmark(Baseline = true)]
    public int ArrayBacked() => Replay(new SubrectangleQueriesSolution.SubrectangleQueriesByArrayBacked(Size, Size));

    [Benchmark]
    public int DynamicArrayBacked() => Replay(new SubrectangleQueriesSolution.SubrectangleQueriesByDynamicArrayBacked(Size, Size));

    // Returns the far-corner cell rather than discarding the result, so the JIT
    // cannot eliminate the replay as dead code.
    private int Replay(SubrectangleQueriesSolution.ISubrectangleQueries queries)
    {
        foreach (var (bounds, value) in _updates)
        {
            queries.UpdateSubrectangle(bounds, value);
        }

        return queries.GetValue(Size - 1, Size - 1);
    }
}
