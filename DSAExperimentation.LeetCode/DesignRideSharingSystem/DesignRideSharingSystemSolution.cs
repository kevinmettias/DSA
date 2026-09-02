using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.DesignRideSharingSystem;

// LeetCode 3829. Design Ride Sharing System: an instance API (addRider/addDriver/
// matchDriverWithRider/cancelRider) rather than a pure function, so - as with
// DesignAuctionSystemSolution - "every strategy for the problem" (§17.3) takes the
// form of two full classes implementing the shared IRideSharingStrategy surface
// below, and a benchmark replays one call script against a fresh instance per arm
// rather than hoisting a "prepared input" (there isn't one to prepare).
internal static class DesignRideSharingSystemSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IRideSharingStrategy
    {
        void AddRider(int riderId);

        void AddDriver(int driverId);

        int[] MatchDriverWithRider();

        void CancelRider(int riderId);
    }

    // The textbook answer: two BCL Queue<int> instances and nothing else - a
    // cancel has no membership index to consult, so it can only splice riderId
    // out by draining and rebuilding the whole rider queue, O(riders) every time.
    // Deliberately written without this repo's primitives - it is the arm the
    // lazy-deletion strategy below has to justify itself against.
    internal sealed class RideSharingSystemByLinearScanQueue : IRideSharingStrategy
    {
        private readonly Queue<int> _riders = new();
        private readonly Queue<int> _drivers = new();

        public void AddRider(int riderId) => _riders.Enqueue(riderId);

        public void AddDriver(int driverId) => _drivers.Enqueue(driverId);

        public void CancelRider(int riderId)
        {
            var remaining = new Queue<int>(_riders.Count);

            while (_riders.Count > 0)
            {
                var candidate = _riders.Dequeue();

                if (candidate != riderId)
                {
                    remaining.Enqueue(candidate);
                }
            }

            while (remaining.Count > 0)
            {
                _riders.Enqueue(remaining.Dequeue());
            }
        }

        public int[] MatchDriverWithRider()
        {
            if (_drivers.Count == 0 || _riders.Count == 0)
            {
                return [LeetCodeAnswer.None, LeetCodeAnswer.None];
            }

            return [_drivers.Dequeue(), _riders.Dequeue()];
        }
    }

    // This repo's own Queue<Element> (DataStructures/Queue) for both FIFO lines,
    // plus a Set<Element> recording which riders are still pending. CancelRider
    // only ever drops the id from the Set - O(1) - and never touches the queue
    // itself; MatchDriverWithRider lazily discards any front rider the Set no
    // longer contains before matching, the same lazy-deletion trick
    // AuctionSystemByLazyDeletionHeap's GetHighestBidder uses against a stale
    // heap entry, just against a queue's front instead of a heap's root.
    internal sealed class RideSharingSystemByLazyDeletionQueue : IRideSharingStrategy
    {
        private readonly DataStructures.Queue.Queue<int> _riders = new();
        private readonly DataStructures.Queue.Queue<int> _drivers = new();
        private readonly Set<int> _pendingRiders = new();

        public void AddRider(int riderId)
        {
            _riders.Enqueue(riderId);
            _pendingRiders.TryAdd(riderId);
        }

        public void AddDriver(int driverId) => _drivers.Enqueue(driverId);

        public void CancelRider(int riderId) => _pendingRiders.TryRemove(riderId);

        public int[] MatchDriverWithRider()
        {
            DiscardCancelledFront();

            if (!_drivers.TryPeek(out _) || !_riders.TryPeek(out _))
            {
                return [LeetCodeAnswer.None, LeetCodeAnswer.None];
            }

            _drivers.TryDequeue(out var driverId);
            _riders.TryDequeue(out var riderId);
            _pendingRiders.TryRemove(riderId);

            return [driverId, riderId];
        }

        private void DiscardCancelledFront()
        {
            while (_riders.TryPeek(out var candidate) && !_pendingRiders.Has(candidate))
            {
                _riders.TryDequeue(out _);
            }
        }
    }
}
