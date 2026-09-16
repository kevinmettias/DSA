using DSAExperimentation.LeetCode.DesignRideSharingSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignRideSharingSystem;

// One call in a RideSharingSystem script: which method to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script (like
// Examples in DesignRideSharingSystemTests) reads like the LeetCode call sequence
// it replays.
public readonly record struct RideOp(RideOp.OpKind kind, int id)
{
    public static RideOp AddRider(int riderId) => new(OpKind.AddRider, riderId);

    public static RideOp AddDriver(int driverId) => new(OpKind.AddDriver, driverId);

    public static RideOp CancelRider(int riderId) => new(OpKind.CancelRider, riderId);

    public static RideOp Match() => new(OpKind.Match, 0);

    // null for the three void calls, the returned [driverId, riderId] pair (or
    // [-1, -1]) for Match - so a script runner can assert against one expected
    // value per operation uniformly. Internal, not public: IRideSharingStrategy is
    // internal to DesignRideSharingSystemSolution, and only this same assembly's
    // RunScript ever calls Apply.
    internal int[]? Apply(DesignRideSharingSystemSolution.IRideSharingStrategy strategy)
    {
        switch (kind)
        {
            case OpKind.AddRider:
                strategy.AddRider(id);
                return null;
            case OpKind.AddDriver:
                strategy.AddDriver(id);
                return null;
            case OpKind.CancelRider:
                strategy.CancelRider(id);
                return null;
            default:
                return strategy.MatchDriverWithRider();
        }
    }

    public enum OpKind
    {
        AddRider,
        AddDriver,
        CancelRider,
        Match,
    }
}
