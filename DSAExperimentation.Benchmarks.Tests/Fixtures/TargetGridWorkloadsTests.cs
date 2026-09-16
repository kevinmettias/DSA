using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for TargetGridWorkloads (ARCHITECTURE 17.7): LC 3609's target is reachable from the
// start only along one exact move rule, so the fixture replays forward moves from a fixed start rather
// than picking coordinates. The test derives reachability independently - it enumerates the same rule's
// coordinates rather than reusing the generator - so a workload that handed back an unreachable pair
// would fail here instead of measuring the strategies on an out-of-contract input.
public sealed partial class TargetGridWorkloadsTests
{
    private const int MoveCount = 10;
    private const int Seed = 3609; // LC problem number
    private const int StartX = 1;
    private const int StartY = 1;

    [Fact]
    public void BuildReachablePair_SeededMoves_StartsFromTheFixedOrigin()
    {
        var (sx, sy, _, _) = TargetGridWorkloads.BuildReachablePair(MoveCount, Seed);

        Assert.Equal(StartX, sx);
        Assert.Equal(StartY, sy);
    }

    [Fact]
    public void BuildReachablePair_SeededMoves_LandsOnACoordinateTheMoveRuleActuallyReaches()
    {
        var (_, _, tx, ty) = TargetGridWorkloads.BuildReachablePair(MoveCount, Seed);

        Assert.Contains(((long)tx, (long)ty), ReachableWithin(MoveCount));
    }

    // The pair is the farthest coordinate the replay ended on, so it is reached at exactly the
    // requested number of moves - not merely at some shorter distance.
    [Fact]
    public void BuildReachablePair_SeededMoves_LandsOnACoordinateNoShorterReplayCould() =>
        Assert.Equal(
            MoveCount,
            ShortestMoveCount(TargetGridWorkloads.BuildReachablePair(MoveCount, Seed)));

    [Fact]
    public void BuildReachablePair_SameSeed_ReturnsTheSamePair() =>
        Assert.Equal(
            TargetGridWorkloads.BuildReachablePair(MoveCount, Seed),
            TargetGridWorkloads.BuildReachablePair(MoveCount, Seed));

    // Every coordinate the rule reaches in at most `moveCount` forward moves - the rule applied
    // directly, one coordinate per state, so it never reads the fixture's own replay.
    private static HashSet<(long X, long Y)> ReachableWithin(int moveCount)
    {
        var frontier = new HashSet<(long X, long Y)> { (StartX, StartY) };
        var seen = new HashSet<(long X, long Y)>(frontier);

        for (var step = 0; step < moveCount; step++)
        {
            var next = new HashSet<(long X, long Y)>();

            foreach (var (x, y) in frontier)
            {
                var m = Math.Max(x, y);
                next.Add((x + m, y));
                next.Add((x, y + m));
            }

            seen.UnionWith(next);
            frontier = next;
        }

        return seen;
    }

    private static int ShortestMoveCount((int Sx, int Sy, int Tx, int Ty) pair)
    {
        for (var moveCount = 0; moveCount <= MoveCount; moveCount++)
        {
            if (ReachableWithin(moveCount).Contains((pair.Tx, pair.Ty)))
            {
                return moveCount;
            }
        }

        return -1;
    }
}
