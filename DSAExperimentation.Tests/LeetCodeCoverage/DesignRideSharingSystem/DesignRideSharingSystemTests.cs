using static DSAExperimentation.LeetCode.DesignRideSharingSystem.DesignRideSharingSystemSolution;

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
        RunScript(new RideSharingSystemByLinearScanQueue(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RideSharingSystemByLazyDeletionQueue_LeetCodeExample_ReturnsFifoMatches(
        RideOp[] operations, int[]?[] expected) =>
        RunScript(new RideSharingSystemByLazyDeletionQueue(), operations, expected);

    private static void RunScript(IRideSharingStrategy strategy, RideOp[] operations, int[]?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }
}

// One call in a RideSharingSystem script: which method to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct RideOp
{
    private readonly Kind _kind;
    private readonly int _id;

    private RideOp(Kind kind, int id)
    {
        _kind = kind;
        _id = id;
    }

    public static RideOp AddRider(int riderId) => new(Kind.AddRider, riderId);

    public static RideOp AddDriver(int driverId) => new(Kind.AddDriver, driverId);

    public static RideOp CancelRider(int riderId) => new(Kind.CancelRider, riderId);

    public static RideOp Match() => new(Kind.Match, 0);

    // null for the three void calls, the returned [driverId, riderId] pair (or
    // [-1, -1]) for Match - so a script runner can assert against one expected
    // value per operation uniformly. Internal, not public: IRideSharingStrategy is
    // internal to DesignRideSharingSystemSolution, and only this same assembly's
    // RunScript ever calls Apply.
    internal int[]? Apply(IRideSharingStrategy strategy)
    {
        switch (_kind)
        {
            case Kind.AddRider:
                strategy.AddRider(_id);
                return null;
            case Kind.AddDriver:
                strategy.AddDriver(_id);
                return null;
            case Kind.CancelRider:
                strategy.CancelRider(_id);
                return null;
            default:
                return strategy.MatchDriverWithRider();
        }
    }

    private enum Kind
    {
        AddRider,
        AddDriver,
        CancelRider,
        Match,
    }
}
