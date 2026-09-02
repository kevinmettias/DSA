using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumGame;

// LeetCode 1927. Sum Game: full minimax recursion over a reduced game state
// (remaining '?' count on each half, running sum difference) via this repo's own
// Memoizer - the same "natural-looking recursion, no hand-rolled cache" shape
// NimGameTests/DivisorGameTests/ChalkboardXorGameTests already use for their own
// game-theory recurrences. Which specific '?' position gets filled never affects the
// outcome, only which half it's on and which digit gets written there, so the state
// collapses to (leftBlanks, rightBlanks, sumDiff) - small enough for Memoizer's
// plain Dictionary cache even though the raw fill-in order space is exponential.
// Resolve's single loop shape handles both quantifiers at once: on Alice's turn she
// wants SOME move with outcome == true (her own win), on Bob's turn he wants SOME
// move with outcome == false (his own win) - "return isAliceTurn as soon as any
// branch's outcome equals isAliceTurn, else return !isAliceTurn" is exactly OR-for-
// Alice / AND-for-Bob without two separate loops. The recursion reduces to the
// well-known "odd blank count always wins Alice; otherwise compare sumDiff to
// 9 * (rightBlanks - leftBlanks) / 2" closed form, which the benchmark compares
// against.
public sealed partial class SumGameTests
{
    [Theory]
    [InlineData("5023", false)]
    [InlineData("25??", true)]
    [InlineData("?3295???", false)]
    public void AliceWins_LeetCodeExamples_MatchesExpectedOutcome(string num, bool expected)
        => Assert.Equal(expected, AliceWins(num));

    private static bool AliceWins(string num)
    {
        var half = num.Length / 2;
        var (leftSum, leftBlanks) = SumKnownDigits(num, 0, half);
        var (rightSum, rightBlanks) = SumKnownDigits(num, half, num.Length);
        var totalBlanks = leftBlanks + rightBlanks;

        return Memoizer.Memoize<(int LeftBlanks, int RightBlanks, int Diff), bool>(
            (leftBlanks, rightBlanks, leftSum - rightSum),
            (state, aliceWins) => Resolve(state, totalBlanks, aliceWins));
    }

    private static bool Resolve(
        (int LeftBlanks, int RightBlanks, int Diff) state,
        int totalBlanks,
        Func<(int LeftBlanks, int RightBlanks, int Diff), bool> aliceWins)
    {
        if (state.LeftBlanks == 0 && state.RightBlanks == 0)
        {
            return state.Diff != 0;
        }

        var movesMade = totalBlanks - (state.LeftBlanks + state.RightBlanks);
        var isAliceTurn = movesMade % 2 == 0;

        return TryFindWinningDigit(
                state.LeftBlanks,
                digit => (state.LeftBlanks - 1, state.RightBlanks, state.Diff + digit),
                aliceWins,
                isAliceTurn)
            ?? TryFindWinningDigit(
                state.RightBlanks,
                digit => (state.LeftBlanks, state.RightBlanks - 1, state.Diff - digit),
                aliceWins,
                isAliceTurn)
            ?? !isAliceTurn;
    }

    // Tries every digit 0-9 that could fill one of the remaining blanks on a given
    // side, returning isAliceTurn as soon as some move flips the outcome to the
    // current mover's favor - null if no digit on this side achieves that.
    private static bool? TryFindWinningDigit(
        int blanksOnSide,
        Func<int, (int LeftBlanks, int RightBlanks, int Diff)> buildNextState,
        Func<(int LeftBlanks, int RightBlanks, int Diff), bool> aliceWins,
        bool isAliceTurn)
    {
        if (blanksOnSide == 0)
        {
            return null;
        }

        for (var digit = 0; digit <= 9; digit++)
        {
            var outcome = aliceWins(buildNextState(digit));

            if (outcome == isAliceTurn)
            {
                return isAliceTurn;
            }
        }

        return null;
    }

    private static (int Sum, int Blanks) SumKnownDigits(string num, int start, int end)
    {
        var sum = 0;
        var blanks = 0;

        for (var i = start; i < end; i++)
        {
            if (num[i] == '?')
            {
                blanks++;
            }
            else
            {
                sum += num[i] - '0';
            }
        }

        return (sum, blanks);
    }
}
