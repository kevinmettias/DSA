namespace DSAExperimentation.LeetCode.PeaksInArrayII;

// One segment-tree node's aggregate over a range of positions: whether it holds
// any peak, its leftmost/rightmost peak position if so, and InteriorGapPairs -
// the summed subarray-pair count of every no-peak gap strictly between two
// consecutive peaks already inside this range. Two ranges merge by bridging
// their own leftmost/rightmost peaks into one more such gap
// (PeakGapMergeOperation.cs) - the same "track boundary elements, fold
// everything else into one running total" shape NonAdjacentSumNode/
// NonAdjacentMergeOperation already use for LC 3165, here tracking peak
// boundaries and gap-pair counts instead of DP states. Answers LC 4017 alone,
// which is why it lives beside the solution rather than in DataStructures.
internal readonly record struct PeakGapNode(bool HasPeak, int MinPeak, int MaxPeak, long InteriorGapPairs)
{
    public static PeakGapNode None { get; } = new(false, 0, 0, 0);

    public static PeakGapNode Leaf(bool isPeak, int index) => isPeak ? new PeakGapNode(true, index, index, 0) : None;

    // Number of length->=3 subarrays wholly inside a run of `gap` consecutive
    // positions with no peak between them - C(gap, 2), the same "positions at
    // distance >= 2 apart" count the solution's own total-subarrays formula
    // uses at a query's [l, r] scale, reused here at the scale of one no-peak
    // gap between two peak boundaries (or a peak boundary and the query edge).
    public static long PairsInGap(long gap) => gap < 2 ? 0 : ChooseTwo(gap);

    // C(gap, 2): the unordered position pairs inside a run of `gap` positions.
    private static long ChooseTwo(long gap) => gap * (gap - 1) / 2;
}
