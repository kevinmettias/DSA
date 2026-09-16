using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.SumGame;

// LeetCode 1927. Sum Game: Alice and Bob alternately fill the '?' positions of an
// even-length digit string, Alice first; Alice wins if the two halves end up with
// different digit sums, Bob wins if they are equal. Both play optimally.
//
// All three strategies are the same game, seen from three distances:
//
//   BruteForceRecursion - the textbook minimax, re-deriving every reachable
//                         position once per fill-in order that reaches it;
//   MemoizedMinimax     - the same recursion routed through this repo's own
//                         Memoizer, so each SumGameState is resolved once;
//   ClosedForm          - the rule the recursion collapses to (an odd number of
//                         blanks always wins Alice; otherwise Bob can hold the
//                         halves level exactly when the known-digit difference
//                         already equals 9 per pair of blanks he is owed).
//
// The two recursive arms share one CanMoverForceAliceWin body and one named recurrence:
// they differ only in whether the driver between recursive calls is this repo's Memoizer
// cache or the call stack itself, which is the whole difference the benchmark measures.
internal static class SumGameSolution
{
    private const int MinDigit = 0;
    private const int MaxDigit = 9;

    // Alice moves on even move counts, Bob on odd.
    private const int TurnParityDivisor = 2;

    // Two blanks - one for each player - is what a pair of optimal replies costs,
    // so Bob's guaranteed margin is 9 per PAIR of blanks he is owed.
    private const int OptimalMarginDivisor = 2;

    // The textbook answer: plain recursion with no cache, so a position reached by
    // k different fill-in orders is resolved k times. Only the recurrence's shape is
    // shared with the memoized arm below - nothing here caches - because it is the arm
    // the memoized and closed-form arms have to justify themselves against.
    public static bool CanAliceWinByBruteForceRecursion(string num) =>
        CanAliceWinByBruteForceRecursion(SumGameState.Of(num));

    public static bool CanAliceWinByBruteForceRecursion(SumGameState start)
    {
        var recurrence = new AliceWinsFrom(start.TotalBlanks);

        return recurrence.Replay(start, recurrence);
    }

    // The same recursion, with Memoizer supplying the recursive call-back: the
    // recurrence reads like ordinary recursion and the cache is the library's
    // problem, exactly the shape NimGame/DivisorGame/ChalkboardXorGame use for
    // their own game-theory recurrences.
    public static bool CanAliceWinByMemoizedRecursion(string num) =>
        CanAliceWinByMemoizedRecursion(SumGameState.Of(num));

    public static bool CanAliceWinByMemoizedRecursion(SumGameState start) =>
        Memoizer.Memoize<SumGameState, bool>(start, new AliceWinsFrom(start.TotalBlanks));

    // One loop shape covers both quantifiers: on Alice's turn she wants SOME move
    // whose outcome is her own win, on Bob's turn he wants SOME move whose outcome
    // is his - so "return isAliceTurn as soon as a branch's outcome equals
    // isAliceTurn, else !isAliceTurn" is OR-for-Alice / AND-for-Bob without two
    // separate loops.
    private static bool CanMoverForceAliceWin(
        SumGameState state, int totalBlanks, IRecurrence<SumGameState, bool> rest)
    {
        if (state.TotalBlanks == 0)
        {
            return state.SumDifference != 0;
        }

        var movesMade = totalBlanks - state.TotalBlanks;
        var turn = new MoverTurn(rest, IsAliceTurn: movesMade % TurnParityDivisor == 0);

        return TryFindWinningDigit(state.LeftBlanks, new LeftHalfPlacement(state), turn)
            ?? TryFindWinningDigit(state.RightBlanks, new RightHalfPlacement(state), turn)
            ?? !turn.IsAliceTurn;
    }

    // The recurrence, as a named type: one game-tree resolution over SumGameState,
    // deciding whether the player to move can force Alice's win. The board's original
    // blank total arrives through the primary constructor and the memoized continuation
    // through `rest`, so the recursive call-back is a method on a named type.
    private sealed class AliceWinsFrom(int totalBlanks) : IRecurrence<SumGameState, bool>
    {
        public bool Replay(SumGameState state, IRecurrence<SumGameState, bool> rest)
            => CanMoverForceAliceWin(state, totalBlanks, rest);
    }

    // Whose move it is, together with the recursive call-back that resolves the
    // positions it leads to - carried as one value the way OpenTheLockSolution's own
    // TurnWalk is, so a digit search names one turn instead of a loose flag. The
    // call-back is the named recurrence itself, never a delegate.
    private readonly record struct MoverTurn(IRecurrence<SumGameState, bool> AliceWins, bool IsAliceTurn);

    // Tries every digit that could fill one of the remaining blanks on a given
    // side, returning isAliceTurn as soon as some move settles the game in the
    // current mover's favor - null if no digit on this side achieves that.
    private static bool? TryFindWinningDigit(int blanksOnSide, IDigitPlacement placement, MoverTurn turn)
    {
        if (blanksOnSide == 0)
        {
            return null;
        }

        for (var digit = MinDigit; digit <= MaxDigit; digit++)
        {
            if (turn.AliceWins.Replay(placement.Place(digit), turn.AliceWins) == turn.IsAliceTurn)
            {
                return turn.IsAliceTurn;
            }
        }

        return null;
    }

    // Which side of the board a digit is written on, which is the only thing the two
    // calls into the digit search below differ in: writing on the left moves the
    // difference in the left half's favor, writing the same digit on the right moves
    // it the other way, and each consumes the blank it filled. Naming that choice
    // keeps "some int becomes some state" from being the whole of what a caller of
    // the search is told.
    private interface IDigitPlacement
    {
        // The board after writing `digit` in one of this side's remaining blanks. The
        // caller only ever offers digits 0-9, and only while a blank remains on this
        // side, so every call describes a legal move.
        SumGameState Place(int digit);
    }

    private sealed class LeftHalfPlacement(SumGameState state) : IDigitPlacement
    {
        public SumGameState Place(int digit) => state.FillLeft(digit);
    }

    private sealed class RightHalfPlacement(SumGameState state) : IDigitPlacement
    {
        public SumGameState Place(int digit) => state.FillRight(digit);
    }

    // The rule the recursion above reduces to. An odd blank count hands Alice the
    // last move, and one free digit is always enough to break a tie. With an even
    // count the players trade replies: whoever is behind answers a 9 with a 9, so
    // the side holding more blanks can shift the difference by exactly 9 per pair
    // of blanks it is owed, and Bob survives precisely when the known digits
    // already sit at that difference.
    public static bool CanAliceWinByClosedForm(string num) => CanAliceWinByClosedForm(SumGameState.Of(num));

    public static bool CanAliceWinByClosedForm(SumGameState state)
    {
        if (state.TotalBlanks % TurnParityDivisor != 0)
        {
            return true;
        }

        // An even total makes RightBlanks - LeftBlanks even too, so this division
        // is exact in both directions.
        var levellingMargin = MaxDigit * (state.RightBlanks - state.LeftBlanks) / OptimalMarginDivisor;

        return state.SumDifference != levellingMargin;
    }
}
