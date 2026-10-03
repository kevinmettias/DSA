using DSAExperimentation.LeetCode.DesignRideSharingSystem;

namespace DSAExperimentation.LeetCode.Tests.DesignRideSharingSystem;

// Harness only. Both strategies are DesignRideSharingSystemSolution's - this file
// replays LeetCode's two published call sequences, plus example 1's extended with a
// rider who cancels before any match and a driver left waiting for the next rider,
// against each
// IRideSharingStrategy implementation via a small operation script, so a failure
// still names the strategy that broke even though the "input" here is a sequence
// of mutating calls rather than a single argument tuple. RideOp.Apply is pure
// dispatch (which method to call with which arguments) - no matching/queueing
// logic of its own. Each test constructs the strategy itself, so a script starts at
// LeetCode's first call after "RideSharingSystem" and its expected outputs drop the
// constructor's leading null.
public sealed partial class DesignRideSharingSystemSolutionTests
{
    public static TheoryData<RideOp[], int[]?[]> Examples =>
        new()
        {
            // LeetCode example 1.
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
                ],
                [null, null, null, [2, 3], null, null, [5, 1], [-1, -1]]
            },

            // LeetCode example 2.
            {
                [
                    RideOp.AddRider(8),
                    RideOp.AddDriver(8),
                    RideOp.AddDriver(6),
                    RideOp.Match(),
                    RideOp.AddRider(2),
                    RideOp.CancelRider(2),
                    RideOp.Match(),
                ],
                [null, null, null, [8, 8], null, null, [-1, -1]]
            },

            // Example 1 continued: rider 7 cancels before any driver can take them, so
            // driver 9 finds nobody, and rider 8 is then matched with the waiting driver 9.
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
        Assert.Equal(expected, RunScript(
            new DesignRideSharingSystemSolution.RideSharingSystemByLinearScanQueue(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RideSharingSystemByLazyDeletionQueue_LeetCodeExample_ReturnsFifoMatches(
        RideOp[] operations, int[]?[] expected) =>
        Assert.Equal(expected, RunScript(
            new DesignRideSharingSystemSolution.RideSharingSystemByLazyDeletionQueue(), operations));

    private static int[]?[] RunScript(
        DesignRideSharingSystemSolution.IRideSharingStrategy strategy,
        RideOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(strategy))];
}
