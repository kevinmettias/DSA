using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Servers That Handled Most Number of Requests (LC 1606): the textbook O(n*k)
// simulation (walk the ring one server at a time from `start` until a free one
// turns up) vs. this repo's own FenwickTree<int,SumOperation<int>> availability BIT
// + BinarySearch.LowerBound (ceiling query, FindServersThatHandledMostNumberOf
// RequestsTests precedent) paired with a Heap<(int,int),MinHeapOrder> of busy
// servers ordered by end time. _load is generated wide enough (up to 3x
// ServerCount) that requests routinely outlive many future arrivals, forcing real
// contention and wraparound instead of every request finding `start` free
// immediately.
[MemoryDiagnoser]
public class FindServersThatHandledMostNumberOfRequestsBenchmarks
{
    [Params(50, 400)]
    public int ServerCount;

    private int[] _arrival = null!;
    private int[] _load = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var requestCount = ServerCount * 20;
        _arrival = new int[requestCount];
        _load = new int[requestCount];

        for (var i = 0; i < requestCount; i++)
        {
            _arrival[i] = i;
            _load[i] = random.Next(1, (ServerCount * 3) + 1);
        }
    }

    [Benchmark(Baseline = true)]
    public int LinearScanRing()
    {
        var k = ServerCount;
        var freeAt = new int[k];
        var handled = new int[k];

        for (var i = 0; i < _arrival.Length; i++)
        {
            var start = i % k;
            var found = -1;

            for (var offset = 0; offset < k; offset++)
            {
                var candidate = (start + offset) % k;

                if (freeAt[candidate] <= _arrival[i])
                {
                    found = candidate;
                    break;
                }
            }

            if (found == -1)
            {
                continue;
            }

            freeAt[found] = _arrival[i] + _load[i];
            handled[found]++;
        }

        return handled.Max();
    }

    [Benchmark]
    public int FenwickCeilingAndHeap()
    {
        var k = ServerCount;
        var availability = new FenwickTree<int, SumOperation<int>>(Enumerable.Repeat(1, k).ToArray());
        var busy = new Heap<(int End, int Server), MinHeapOrder<(int, int)>>();
        var handled = new int[k];

        for (var i = 0; i < _arrival.Length; i++)
        {
            var arrivalTime = _arrival[i];

            while (busy.TryPeek(out var freed) && freed.End <= arrivalTime)
            {
                busy.TryPop(out freed);
                availability.Add(freed.Server, 1);
            }

            if (!TryFindAvailableServer(availability, k, i % k, out var server))
            {
                continue;
            }

            availability.Add(server, -1);
            busy.Push((arrivalTime + _load[i], server));
            handled[server]++;
        }

        return handled.Max();
    }

    private static bool TryFindAvailableServer(FenwickTree<int, SumOperation<int>> availability, int k, int start, out int server)
    {
        var totalAvailable = availability.PrefixQuery(k - 1);

        if (totalAvailable == 0)
        {
            server = -1;
            return false;
        }

        var beforeStart = start == 0 ? 0 : availability.PrefixQuery(start - 1);
        var threshold = beforeStart < totalAvailable ? beforeStart + 1 : 1;
        var sequence = new AvailabilityPrefixSequence(availability, k);

        server = BinarySearch.LowerBound(sequence, threshold);
        return true;
    }

    private readonly struct AvailabilityPrefixSequence(FenwickTree<int, SumOperation<int>> availability, int length)
        : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => availability.PrefixQuery(index);
    }
}
