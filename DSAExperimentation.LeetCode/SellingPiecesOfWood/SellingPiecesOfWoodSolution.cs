using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.SellingPiecesOfWood;

// LeetCode 2312. Selling Pieces of Wood: the most money an m x n piece can be cut
// into, given a price list for certain (height, width) pieces. A piece is either
// sold whole at its listed price, or split by one full horizontal or vertical cut
// and each half solved the same way.
//
// That is an interval DP over piece dimensions - the same (Left, Right)-keyed
// Memoizer<TState, TResult> shape MinimumScoreTriangulationOfPolygon and
// BurstBalloons already use, generalized to two independent cut axes the same way
// NumberOfWaysOfCuttingAPizza's (Row, Col, RemainingCuts) state generalizes a
// single-axis recurrence. Every listed price is looked up through this repo's own
// HashMap<TKey, TValue>, keyed on the (height, width) ValueTuple.
//
// The recurrence itself is identical in both strategies; the only thing they differ
// in is whether a recursive call is routed through a cache. Return type is long,
// not int: up to 2*10^4 listed prices at up to 10^6 each can sum past Int32.MaxValue,
// the same overflow-law-first reasoning IntegerReplacement's int-vs-long choice
// already documents.
internal static class SellingPiecesOfWoodSolution
{
    // The textbook answer: plain recursion with no cache at all, which is exponential
    // here because the same (height, width) sub-piece recurs across many different
    // earlier cut sequences that land on it. It is the arm the memoized strategy
    // below has to justify itself against.
    public static long SellingWoodByUnmemoizedRecursion(
        int boardHeight, int boardWidth, (int Height, int Width, int Price)[] prices) =>
        SellingWoodByUnmemoizedRecursion(boardHeight, boardWidth, PriceIndex(prices));

    public static long SellingWoodByUnmemoizedRecursion(
        int boardHeight, int boardWidth, HashMap<(int Height, int Width), int> priceByDimensions)
    {
        var plan = new CuttingPlan(priceByDimensions);

        return plan.Replay((boardHeight, boardWidth), plan);
    }

    // The same recurrence through Memoizer, which caches on the (height, width) pair
    // and so evaluates each distinct sub-piece exactly once - boardHeight * boardWidth
    // states, each doing O(boardHeight + boardWidth) work.
    public static long SellingWoodByMemoizedRecursion(
        int boardHeight, int boardWidth, (int Height, int Width, int Price)[] prices) =>
        SellingWoodByMemoizedRecursion(boardHeight, boardWidth, PriceIndex(prices));

    public static long SellingWoodByMemoizedRecursion(
        int boardHeight, int boardWidth, HashMap<(int Height, int Width), int> priceByDimensions) =>
        Memoizer.Memoize<(int Height, int Width), long>(
            (boardHeight, boardWidth),
            new CuttingPlan(priceByDimensions));

    // LeetCode hands the price list as rows; both strategies want O(1) lookup by
    // dimensions, so the rows are indexed once before the recursion starts. Given to
    // the hoisted overloads directly by a benchmark, so the indexing is charged to
    // its setup rather than to the search.
    private static HashMap<(int Height, int Width), int> PriceIndex(
        (int Height, int Width, int Price)[] prices)
    {
        var priceByDimensions = new HashMap<(int Height, int Width), int>();

        foreach (var (height, width, price) in prices)
        {
            priceByDimensions.Set((height, width), price);
        }

        return priceByDimensions;
    }

    // The rule, named: a sub-piece is worth whichever is larger - its own listed price
    // sold whole (0 when unlisted, which is what TryGetValue's out parameter already
    // leaves behind on a miss), or the best single cut across either axis. The same named
    // rule answers both strategies: the memoized arm hands it the cache-backed recursion,
    // and the un-memoized arm replays it straight back into itself with no cache in
    // between. The prices are the whole of what the rule needs from its caller, so they
    // are the constructor's only input.
    private sealed class CuttingPlan(HashMap<(int Height, int Width), int> priceByDimensions)
        : IRecurrence<(int Height, int Width), long>
    {
        public long Replay((int Height, int Width) piece, IRecurrence<(int Height, int Width), long> rest)
        {
            long whole = priceByDimensions.TryGetValue(piece, out var listedPrice) ? listedPrice : 0;

            var bestHorizontalCut = BestCutAlongAxis(CutAxis.Horizontal, piece, rest);
            var bestVerticalCut = BestCutAlongAxis(CutAxis.Vertical, piece, rest);
            var bestCut = Math.Max(bestHorizontalCut, bestVerticalCut);

            return Math.Max(whole, bestCut);
        }
    }

    // Which dimension a single full cut divides: a horizontal cut divides the height and
    // leaves two pieces of the same width, a vertical cut divides the width and leaves
    // two pieces of the same height.
    private enum CutAxis
    {
        Horizontal,
        Vertical,
    }

    // One axis of cut positions: every place a single full cut can fall across a span
    // of that length, worth whatever the two halves it leaves are worth together. The
    // two axes are the same search - only what a cut position means differs - so they
    // share this walk rather than writing it out twice, the same shape
    // NumberOfWaysOfCuttingAPizzaSolution's own SumCutsAlongAxis takes. A span of 1
    // admits no cut at all, leaving 0, which never beats selling the piece whole.
    private static long BestCutAlongAxis(
        CutAxis axis,
        (int Height, int Width) piece,
        IRecurrence<(int Height, int Width), long> rest)
    {
        var span = axis == CutAxis.Horizontal ? piece.Height : piece.Width;
        var best = 0L;

        for (var cut = 1; cut < span; cut++)
        {
            var (firstHalf, secondHalf) = HalvesOfCut(axis, piece, cut);
            best = Math.Max(best, rest.Replay(firstHalf, rest) + rest.Replay(secondHalf, rest));
        }

        return best;
    }

    // The two pieces a single cut at `cut` leaves, named by which dimension the cut
    // divides: a horizontal cut shortens the height, a vertical cut shortens the width.
    private static ((int Height, int Width) First, (int Height, int Width) Second) HalvesOfCut(
        CutAxis axis, (int Height, int Width) piece, int cut)
    {
        if (axis == CutAxis.Horizontal)
        {
            return ((cut, piece.Width), (piece.Height - cut, piece.Width));
        }

        return ((piece.Height, cut), (piece.Height, piece.Width - cut));
    }
}
