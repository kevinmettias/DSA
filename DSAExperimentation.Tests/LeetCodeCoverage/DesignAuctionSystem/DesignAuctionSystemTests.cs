using static DSAExperimentation.LeetCode.DesignAuctionSystem.DesignAuctionSystemSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAuctionSystem;

// Harness only. Both strategies are DesignAuctionSystemSolution's - this file
// replays LeetCode's published call sequence, extended with two extra calls that
// exercise addBid's own replace-on-same-(userId, itemId) rule (never hit by the
// published sequence itself), against each IAuctionSystemStrategy implementation
// via a small operation script, so a failure still names the strategy that broke
// even though the "input" here is a sequence of mutating calls rather than a
// single argument tuple. AuctionOp.Apply is pure dispatch (which method to call
// with which arguments) - no bidding/ordering logic of its own.
public sealed class DesignAuctionSystemTests
{
    public static TheoryData<AuctionOp[], int?[]> Examples =>
        new()
        {
            {
                [
                    AuctionOp.AddBid(1, 7, 5),
                    AuctionOp.AddBid(2, 7, 6),
                    AuctionOp.GetHighestBidder(7),
                    AuctionOp.UpdateBid(1, 7, 8),
                    AuctionOp.GetHighestBidder(7),
                    AuctionOp.RemoveBid(2, 7),
                    AuctionOp.GetHighestBidder(7),
                    AuctionOp.GetHighestBidder(3),
                    AuctionOp.AddBid(1, 7, 3),
                    AuctionOp.GetHighestBidder(7),
                ],
                [null, null, 2, null, 1, null, 1, -1, null, 1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AuctionSystemByLinearScan_LeetCodeExample_ReturnsHighestBidderPerItem(
        AuctionOp[] operations, int?[] expected) =>
        RunScript(new AuctionSystemByLinearScan(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void AuctionSystemByLazyDeletionHeap_LeetCodeExample_ReturnsHighestBidderPerItem(
        AuctionOp[] operations, int?[] expected) =>
        RunScript(new AuctionSystemByLazyDeletionHeap(), operations, expected);

    private static void RunScript(IAuctionSystemStrategy strategy, AuctionOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }
}

// One call in an AuctionSystem script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct AuctionOp
{
    private readonly Kind _kind;
    private readonly int _userId;
    private readonly int _itemId;
    private readonly int _amount;

    private AuctionOp(Kind kind, int userId, int itemId, int amount)
    {
        _kind = kind;
        _userId = userId;
        _itemId = itemId;
        _amount = amount;
    }

    public static AuctionOp AddBid(int userId, int itemId, int bidAmount) =>
        new(Kind.AddBid, userId, itemId, bidAmount);

    public static AuctionOp UpdateBid(int userId, int itemId, int newAmount) =>
        new(Kind.UpdateBid, userId, itemId, newAmount);

    public static AuctionOp RemoveBid(int userId, int itemId) => new(Kind.RemoveBid, userId, itemId, 0);

    public static AuctionOp GetHighestBidder(int itemId) => new(Kind.GetHighestBidder, 0, itemId, 0);

    // null for the three void calls, the returned userId for GetHighestBidder - so
    // a script runner can assert against one expected value per operation
    // uniformly. Internal, not public: IAuctionSystemStrategy is internal to
    // DesignAuctionSystemSolution, and only this same assembly's RunScript ever
    // calls Apply.
    internal int? Apply(IAuctionSystemStrategy strategy)
    {
        switch (_kind)
        {
            case Kind.AddBid:
                strategy.AddBid(_userId, _itemId, _amount);
                return null;
            case Kind.UpdateBid:
                strategy.UpdateBid(_userId, _itemId, _amount);
                return null;
            case Kind.RemoveBid:
                strategy.RemoveBid(_userId, _itemId);
                return null;
            default:
                return strategy.GetHighestBidder(_itemId);
        }
    }

    private enum Kind
    {
        AddBid,
        UpdateBid,
        RemoveBid,
        GetHighestBidder,
    }
}
