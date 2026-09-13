using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfWaysOfCuttingAPizza;

// LeetCode 1444. Number of Ways of Cutting a Pizza: count the ways to make k - 1
// horizontal/vertical cuts so that every one of the k pieces holds at least one
// apple, modulo 1e9+7.
//
// A cut always keeps the bottom-right remainder, so the whole problem is a
// recurrence over (Row, Col, RemainingCuts) - the top-left corner of what is left
// and how many cuts are still owed - and AppleGrid's suffix sums answer "did this
// slice take an apple with it" in O(1). Both strategies walk exactly that
// recurrence over exactly that table; they differ only in whether repeated states
// are cached, the same un-memoized-vs-Memoizer split CherryPickupSolution and
// BurstBalloonsSolution already use.
//
// The induced state graph is well-founded for Memoizer's own precondition:
// RemainingCuts strictly decreases on every recursive call, so no state can recur
// on its own path.
internal static class NumberOfWaysOfCuttingAPizzaSolution
{
    // A pizza with no apple left in it cannot be cut into apple-bearing pieces.
    private const int NoWays = 0;

    // With no cuts owed, the remaining pizza is itself the last piece - one way.
    private const int WholeRemainderIsOnePiece = 1;

    // The textbook answer: plain recursion over (Row, Col, RemainingCuts) with no
    // cache - exponential, because the same state is reached again through many
    // different earlier cut sequences. Deliberately written without this repo's
    // primitives; it is the arm the composed strategy below has to justify itself
    // against.
    public static int CountWaysByUnmemoizedRecursion(string[] pizza, int k) =>
        CountWaysByUnmemoizedRecursion(new AppleGrid(pizza), k);

    public static int CountWaysByUnmemoizedRecursion(AppleGrid apples, int k) =>
        WaysFromUnmemoized(apples, (0, 0, k - 1));

    private static int WaysFromUnmemoized(AppleGrid apples, (int Row, int Col, int RemainingCuts) state) =>
        WaysFrom(apples, state, next => WaysFromUnmemoized(apples, next));

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the
    // (Row, Col, RemainingCuts) triple - collapsing the exponential walk above to
    // the Rows * Cols * k distinct states.
    public static int CountWaysByMemoizedRecursion(string[] pizza, int k) =>
        CountWaysByMemoizedRecursion(new AppleGrid(pizza), k);

    public static int CountWaysByMemoizedRecursion(AppleGrid apples, int k) =>
        Memoizer.Memoize<(int Row, int Col, int RemainingCuts), int>(
            (0, 0, k - 1),
            (state, waysFrom) => WaysFrom(apples, state, waysFrom));

    // The recurrence itself, shared by both strategies so that the only thing they
    // differ in is how the recursive call reaches back in.
    private static int WaysFrom(
        AppleGrid apples,
        (int Row, int Col, int RemainingCuts) state,
        Func<(int Row, int Col, int RemainingCuts), int> waysFrom)
    {
        var (row, col, remainingCuts) = state;

        if (apples.ApplesFrom(row, col) == 0)
        {
            return NoWays;
        }

        if (remainingCuts == 0)
        {
            return WholeRemainderIsOnePiece;
        }

        var horizontalWays = SumCutsAlongAxis(
            apples.ApplesFrom(row, col),
            (row + 1, apples.Rows),
            nextRow => apples.ApplesFrom(nextRow, col),
            nextRow => waysFrom((nextRow, col, remainingCuts - 1)));

        var verticalWays = SumCutsAlongAxis(
            apples.ApplesFrom(row, col),
            (col + 1, apples.Cols),
            nextCol => apples.ApplesFrom(row, nextCol),
            nextCol => waysFrom((row, nextCol, remainingCuts - 1)));

        return (int)((horizontalWays + verticalWays) % ModularArithmetic.Modulo);
    }

    // One axis of cut positions: a cut at `next` is legal only when the piece it
    // slices off - the difference between the two suffix sums - holds an apple.
    private static long SumCutsAlongAxis(
        int applesRemaining,
        (int Start, int Bound) range,
        Func<int, int> applesFrom,
        Func<int, int> waysAt)
    {
        var total = 0L;

        for (var next = range.Start; next < range.Bound; next++)
        {
            if (applesRemaining - applesFrom(next) > 0)
            {
                total = (total + waysAt(next)) % ModularArithmetic.Modulo;
            }
        }

        return total;
    }
}
