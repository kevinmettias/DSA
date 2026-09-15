using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DataStreamAsDisjointIntervals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DataStreamAsDisjointIntervalsSolution's, the same strategies
// DataStreamAsDisjointIntervalsTests proves correct. Drain feeds the whole generated value
// stream through addNum() one call at a time - the LeetCode-shaped sequence itself, not a
// batch construction - then reads getIntervals() once so the whole stream is charged, the
// same "run the stateful object end to end" shape BinarySearchTreeIteratorBenchmarks uses.
[MemoryDiagnoser]
public class DataStreamAsDisjointIntervalsBenchmarks
{
    private const int RandomSeed = 7;

    // How far the generated value range spreads beyond Length, so intervals overlap
    // and merge realistically instead of always being disjoint singletons.
    private const int ValueRangeMultiplier = 3;

    private int[] _values = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length * ValueRangeMultiplier)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullRebuildEachCall() => Drain(DataStreamAsDisjointIntervalsSolution.CreateByFullRebuildEachCall());

    [Benchmark]
    public int IntervalSetAddNum() => Drain(DataStreamAsDisjointIntervalsSolution.CreateByIntervalSetMerge());

    private int Drain(DataStreamAsDisjointIntervalsSolution.ISummaryRanges stream)
    {
        foreach (var value in _values)
        {
            stream.AddNum(value);
        }

        return stream.GetIntervals().Count;
    }
}
