using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.DesignAuctionSystem;

// LeetCode 3815. Design Auction System: an instance API (addBid/updateBid/
// removeBid/getHighestBidder) rather than a pure function, so - as with
// DesignTaskManagerSolution - "every strategy for the problem" (§17.3) takes the
// form of two full classes implementing the shared IAuctionSystemStrategy surface
// below, and a benchmark replays one call script against a fresh instance per arm
// rather than hoisting a "prepared input" (there isn't one to prepare).
internal static class DesignAuctionSystemSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IAuctionSystemStrategy
    {
        void AddBid(int userId, int itemId, int bidAmount);

        void UpdateBid(int userId, int itemId, int newAmount);

        void RemoveBid(int userId, int itemId);

        int GetHighestBidder(int itemId);
    }

    // The textbook answer: a BCL Dictionary<itemId, Dictionary<userId, amount>> and
    // a full linear scan for the highest bid on every getHighestBidder -
    // deliberately without this repo's Heap, the arm the lazy-deletion heap
    // strategy below has to justify itself against.
    internal sealed class AuctionSystemByLinearScan : IAuctionSystemStrategy
    {
        private readonly Dictionary<int, Dictionary<int, int>> _bidsByItem = new();

        public void AddBid(int userId, int itemId, int bidAmount) => SetBid(userId, itemId, bidAmount);

        public void UpdateBid(int userId, int itemId, int newAmount) => SetBid(userId, itemId, newAmount);

        public void RemoveBid(int userId, int itemId)
        {
            if (_bidsByItem.TryGetValue(itemId, out var bids))
            {
                bids.Remove(userId);
            }
        }

        public int GetHighestBidder(int itemId)
        {
            if (!_bidsByItem.TryGetValue(itemId, out var bids) || bids.Count == 0)
            {
                return LeetCodeAnswer.None;
            }

            var highest = bids.Aggregate((champion, next) => IsHigherBid(next, champion) ? next : champion);
            return highest.Key;
        }

        private static bool IsHigherBid(KeyValuePair<int, int> candidate, KeyValuePair<int, int> incumbent)
            => candidate.Value != incumbent.Value
                ? candidate.Value > incumbent.Value
                : candidate.Key > incumbent.Key;

        private void SetBid(int userId, int itemId, int amount)
        {
            if (!_bidsByItem.TryGetValue(itemId, out var bids))
            {
                bids = new Dictionary<int, int>();
                _bidsByItem[itemId] = bids;
            }

            bids[userId] = amount;
        }
    }

    // This repo's own Heap<Element, MaxHeapOrder> per item, ordered by BidEntry's
    // (BidAmount, UserId) comparison, plus a HashMap<(userId, itemId), amount>
    // holding each bid's current authoritative amount. AddBid/UpdateBid both push a
    // fresh BidEntry rather than mutating the heap in place - Heap has no
    // decrease-key or arbitrary remove (see Heap.cs) - so GetHighestBidder lazily
    // discards popped entries that no longer match the HashMap's current record for
    // that (userId, itemId), the same lazy-deletion trick DesignTaskManager's
    // TaskManagerByLazyDeletionHeap already uses. Unlike ExecTop, GetHighestBidder
    // is a pure query rather than a consuming pop, so the one entry that does match
    // is pushed straight back once found instead of being discarded.
    internal sealed class AuctionSystemByLazyDeletionHeap : IAuctionSystemStrategy
    {
        private readonly HashMap<int, Heap<BidEntry, MaxHeapOrder<BidEntry>>> _heapsByItem = new();
        private readonly HashMap<(int UserId, int ItemId), int> _currentAmount = new();

        public void AddBid(int userId, int itemId, int bidAmount) => SetBid(userId, itemId, bidAmount);

        public void UpdateBid(int userId, int itemId, int newAmount) => SetBid(userId, itemId, newAmount);

        public void RemoveBid(int userId, int itemId) => _currentAmount.TryRemove((userId, itemId));

        public int GetHighestBidder(int itemId)
        {
            if (!_heapsByItem.TryGetValue(itemId, out var heap))
            {
                return LeetCodeAnswer.None;
            }

            while (heap.TryPop(out var candidate))
            {
                if (_currentAmount.TryGetValue((candidate.UserId, itemId), out var authoritative) &&
                    authoritative == candidate.BidAmount)
                {
                    heap.Push(candidate);
                    return candidate.UserId;
                }
            }

            return LeetCodeAnswer.None;
        }

        private void SetBid(int userId, int itemId, int amount)
        {
            _currentAmount.Set((userId, itemId), amount);

            if (!_heapsByItem.TryGetValue(itemId, out var heap))
            {
                heap = new Heap<BidEntry, MaxHeapOrder<BidEntry>>();
                _heapsByItem.Set(itemId, heap);
            }

            heap.Push(new BidEntry(amount, userId));
        }
    }
}
