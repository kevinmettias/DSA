using DSAExperimentation.LeetCode.DesignAuctionSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAuctionSystem;

// Harness only. Both strategies are DesignAuctionSystemSolution's - this file
// replays LeetCode's published call sequence, extended with two extra calls that
// exercise addBid's own replace-on-same-(userId, itemId) rule (never hit by the
// published sequence itself), against each IAuctionSystemStrategy implementation
// via a small operation script, so a failure still names the strategy that broke
// even though the "input" here is a sequence of mutating calls rather than a
// single argument tuple. AuctionOp.Apply is pure dispatch (which method to call
// with which arguments) - no bidding/ordering logic of its own.
public sealed partial class DesignAuctionSystemTests
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
        Assert.Equal(expected, RunScript(new DesignAuctionSystemSolution.AuctionSystemByLinearScan(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AuctionSystemByLazyDeletionHeap_LeetCodeExample_ReturnsHighestBidderPerItem(
        AuctionOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignAuctionSystemSolution.AuctionSystemByLazyDeletionHeap(), operations));

    private static int?[] RunScript(
        DesignAuctionSystemSolution.IAuctionSystemStrategy strategy,
        AuctionOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(strategy))];
}
