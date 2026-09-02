using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.Set;
using RepoPacketQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Source, int Destination, int Timestamp)>;

namespace DSAExperimentation.LeetCode.ImplementRouter;

// LeetCode 3508. Implement Router: an instance API (addPacket/forwardPacket/
// getCount) rather than a pure function, so "every strategy for the problem"
// (§17.3) takes the form of two full classes implementing the shared
// IRouterStrategy surface below, instead of two static methods sharing an
// <Operation>By<Strategy> name - the same shape DesignTaskManagerSolution
// already uses for its own Design-category problem.
internal static class ImplementRouterSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IRouterStrategy
    {
        bool AddPacket(int source, int destination, int timestamp);

        int[] ForwardPacket();

        int GetCount(int destination, int startTime, int endTime);
    }

    // The textbook answer: a BCL List as FIFO storage, a BCL HashSet for the
    // duplicate check, and a full linear scan of every stored packet on every
    // getCount - deliberately without this repo's primitives, the arm the
    // binary-search-index strategy below has to justify itself against.
    internal sealed class RouterByLinearScan : IRouterStrategy
    {
        private readonly int _memoryLimit;
        private readonly List<(int Source, int Destination, int Timestamp)> _stored = [];
        private readonly HashSet<(int Source, int Destination, int Timestamp)> _seen = [];

        public RouterByLinearScan(int memoryLimit) => _memoryLimit = memoryLimit;

        public bool AddPacket(int source, int destination, int timestamp)
        {
            var packet = (source, destination, timestamp);

            if (!_seen.Add(packet))
            {
                return false;
            }

            if (_stored.Count == _memoryLimit)
            {
                _seen.Remove(_stored[0]);
                _stored.RemoveAt(0);
            }

            _stored.Add(packet);
            return true;
        }

        public int[] ForwardPacket()
        {
            if (_stored.Count == 0)
            {
                return [];
            }

            var packet = _stored[0];
            _stored.RemoveAt(0);
            _seen.Remove(packet);
            return [packet.Source, packet.Destination, packet.Timestamp];
        }

        public int GetCount(int destination, int startTime, int endTime)
        {
            var count = 0;

            foreach (var packet in _stored)
            {
                if (packet.Destination == destination && packet.Timestamp >= startTime && packet.Timestamp <= endTime)
                {
                    count++;
                }
            }

            return count;
        }
    }

    // This repo's own Queue<Element> for global FIFO storage order, Set<Element>
    // for the duplicate check, and a HashMap<int, DestinationLog> tracking, per
    // destination, every timestamp it has ever held (append-only, so it stays
    // sorted for free given LC's own "calls arrive in non-decreasing timestamp
    // order" guarantee) plus a start index that only ever advances. Both
    // addPacket's memory-limit eviction and forwardPacket remove from the same
    // global front, and LC's monotonic-timestamp guarantee means whichever
    // packet that is is also the oldest survivor for its own destination - so
    // advancing that one destination's start index is always correct, never a
    // rescan. getCount then composes Algorithms.Searching.BinarySearch's
    // LowerBound/UpperBound over that destination's own timestamps
    // (DynamicArraySequence-wrapped for IRandomAccessSequence), clipped to the
    // still-live suffix, the same O(log n) range-count shape
    // FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximum-style problems
    // already lean on.
    internal sealed class RouterByBinarySearchIndex : IRouterStrategy
    {
        private readonly int _memoryLimit;
        private readonly RepoPacketQueue _stored = new();
        private readonly Set<(int Source, int Destination, int Timestamp)> _seen = new();
        private readonly HashMap<int, DestinationLog> _byDestination = new();

        public RouterByBinarySearchIndex(int memoryLimit) => _memoryLimit = memoryLimit;

        public bool AddPacket(int source, int destination, int timestamp)
        {
            var packet = (source, destination, timestamp);

            if (!_seen.TryAdd(packet))
            {
                return false;
            }

            if (_stored.Count == _memoryLimit)
            {
                Evict();
            }

            _stored.Enqueue(packet);
            LogFor(destination).Timestamps.Add(timestamp);
            return true;
        }

        public int[] ForwardPacket()
        {
            if (!_stored.TryDequeue(out var packet))
            {
                return [];
            }

            _seen.TryRemove(packet);
            LogFor(packet.Destination).StartIndex++;
            return [packet.Source, packet.Destination, packet.Timestamp];
        }

        public int GetCount(int destination, int startTime, int endTime)
        {
            if (!_byDestination.TryGetValue(destination, out var log))
            {
                return 0;
            }

            var sequence = new DynamicArraySequence<int>(log.Timestamps);
            var lower = Math.Max(log.StartIndex, BinarySearch.LowerBound<int, DynamicArraySequence<int>>(sequence, startTime));
            var upper = BinarySearch.UpperBound<int, DynamicArraySequence<int>>(sequence, endTime);
            return Math.Max(0, upper - lower);
        }

        private void Evict()
        {
            if (_stored.TryDequeue(out var evicted))
            {
                _seen.TryRemove(evicted);
                LogFor(evicted.Destination).StartIndex++;
            }
        }

        private DestinationLog LogFor(int destination)
        {
            if (_byDestination.TryGetValue(destination, out var log))
            {
                return log;
            }

            log = new DestinationLog();
            _byDestination.Set(destination, log);
            return log;
        }

        // Per-destination bookkeeping: every timestamp that destination has
        // ever been sent, in arrival order (so always sorted, per LC's own
        // monotonic-timestamp guarantee), plus how many of its earliest
        // entries have already left storage.
        private sealed class DestinationLog
        {
            public DynamicArray<int> Timestamps { get; } = new();

            public int StartIndex { get; set; }
        }
    }
}
