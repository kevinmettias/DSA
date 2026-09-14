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
// The two recursive arms share one Resolve body: they differ only in what they
// hand it as the recursive call-back, which is the whole difference the benchmark
// measures.
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
    // k different fill-in orders is resolved k times. Deliberately written without
    // this repo's primitives - only the state it is handed is a repo type
    // (ARCHITECTURE.md section 17.5) - because it is the arm the memoized and
    // closed-form arms below have to justify themselves against.
    public static bool AliceWinsByBruteForceRecursion(string num) =>
        AliceWinsByBruteForceRecursion(SumGameState.Of(num));

    public static bool AliceWinsByBruteForceRecursion(SumGameState start)
    {
        var totalBlanks = start.TotalBlanks;

        bool AliceWinsFrom(SumGameState state) => Resolve(state, totalBlanks, AliceWinsFrom);

        return AliceWinsFrom(start);
    }

    // The same recursion, with Memoizer supplying the recursive call-back: the
    // recurrence reads like ordinary recursion and the cache is the library's
    // problem, exactly the shape NimGame/DivisorGame/ChalkboardXorGame use for
    // their own game-theory recurrences.
    public static bool AliceWinsByMemoizedRecursion(string num) =>
        AliceWinsByMemoizedRecursion(SumGameState.Of(num));

    public static bool AliceWinsByMemoizedRecursion(SumGameState start)
    {
        var totalBlanks = start.TotalBlanks;

        return Memoizer.Memoize<SumGameState, bool>(
            start,
            (state, aliceWins) => Resolve(state, totalBlanks, aliceWins));
    }

    // One loop shape covers both quantifiers: on Alice's turn she wants SOME move
    // whose outcome is her own win, on Bob's turn he wants SOME move whose outcome
    // is his - so "return isAliceTurn as soon as a branch's outcome equals
    // isAliceTurn, else !isAliceTurn" is OR-for-Alice / AND-for-Bob without two
    // separate loops.
    private static bool Resolve(SumGameState state, int totalBlanks, Func<SumGameState, bool> aliceWins)
    {
        if (state.TotalBlanks == 0)
        {
            return state.SumDifference != 0;
        }

        var movesMade = totalBlanks - state.TotalBlanks;
        var turn = new MoverTurn(aliceWins, IsAliceTurn: movesMade % TurnParityDivisor == 0);

        return TryFindWinningDigit(state.LeftBlanks, state.FillLeft, turn)
            ?? TryFindWinningDigit(state.RightBlanks, state.FillRight, turn)
            ?? !turn.IsAliceTurn;
    }

    // Whose move it is, together with the recursive call-back that resolves the
    // positions it leads to - carried as one value the way OpenTheLockSolution's own
    // TurnWalk is, so a digit search names one turn instead of a loose flag.
    private readonly record struct MoverTurn(Func<SumGameState, bool> AliceWins, bool IsAliceTurn);

    // Tries every digit that could fill one of the remaining blanks on a given
    // side, returning isAliceTurn as soon as some move settles the game in the
    // current mover's favor - null if no digit on this side achieves that.
    private static bool? TryFindWinningDigit(int blanksOnSide, Func<int, SumGameState> fill, MoverTurn turn)
    {
        if (blanksOnSide == 0)
        {
            return null;
        }

        for (var digit = MinDigit; digit <= MaxDigit; digit++)
        {
            if (turn.AliceWins(fill(digit)) == turn.IsAliceTurn)
            {
                return turn.IsAliceTurn;
            }
        }

        return null;
    }

    // The rule the recursion above reduces to. An odd blank count hands Alice the
    // last move, and one free digit is always enough to break a tie. With an even
    // count the players trade replies: whoever is behind answers a 9 with a 9, so
    // the side holding more blanks can shift the difference by exactly 9 per pair
    // of blanks it is owed, and Bob survives precisely when the known digits
    // already sit at that difference.
    public static bool AliceWinsByClosedForm(string num) => AliceWinsByClosedForm(SumGameState.Of(num));

    public static bool AliceWinsByClosedForm(SumGameState state)
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
