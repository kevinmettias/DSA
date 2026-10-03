using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.KthLargestElementInAStream;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthLargestElementInAStreamSolution's, the same
// factories KthLargestElementInAStreamSolutionTests proves correct, replaying the same
// interleaved Add stream against a freshly created, unseeded stream instance - a
// sort-on-every-add baseline (O(n log n) per call) vs. this repo's own size-k
// min-heap (O(log k) per call) - the FindMedianFromDataStream two-heap
// benchmark's "process the same interleaved stream, compare per-call cost" shape,
// specialized to LC703's single running order statistic.
public class KthLargestElementInAStreamBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 703; // LC problem number
    private const int StreamValueExclusiveBound = 1_000_000;

    private int[] _stream = [];

    // What every Add returned, in stream order - what each arm returns.
    private int[] _kthLargest = [];

    [Params(100, 1_000)]
    public int StreamLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _stream = KthLargestElementInAStreamWorkloads.BuildStream(StreamLength, RandomSeed, StreamValueExclusiveBound);
        _kthLargest = new int[_stream.Length];
    }

    [Benchmark(Baseline = true)]
    public int[] SortOnEveryAdd()
    {
        var stream = KthLargestElementInAStreamSolution.CreateBySortOnEveryAdd(K, []);
        return Replay(stream);
    }

    [Benchmark]
    public int[] SizeKMinHeap()
    {
        var stream = KthLargestElementInAStreamSolution.CreateBySizeKMinHeap(K, []);
        return Replay(stream);
    }

    private int[] Replay(IKthLargestStream stream)
    {
        for (var i = 0; i < _stream.Length; i++)
        {
            _kthLargest[i] = stream.Add(_stream[i]);
        }

        return _kthLargest;
    }
}
