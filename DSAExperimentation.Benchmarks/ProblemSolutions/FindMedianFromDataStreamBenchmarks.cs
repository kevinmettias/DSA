using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindMedianFromDataStream;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMedianFromDataStreamSolution's, the same
// factories FindMedianFromDataStreamSolutionTests proves correct, replaying the same
// interleaved AddNum/FindMedian stream - a sort-on-every-query baseline
// (O(n log n) per query) vs. the two-heap approach (O(log n) per insert, O(1)
// per query). Each arm returns every median the stream reported, in order.
public class FindMedianFromDataStreamBenchmarks
{
    // LC problem number, reused as the fixed benchmark-data seed.
    private const int RandomSeed = 295;

    // The stream is drawn below this, inside LC 295's largest num, 10^5.
    private const int MaxStreamValue = 100_000;

    private int[] _stream = [];

    // Every median FindMedian reports; sized in setup so the replay allocates nothing.
    private double[] _medians = [];

    [Params(100, 1_000)]
    public int StreamLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _stream = FindMedianFromDataStreamWorkloads.BuildStream(StreamLength, RandomSeed, MaxStreamValue);
        _medians = new double[_stream.Length];
    }

    [Benchmark(Baseline = true)]
    public double[] SortOnEveryQuery() => Replay(FindMedianFromDataStreamSolution.CreateBySortOnEveryQuery());

    [Benchmark]
    public double[] TwoHeaps() => Replay(FindMedianFromDataStreamSolution.CreateByTwoHeaps());

    private double[] Replay(IMedianFinder medianFinder)
    {
        for (var i = 0; i < _stream.Length; i++)
        {
            medianFinder.AddNum(_stream[i]);
            _medians[i] = medianFinder.FindMedian();
        }

        return _medians;
    }
}
