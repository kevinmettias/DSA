using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindServersThatHandledMostNumberOfRequests;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindServersThatHandledMostNumberOfRequestsSolution's,
// the same methods FindServersThatHandledMostNumberOfRequestsTests proves correct -
// the textbook O(n*k) ring walk against the FenwickTree availability BIT +
// BinarySearch.LowerBound ceiling query paired with a Heap of busy servers. _load is
// generated wide enough (up to 3x ServerCount) that requests routinely outlive many
// future arrivals, forcing real contention and wraparound instead of every request
// finding `start` free immediately; [GlobalSetup] owns that construction.
[MemoryDiagnoser]
public class FindServersThatHandledMostNumberOfRequestsBenchmarks
{
    private const int RequestsPerServer = 20;
    private const int MaxLoadMultiplier = 3;
    private const int RandomSeed = 1;

    [Params(50, 400)]
    public int ServerCount;

    private int[] _arrival = null!;
    private int[] _load = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var requestCount = ServerCount * RequestsPerServer;
        _arrival = new int[requestCount];
        _load = new int[requestCount];

        for (var i = 0; i < requestCount; i++)
        {
            _arrival[i] = i;
            _load[i] = random.Next(1, (ServerCount * MaxLoadMultiplier) + 1);
        }
    }

    [Benchmark(Baseline = true)]
    public int[] LinearScanRing() =>
        FindServersThatHandledMostNumberOfRequestsSolution.BusiestServersByLinearScanRing(
            ServerCount, _arrival, _load);

    [Benchmark]
    public int[] FenwickCeilingAndHeap() =>
        FindServersThatHandledMostNumberOfRequestsSolution.BusiestServersByFenwickCeilingAndHeap(
            ServerCount, _arrival, _load);
}
