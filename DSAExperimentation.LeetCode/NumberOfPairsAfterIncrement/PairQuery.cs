namespace DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

// One parsed LC 3943 query: either "add Delta to nums2[Left..Right]" or "count
// pairs summing to Tot". LeetCode hands both shapes down one jagged int[][], so
// this is the hoisted-overload input (ARCHITECTURE.md 17.4) both
// NumberOfPairsAfterIncrementSolution strategies share - a benchmark charges the
// int[] -> PairQuery parse to [GlobalSetup] instead of the measured method, and
// a plain record struct (not a BCL collection) keeps the hoisted overload
// unambiguous with the LeetCode-shaped int[][] one.
internal readonly record struct PairQuery(PairQueryKind Kind, int Left, int Right, long Delta, long Tot)
{
    public static PairQuery Increment(int left, int right, int delta) =>
        new(PairQueryKind.Increment, left, right, delta, Tot: 0);

    public static PairQuery Count(long tot) =>
        new(PairQueryKind.Count, Left: 0, Right: 0, Delta: 0, tot);
}

internal enum PairQueryKind
{
    Increment,
    Count,
}
