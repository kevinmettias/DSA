using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindMedianFromDataStream;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMedianFromDataStreamSolution's, the same
// factories FindMedianFromDataStreamTests proves correct, replaying the same
// interleaved AddNum/FindMedian stream - a sort-on-every-query baseline
// (O(n log n) per query) vs. the two-heap approach (O(log n) per insert, O(1)
// per query).
[MemoryDiagnoser]
public class FindMedianFromDataStreamBenchmarks
{
    // LC problem number, reused as the fixed benchmark-data seed.
    private const int RandomSeed = 295;

    private const int MaxStreamValue = 1_000_000;

    private int[] _stream = [];

    [Params(100, 1_000)]
    public int StreamLength { get; set; }

    [GlobalSetup]
    public void Setup() => _stream = FindMedianFromDataStreamWorkloads.BuildStream(
        StreamLength, RandomSeed, MaxStreamValue);

    [Benchmark(Baseline = true)]
    public double SortOnEveryQuery() => Replay(FindMedianFromDataStreamSolution.CreateBySortOnEveryQuery());

    [Benchmark]
    public double TwoHeaps() => Replay(FindMedianFromDataStreamSolution.CreateByTwoHeaps());

    private double Replay(IMedianFinder medianFinder)
    {
        var lastMedian = 0.0;

        foreach (var value in _stream)
        {
            medianFinder.AddNum(value);
            lastMedian = medianFinder.FindMedian();
        }

        return lastMedian;
    }
}
