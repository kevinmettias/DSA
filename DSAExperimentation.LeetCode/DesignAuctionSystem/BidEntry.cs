namespace DSAExperimentation.LeetCode.DesignAuctionSystem;

// Heap element for AuctionSystemByLazyDeletionHeap: max-heap ordering is bidAmount
// first, then userId - LC's own tie-break rule for getHighestBidder (the highest
// userId wins on equal bidAmount). IComparable<BidEntry> is what
// DataStructures.Heap.MaxHeapOrder<Element> requires; a witness meaningful only to
// this problem's heap ordering, so it lives here rather than in Domain/ (§17.3).
internal readonly record struct BidEntry(int BidAmount, int UserId) : IComparable<BidEntry>
{
    public int CompareTo(BidEntry other)
    {
        var amountCompare = BidAmount.CompareTo(other.BidAmount);
        return amountCompare != 0 ? amountCompare : UserId.CompareTo(other.UserId);
    }
}
