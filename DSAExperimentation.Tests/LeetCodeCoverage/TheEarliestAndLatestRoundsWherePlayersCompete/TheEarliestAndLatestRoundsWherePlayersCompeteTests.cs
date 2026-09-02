using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheEarliestAndLatestRoundsWherePlayersCompete;

// LeetCode 1900. The Earliest and Latest Rounds Where Players Compete: memoized
// recursion over (roundSize, low, high) via this repo's own Memoizer - the same
// tuple-state shape CatAndMouseIITests/MinimumCostToConnectTwoGroupsOfPointsTests
// already use, just for tournament-bracket round counting instead of a grid game or
// bitmask assignment.
//
// Positions low/high must both survive every round (they have to, to ever meet);
// every OTHER player's win/loss is a free choice searched over. Since those other
// players are otherwise interchangeable, only the COUNT of survivors landing below
// low (x) and below high (y) matters for next round's (low', high'), not their
// identities. That count splits cleanly by zone:
//  - "Left" zone (positions < low, size low-1): each one's mirror partner
//    (position n+1-p) always lands in the "Right" zone (> high), so each is a free
//    0/1 contribution to survivors below BOTH low and high.
//  - "Mid" zone (low < . < high): if high is itself a pairing "low" member (its
//    partner n+1-high lands in the Right zone), every Mid position pairs outward
//    too, same free-0/1 shape (contributing only to the <high count). If high is a
//    "high" member instead, its partner sits inside Mid, splitting the rest of Mid
//    into a free Mid-Right region and a self-paired Mid-Mid region whose survivors
//    are forced (one per pair, always < high). For odd n, the automatic-bye middle
//    player is either high itself (no pairing needed at all) or sits inside that
//    self-paired region, contributing one more forced survivor.
// nextLow = 1 + x, nextHigh = 2 + x + y + fixedMidSurvivors (the "+2" baseline
// counts low itself, which is always < high, plus the 1-indexed rank offset).
//
// Recurrence hand-verified against both official LeetCode examples below.
public sealed partial class TheEarliestAndLatestRoundsWherePlayersCompeteTests
{
    [Theory]
    [InlineData(11, new[] { 2, 4 }, 3, 4)]
    [InlineData(10, new[] { 3, 4 }, 4, 4)]
    public void EarliestAndLatest_LeetCodeExamples_ReturnsExpectedRoundBounds(
        int n, int[] players, int expectedEarliest, int expectedLatest)
    {
        var (earliest, latest) = EarliestAndLatest(n, players[0], players[1]);

        Assert.Equal(expectedEarliest, earliest);
        Assert.Equal(expectedLatest, latest);
    }

    private static (int Earliest, int Latest) EarliestAndLatest(int n, int firstPlayer, int secondPlayer)
    {
        var low = Math.Min(firstPlayer, secondPlayer);
        var high = Math.Max(firstPlayer, secondPlayer);

        return Memoizer.Memoize<(int N, int Low, int High), (int Earliest, int Latest)>((n, low, high), Recurrence);
    }

    private static (int Earliest, int Latest) Recurrence(
        (int N, int Low, int High) state,
        Func<(int N, int Low, int High), (int Earliest, int Latest)> solve)
    {
        var (roundSize, l, h) = state;

        if (l + h > roundSize + 1)
        {
            // Reflection symmetry: relabeling every position p as roundSize+1-p
            // is an automorphism of the bracket, so this is the same game.
            return solve((roundSize, roundSize + 1 - h, roundSize + 1 - l));
        }

        if (l + h == roundSize + 1)
        {
            return (1, 1);
        }

        var leftFree = l - 1;
        var (midFree, midFixed) = ComputeMidZone(l, h, roundSize);
        var nextRoundSize = (roundSize + 1) / 2;
        var zones = new NextRoundZones(leftFree, midFree, midFixed, nextRoundSize);

        return SearchNextRoundBounds(zones, solve);
    }

    // Mid zone (positions strictly between low and high) splits by where high
    // itself falls in its own mirror pairing - see the class remarks above for
    // the full derivation of each branch.
    private static (int MidFree, int MidFixed) ComputeMidZone(int l, int h, int roundSize)
    {
        var isOdd = roundSize % 2 == 1;

        if (IsAutomaticByeMiddle(h, roundSize, isOdd))
        {
            // high is the automatic-bye middle player: every other pair index
            // up to half is "other" and pairs outward into the Right zone.
            return (roundSize / 2 - l, 0);
        }

        var partner = Math.Min(h, roundSize + 1 - h);
        if (partner == h)
        {
            // high is a pairing "low" member: all of Mid pairs outward.
            return (h - l - 1, 0);
        }

        return ComputeSplitMidZone(l, h, partner, isOdd);
    }

    private static bool IsAutomaticByeMiddle(int h, int roundSize, bool isOdd)
        => isOdd && h == (roundSize + 1) / 2;

    // high is a "high" member; its partner sits inside Mid, splitting the
    // rest of Mid into a free region (pairs outward) and a self-paired
    // region (forced survivors, one per pair, plus the odd-n bye if it
    // falls in that region).
    private static (int MidFree, int MidFixed) ComputeSplitMidZone(int l, int h, int partner, bool isOdd)
    {
        var midFree = partner - l - 1;
        var pairableInnerRegion = h - 1 - partner - (isOdd ? 1 : 0);
        var midFixed = (pairableInnerRegion / 2) + (isOdd ? 1 : 0);
        return (midFree, midFixed);
    }

    private static (int Earliest, int Latest) SearchNextRoundBounds(
        NextRoundZones zones,
        Func<(int N, int Low, int High), (int Earliest, int Latest)> solve)
    {
        var earliest = int.MaxValue;
        var latest = int.MinValue;

        for (var x = 0; x <= zones.LeftFree; x++)
        {
            for (var y = 0; y <= zones.MidFree; y++)
            {
                var nextLow = 1 + x;
                var nextHigh = 2 + x + y + zones.MidFixed;

                var (e, la) = solve((zones.NextRoundSize, nextLow, nextHigh));
                earliest = Math.Min(earliest, e + 1);
                latest = Math.Max(latest, la + 1);
            }
        }

        return (earliest, latest);
    }

    private readonly record struct NextRoundZones(int LeftFree, int MidFree, int MidFixed, int NextRoundSize);
}
