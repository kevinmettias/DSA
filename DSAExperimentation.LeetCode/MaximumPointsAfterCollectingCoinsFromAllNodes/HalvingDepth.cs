namespace DSAExperimentation.LeetCode.MaximumPointsAfterCollectingCoinsFromAllNodes;

// How deep the halving has to be tracked: coins[i] <= 1e4 < 2^14, so coins[node] >> h
// is already 0 for every h at or past Max, and folding one level deeper cannot change
// a further-halved contribution - the same "once it can't move the answer, stop
// tracking it more finely" bound RoomWaysPrecomputedFactorialAlgebra's Prepare(n)
// table uses, just against a value magnitude instead of a tree size. Separate from
// CoinPointsAlgebra because the solution that reads the folded per-level table names
// the same cap, so one definition of the depth is what keeps the two in step.
internal static class HalvingDepth
{
    public const int Max = 14;
}
