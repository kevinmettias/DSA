using System.Collections.Immutable;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.LeetCode.Solutions.DivisorGame;

// LeetCode 1025. Divisor Game: Alice moves first and must replace the position with
// position - x for some divisor x of it with 0 < x < position; a player who cannot
// move loses. Alice wins iff some legal move hands Bob a losing position.
//
// CanAliceWinByMemoizedRecursion states that definition literally - the
// natural-looking game-theory recursion over this repo's own Memoizer, the same
// shape NimGame and StoneGame use for their recurrences. CanAliceWinByParityFormula
// is the O(1) closed form the recursion provably reduces to ("an even position
// wins"), which is the arm the recursion is measured against; keeping both here
// is what finally puts them under the same assertions instead of one living in a
// test and one in a benchmark.
[LeetCodeQuestion("divisor-game", LeetCodeAnswerOrder.AsWritten)]
internal static class DivisorGameSolution
{
    private const int ParityDivisor = 2;

    // O(n^2) memoized search: for each state, scan every candidate divisor x and win
    // as soon as one leaves the opponent in a losing state.
    public static bool CanAliceWinByMemoizedRecursion(int position) =>
        Memoizer.Memoize<int, bool>(position, new WinFromPosition());

    // The closed form: subtracting 1 always flips parity, and from an odd position
    // every divisor is odd, so odd positions can only hand back even ones. Alice wins
    // exactly when position is even.
    public static bool CanAliceWinByParityFormula(int position) =>
        position % ParityDivisor == 0;

    // The rule named as Domain's impartial game: Alice, moving first, wins when the start's
    // Grundy number, from Algorithms/GameSearch, is not zero.
    public static bool CanAliceWinBySpragueGrundy(int position) =>
        SpragueGrundy.Number(new ProperDivisorSubtraction(position), position) != 0;

    // The recurrence, as a named type: the position is winning as soon as some proper
    // divisor of it hands the opponent a losing position - the rule as stated.
    private sealed class WinFromPosition : IRecurrence<int, bool>
    {
        public bool Replay(int current, IRecurrence<int, bool> rest)
        {
            for (var x = 1; x < current; x++)
            {
                if (current % x == 0 && !rest.Replay(current - x, rest))
                {
                    return true;
                }
            }

            return false;
        }
    }

    // The rule as an impartial game: a turn subtracts a proper divisor of the position. Each
    // position's proper divisors are sieved once, up to the start, so move i subtracts the
    // i-th of them without searching.
    private readonly struct ProperDivisorSubtraction : IImpartialGame<int>
    {
        private readonly ImmutableArray<ImmutableArray<int>> _properDivisors;

        public ProperDivisorSubtraction(int largest)
        {
            var divisors = new List<int>[largest + 1];

            for (var position = 0; position <= largest; position++)
            {
                divisors[position] = [];
            }

            for (var divisor = 1; divisor <= largest; divisor++)
            {
                for (var multiple = divisor * ParityDivisor; multiple <= largest; multiple += divisor)
                {
                    divisors[multiple].Add(divisor);
                }
            }

            _properDivisors = [.. divisors.Select(list => list.ToImmutableArray())];
        }

        public int MoveCount(int position) => _properDivisors[position].Length;

        public int Next(int position, int move) => position - _properDivisors[position][move];
    }
}
