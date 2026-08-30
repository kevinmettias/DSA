using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindServersThatHandledMostNumberOfRequests;

// LeetCode 1606. Find Servers That Handled Most Number of Requests: two repo
// primitives standing in for the textbook "TreeSet of free servers + TreeSet of
// (endTime, server) busy servers" this problem is usually solved with.
//
// Busy servers: a Heap<(int End, int Server), MinHeapOrder<(int,int)>> ordered by
// end time (KClosestPointsToOriginTests precedent - ValueTuple's own IComparable
// already orders by End first, Server second) - its root is always the next server
// due to free.
//
// Free servers: a FenwickTree<int, SumOperation<int>> of 0/1 availability. Finding
// the first free server at or after `start` reduces to "smallest index whose
// availability prefix sum has grown past the count already seen before start" -
// PrefixQuery is monotonic non-decreasing (deltas are only 0/1), so that index is
// found by wrapping the tree in an IRandomAccessSequence<int> (Get(i) =
// PrefixQuery(i)) and handing it to this repo's own BinarySearch.LowerBound
// (CountOfSmallerNumbersAfterSelfTests/ExamRoomTests precedent for "wrap a repo
// structure as a Sequence view, then search it"), instead of a linear scan around
// the ring.
public sealed partial class FindServersThatHandledMostNumberOfRequestsTests
{
    [Fact]
    public void BusiestServers_LeetCodeExampleOne_ReturnsSingleBusiestServer()
    {
        int[] arrival = [1, 2, 3, 4, 5];
        int[] load = [5, 2, 3, 3, 3];

        var busiest = BusiestServers(k: 3, arrival, load);

        Assert.Equal([1], busiest);
    }

    [Fact]
    public void BusiestServers_EveryServerHandlesExactlyOneRequest_ReturnsAllServersTied()
    {
        int[] arrival = [1, 2, 3];
        int[] load = [10, 12, 11];

        var busiest = BusiestServers(k: 3, arrival, load);

        Assert.Equal([0, 1, 2], busiest);
    }

    [Fact]
    public void BusiestServers_RequestArrivesFasterThanServersFree_DropsRequestWithNoFreeServer()
    {
        int[] arrival = [1, 2, 3, 4];
        int[] load = [1, 2, 1, 2];

        var busiest = BusiestServers(k: 3, arrival, load);

        Assert.Equal([0], busiest);
    }

    private static int[] BusiestServers(int k, int[] arrival, int[] load)
    {
        var availability = new FenwickTree<int, SumOperation<int>>(Enumerable.Repeat(1, k).ToArray());
        var busy = new Heap<(int End, int Server), MinHeapOrder<(int, int)>>();
        var handled = new int[k];

        for (var i = 0; i < arrival.Length; i++)
        {
            var arrivalTime = arrival[i];

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
            busy.Push((arrivalTime + load[i], server));
            handled[server]++;
        }

        var maxHandled = handled.Max();

        return [.. Enumerable.Range(0, k).Where(server => handled[server] == maxHandled)];
    }

    // Smallest available server index >= start, wrapping to [0, start) when nothing
    // is free from start through the end of the ring.
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
