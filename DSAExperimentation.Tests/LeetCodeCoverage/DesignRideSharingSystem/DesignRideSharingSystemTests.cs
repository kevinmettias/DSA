using DSAExperimentation.LeetCode.DesignRideSharingSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignRideSharingSystem;

// Harness only. Both strategies are DesignRideSharingSystemSolution's - this file
// replays LeetCode's published call sequence, extended with a cancel-before-match
// (never hit by the published sequence itself, which only cancels an already-
// matched rider) that actually removes a rider from consideration, against each
// IRideSharingStrategy implementation via a small operation script, so a failure
// still names the strategy that broke even though the "input" here is a sequence
// of mutating calls rather than a single argument tuple. RideOp.Apply is pure
// dispatch (which method to call with which arguments) - no matching/queueing
// logic of its own.
public sealed class DesignRideSharingSystemTests
{
    public static TheoryData<RideOp[], int[]?[]> Examples =>
        new()
        {
            {
                [
                    RideOp.AddRider(3),
                    RideOp.AddDriver(2),
                    RideOp.AddRider(1),
                    RideOp.Match(),
                    RideOp.AddDriver(5),
                    RideOp.CancelRider(3),
                    RideOp.Match(),
                    RideOp.Match(),
                    RideOp.AddRider(7),
                    RideOp.CancelRider(7),
                    RideOp.AddDriver(9),
                    RideOp.Match(),
                    RideOp.AddRider(8),
                    RideOp.Match(),
                ],
                [
                    null, null, null, [2, 3], null, null, [5, 1], [-1, -1],
                    null, null, null, [-1, -1], null, [9, 8],
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RideSharingSystemByLinearScanQueue_LeetCodeExample_ReturnsFifoMatches(
        RideOp[] operations, int[]?[] expected) =>
        RunScript(new DesignRideSharingSystemSolution.RideSharingSystemByLinearScanQueue(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RideSharingSystemByLazyDeletionQueue_LeetCodeExample_ReturnsFifoMatches(
        RideOp[] operations, int[]?[] expected) =>
        RunScript(new DesignRideSharingSystemSolution.RideSharingSystemByLazyDeletionQueue(), operations, expected);

    private static void RunScript(
        DesignRideSharingSystemSolution.IRideSharingStrategy strategy,
        RideOp[] operations,
        int[]?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }
}
