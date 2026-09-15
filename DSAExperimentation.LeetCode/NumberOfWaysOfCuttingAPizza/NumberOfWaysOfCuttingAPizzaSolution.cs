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

    public static int CountWaysByUnmemoizedRecursion(AppleGrid apples, int k)
    {
        var ways = new PizzaCutWays(apples);

        return ways.Replay((0, 0, k - 1), ways);
    }

    // This repo's own Memoizer<TState,TResult> supplies the cache, keyed by the
    // (Row, Col, RemainingCuts) triple - collapsing the exponential walk above to
    // the Rows * Cols * k distinct states.
    public static int CountWaysByMemoizedRecursion(string[] pizza, int k) =>
        CountWaysByMemoizedRecursion(new AppleGrid(pizza), k);

    public static int CountWaysByMemoizedRecursion(AppleGrid apples, int k) =>
        Memoizer.Memoize<(int Row, int Col, int RemainingCuts), int>(
            (0, 0, k - 1),
            new PizzaCutWays(apples));

    // The recurrence, as a named type. It forwards whatever recursion it is handed
    // straight into the shared body below, so the whole difference between the two
    // strategies is what plays that part: the memo run, which holds the cache, or this
    // same type naming itself, which holds nothing.
    private sealed class PizzaCutWays(AppleGrid apples)
        : IRecurrence<(int Row, int Col, int RemainingCuts), int>
    {
        public int Replay(
            (int Row, int Col, int RemainingCuts) state,
            IRecurrence<(int Row, int Col, int RemainingCuts), int> rest) =>
            WaysFrom(apples, state, rest);
    }

    // The recurrence itself, shared by both strategies so that the only thing they
    // differ in is how the recursive call reaches back in.
    private static int WaysFrom(
        AppleGrid apples,
        (int Row, int Col, int RemainingCuts) state,
        IRecurrence<(int Row, int Col, int RemainingCuts), int> rest)
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

        var (horizontalWays, verticalWays) = SumCutsAlongBothAxes(apples, state, rest);

        return (int)((horizontalWays + verticalWays) % ModularArithmetic.Modulo);
    }

    // Both axes of the recurrence: a cut may run below the current row or right of
    // the current column, and the two totals add up to the ways at this state.
    private static (long Horizontal, long Vertical) SumCutsAlongBothAxes(
        AppleGrid apples,
        (int Row, int Col, int RemainingCuts) state,
        IRecurrence<(int Row, int Col, int RemainingCuts), int> rest)
    {
        var (row, col, remainingCuts) = state;
        var applesRemaining = apples.ApplesFrom(row, col);

        var horizontalWays = SumCutsAlongAxis(
            applesRemaining,
            (row + 1, apples.Rows),
            new RowCutAxis(apples, col, remainingCuts - 1),
            rest);

        var verticalWays = SumCutsAlongAxis(
            applesRemaining,
            (col + 1, apples.Cols),
            new ColumnCutAxis(apples, row, remainingCuts - 1),
            rest);

        return (horizontalWays, verticalWays);
    }

    // One axis of cut positions: a cut at `next` is legal only when the piece it
    // slices off - the difference between the two suffix sums - holds an apple.
    private static long SumCutsAlongAxis(
        int applesRemaining,
        (int Start, int Bound) range,
        IPizzaCutAxis axis,
        IRecurrence<(int Row, int Col, int RemainingCuts), int> rest)
    {
        var total = 0L;

        for (var next = range.Start; next < range.Bound; next++)
        {
            if (applesRemaining - axis.ApplesFrom(next) > 0)
            {
                total = (total + rest.Replay(axis.NextState(next), rest))
                    % ModularArithmetic.Modulo;
            }
        }

        return total;
    }

    // One axis of the remaining pizza, with the other coordinate held fixed: the apples
    // each strip still holds, and the state a cut at a position leaves. Which of the two
    // axes is being measured is the whole of what the implementations differ in, so the
    // type says which one and its methods say what comes back.
    private interface IPizzaCutAxis
    {
        // Apples still in the strip that starts at `position` and runs to the far
        // edge, along this instance's own axis.
        int ApplesFrom(int position);

        // The state a cut at `position` leaves: the coordinate this axis moves takes the
        // cut's own position, and the cut it took is one fewer still owed.
        (int Row, int Col, int RemainingCuts) NextState(int position);
    }

    // A fixed column read down the rows, for the horizontal-cut axis.
    private sealed class RowCutAxis(AppleGrid apples, int col, int cutsAfterThisOne) : IPizzaCutAxis
    {
        public int ApplesFrom(int position) => apples.ApplesFrom(position, col);

        public (int Row, int Col, int RemainingCuts) NextState(int position) =>
            (position, col, cutsAfterThisOne);
    }

    // A fixed row read across the columns, for the vertical-cut axis.
    private sealed class ColumnCutAxis(AppleGrid apples, int row, int cutsAfterThisOne) : IPizzaCutAxis
    {
        public int ApplesFrom(int position) => apples.ApplesFrom(row, position);

        public (int Row, int Col, int RemainingCuts) NextState(int position) =>
            (row, position, cutsAfterThisOne);
    }
}
