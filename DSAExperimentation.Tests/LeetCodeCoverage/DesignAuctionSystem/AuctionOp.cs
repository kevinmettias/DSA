using DSAExperimentation.LeetCode.DesignAuctionSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAuctionSystem;

// One call in an AuctionSystem script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script (like
// Examples in DesignAuctionSystemTests) reads like the LeetCode call sequence it
// replays.
public readonly record struct AuctionOp(AuctionOp.OpKind kind, int userId, int itemId, int amount)
{
    public static AuctionOp AddBid(int userId, int itemId, int bidAmount) =>
        new(OpKind.AddBid, userId, itemId, bidAmount);

    public static AuctionOp UpdateBid(int userId, int itemId, int newAmount) =>
        new(OpKind.UpdateBid, userId, itemId, newAmount);

    public static AuctionOp RemoveBid(int userId, int itemId) => new(OpKind.RemoveBid, userId, itemId, 0);

    public static AuctionOp GetHighestBidder(int itemId) => new(OpKind.GetHighestBidder, 0, itemId, 0);

    // null for the three void calls, the returned userId for GetHighestBidder - so
    // a script runner can assert against one expected value per operation
    // uniformly. Internal, not public: IAuctionSystemStrategy is internal to
    // DesignAuctionSystemSolution, and only this same assembly's RunScript ever
    // calls Apply.
    internal int? Apply(DesignAuctionSystemSolution.IAuctionSystemStrategy strategy)
    {
        switch (kind)
        {
            case OpKind.AddBid:
                strategy.AddBid(userId, itemId, amount);
                return null;
            case OpKind.UpdateBid:
                strategy.UpdateBid(userId, itemId, amount);
                return null;
            case OpKind.RemoveBid:
                strategy.RemoveBid(userId, itemId);
                return null;
            default:
                return strategy.GetHighestBidder(itemId);
        }
    }

    public enum OpKind
    {
        AddBid,
        UpdateBid,
        RemoveBid,
        GetHighestBidder,
    }
}
