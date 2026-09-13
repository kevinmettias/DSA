namespace DSAExperimentation.LeetCode.RaceCar;

// The bound that makes LC 818 finite. The car's (position, speed) states are
// otherwise unbounded - speed doubles on every 'A' - so both strategies search
// inside the same generous, target-scaled box: |speed| never needs to exceed the
// smallest power of two past 4*target (ample headroom to overshoot and correct with
// an 'R'), and position stays within that same margin either side of zero.
//
// Describing the box separately from RaceCarStateGraph is what lets the textbook
// arm stay textbook: it needs the bound, not a materialized graph.
internal readonly record struct RaceCarStateSpace(int PositionBound, int MaxSpeedMagnitude)
{
    // Every run starts at the origin, already moving forward at speed 1.
    public const int StartPosition = 0;
    public const int StartSpeed = 1;

    // An 'A' command doubles the current speed.
    public const int SpeedDoublingFactor = 2;

    private const int PositionBoundMultiplier = 4;
    private const int PositionBoundPadding = 2;

    public static RaceCarStateSpace For(int target)
    {
        var positionBound = (PositionBoundMultiplier * target) + PositionBoundPadding;

        return new RaceCarStateSpace(positionBound, SmallestPowerOfTwoAtLeast(positionBound));
    }

    public bool Contains(int position, int speed) =>
        Math.Abs(position) <= PositionBound && Math.Abs(speed) <= MaxSpeedMagnitude;

    // Both signs of every reachable speed magnitude: speeds only ever start at +-1
    // and double, so every magnitude in range is a power of two.
    public List<int> Speeds()
    {
        var speeds = new List<int>();

        for (var magnitude = StartSpeed; magnitude <= MaxSpeedMagnitude; magnitude *= SpeedDoublingFactor)
        {
            speeds.Add(magnitude);
            speeds.Add(-magnitude);
        }

        return speeds;
    }

    private static int SmallestPowerOfTwoAtLeast(int bound)
    {
        var magnitude = StartSpeed;

        while (magnitude < bound)
        {
            magnitude *= SpeedDoublingFactor;
        }

        return magnitude;
    }
}
