using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.KthLargestElementInAStream;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthLargestElementInAStreamSolution's, the same
// factories KthLargestElementInAStreamTests proves correct, replaying the same
// interleaved Add stream against a freshly created, unseeded stream instance - a
// sort-on-every-add baseline (O(n log n) per call) vs. this repo's own size-k
// min-heap (O(log k) per call) - the FindMedianFromDataStream two-heap
// benchmark's "process the same interleaved stream, compare per-call cost" shape,
// specialized to LC703's single running order statistic.
[MemoryDiagnoser]
public class KthLargestElementInAStreamBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 703; // LC problem number
    private const int StreamValueExclusiveBound = 1_000_000;

    [Params(100, 1_000)]
    public int StreamLength;

    private int[] _stream = null!;

    [GlobalSetup]
    public void Setup() => _stream = KthLargestElementInAStreamWorkloads.BuildStream(
        StreamLength, RandomSeed, StreamValueExclusiveBound);

    [Benchmark(Baseline = true)]
    public int SortOnEveryAdd() =>
        Replay(KthLargestElementInAStreamSolution.CreateBySortOnEveryAdd(K, []));

    [Benchmark]
    public int SizeKMinHeap() =>
        Replay(KthLargestElementInAStreamSolution.CreateBySizeKMinHeap(K, []));

    private int Replay(IKthLargestStream stream)
    {
        var lastKthLargest = 0;

        foreach (var value in _stream)
        {
            lastKthLargest = stream.Add(value);
        }

        return lastKthLargest;
    }
}
