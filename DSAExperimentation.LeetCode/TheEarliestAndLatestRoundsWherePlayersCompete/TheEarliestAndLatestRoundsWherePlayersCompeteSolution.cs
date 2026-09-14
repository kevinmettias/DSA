using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.TheEarliestAndLatestRoundsWherePlayersCompete;

// LeetCode 1900. The Earliest and Latest Rounds Where Players Compete: in a
// bracket where a round pairs position p against position (size + 1 - p), find the
// first and last round in which the two tracked players can meet.
//
// Both strategies walk the same recurrence over (roundSize, low, high) and differ
// only in whether revisited states are cached: the baseline re-explores every
// branch from scratch, the other routes the identical recurrence through this
// repo's own Memoizer - the same tuple-state shape CatAndMouseII and
// MinimumCostToConnectTwoGroupsOfPoints already use, here for tournament-bracket
// round counting instead of a grid game or a bitmask assignment.
//
// The transition itself: positions low/high must both survive every round (they
// have to, to ever meet); every OTHER player's win/loss is a free choice searched
// over. Since those other players are otherwise interchangeable, only the COUNT of
// survivors landing below low (x) and below high (y) matters for next round's
// (low', high'), not their identities. That count splits cleanly by zone:
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
// nextLow = 1 + x, nextHigh = 2 + x + y + midFixed (the "+2" baseline counts low
// itself, which is always below high, plus the 1-indexed rank offset).
//
// Recurrence hand-verified against both official LeetCode examples, which the
// coverage tests state.
internal static class TheEarliestAndLatestRoundsWherePlayersCompeteSolution
{
    private const int HalvingFactor = 2;

    // Next round's low is 1 + x; its high starts from low itself plus the
    // 1-indexed rank offset, before any of the searched-over survivors are added.
    private const int HighSeedBaseOffset = 2;

    // The textbook answer without this repo: the same recurrence calling itself,
    // so every (roundSize, low, high) state reachable down more than one branch is
    // recomputed from scratch. Pure BCL - it is the arm the memoized version below
    // has to justify itself against.
    public static (int Earliest, int Latest) EarliestAndLatestByPlainRecursion(
        int playerCount, int firstPlayer, int secondPlayer)
    {
        var (low, high) = Ordered(firstPlayer, secondPlayer);

        return SolveUncached(playerCount, low, high);
    }

    private static (int Earliest, int Latest) SolveUncached(int roundSize, int low, int high)
        => RoundBounds(new BracketState(roundSize, low, high), SolveUncached);

    // The same recurrence routed through Memoizer, whose cache collapses the many
    // elimination choices among the other players that reach an identical
    // (roundSize, low, high) state onto a single evaluation.
    public static (int Earliest, int Latest) EarliestAndLatestByMemoizedRecurrence(
        int playerCount, int firstPlayer, int secondPlayer)
    {
        var (low, high) = Ordered(firstPlayer, secondPlayer);

        return Memoizer.Memoize<(int RoundSize, int Low, int High), (int Earliest, int Latest)>(
            (playerCount, low, high), Recurrence);
    }

    private static (int Earliest, int Latest) Recurrence(
        (int RoundSize, int Low, int High) state,
        Func<(int RoundSize, int Low, int High), (int Earliest, int Latest)> solve)
    {
        var (roundSize, low, high) = state;

        return RoundBounds(
            new BracketState(roundSize, low, high),
            (nextSize, nextLow, nextHigh) => solve((nextSize, nextLow, nextHigh)));
    }

    private static (int Low, int High) Ordered(int firstPlayer, int secondPlayer)
        => (Math.Min(firstPlayer, secondPlayer), Math.Max(firstPlayer, secondPlayer));

    // Shared shape between the plain self-recursion and the memoized recurrence:
    // normalize by mirror symmetry, stop once the two tracked players must already
    // have met, otherwise fan out over every elimination choice for the other
    // survivors.
    private static (int Earliest, int Latest) RoundBounds(
        BracketState state, Func<int, int, int, (int Earliest, int Latest)> solve)
    {
        var (roundSize, low, high) = state;

        if (low + high > roundSize + 1)
        {
            // Reflection symmetry: relabeling every position p as roundSize+1-p is
            // an automorphism of the bracket, so this is the same game.
            return solve(roundSize, roundSize + 1 - high, roundSize + 1 - low);
        }

        if (low + high == roundSize + 1)
        {
            return (1, 1);
        }

        var zones = Transition(state) with { NextRoundSize = (roundSize + 1) / HalvingFactor };

        return ExploreRound(zones, solve);
    }

    private static (int Earliest, int Latest) ExploreRound(
        NextRoundZones zones, Func<int, int, int, (int Earliest, int Latest)> solve)
    {
        var earliest = int.MaxValue;
        var latest = int.MinValue;

        for (var x = 0; x <= zones.LeftFree; x++)
        {
            for (var y = 0; y <= zones.MidFree; y++)
            {
                var nextHigh = HighSeedBaseOffset + x + y + zones.MidFixed;
                var (roundEarliest, roundLatest) = solve(zones.NextRoundSize, 1 + x, nextHigh);
                earliest = Math.Min(earliest, roundEarliest + 1);
                latest = Math.Max(latest, roundLatest + 1);
            }
        }

        return (earliest, latest);
    }

    // Zone counting (see the class remarks for the derivation): how many "other"
    // survivors can freely land below low (LeftFree) and below high (MidFree, plus
    // the MidFixed forced ones from Mid-Mid pairs and the odd-n bye) after one
    // round. NextRoundSize is filled in by the caller, which owns the halving.
    private static NextRoundZones Transition(BracketState state)
    {
        var (midFree, midFixed) = MidZone(state);

        return new NextRoundZones(state.Low - 1, midFree, midFixed, 0);
    }

    // The Mid zone (positions strictly between low and high) splits by where high
    // itself falls in its own mirror pairing.
    private static (int MidFree, int MidFixed) MidZone(BracketState state)
    {
        var (roundSize, low, high) = state;

        if (state.HasByeMiddle && high == (roundSize + 1) / HalvingFactor)
        {
            // high is the automatic-bye middle player: every other position up to
            // half is "other" and pairs outward into the Right zone.
            return ((roundSize / HalvingFactor) - low, 0);
        }

        var partner = Math.Min(high, roundSize + 1 - high);

        if (partner == high)
        {
            // high is a pairing "low" member, so all of Mid pairs outward too.
            return (high - low - 1, 0);
        }

        return SplitMidZone(state, partner);
    }

    // high is a "high" member; its partner sits inside Mid, splitting the rest of
    // Mid into a free region (pairs outward) and a self-paired region (forced
    // survivors, one per pair, plus the odd-n bye if it lands there).
    private static (int MidFree, int MidFixed) SplitMidZone(BracketState state, int partner)
    {
        var bye = state.HasByeMiddle ? 1 : 0;
        var pairableInnerRegion = state.High - 1 - partner - bye;
        var midFixed = (pairableInnerRegion / HalvingFactor) + bye;

        return (partner - state.Low - 1, midFixed);
    }

    private readonly record struct BracketState(int RoundSize, int Low, int High)
    {
        // An odd round leaves one player unpaired, who advances automatically.
        public bool HasByeMiddle => RoundSize % HalvingFactor == 1;
    }

    private readonly record struct NextRoundZones(int LeftFree, int MidFree, int MidFixed, int NextRoundSize);
}
