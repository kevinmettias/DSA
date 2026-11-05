using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch;

// Finds the Grundy number of a position of an impartial game: the minimum excludant of the numbers of
// the positions its moves reach (Domain/Mathematics/GameTheory). Under normal play the player to move
// wins exactly when the number is not zero, and a sum of independent games is won by whoever can make
// the exclusive or of its parts' numbers zero. As in Negamax, positions recur, so the recursion runs
// through Memoizer and each position's number is found once; a position's options are gathered on the
// stack when there are few of them.
/// <summary>Finds Grundy numbers of an impartial game's positions over memoized positions.</summary>
public static class SpragueGrundy
{
    /// <summary>Finds the Grundy number of a position.</summary>
    /// <param name="game">The game's rules, carrying the problem's input.</param>
    /// <param name="start">The position.</param>
    /// <returns>The position's Grundy number; the player to move wins under normal play exactly when it is not zero.</returns>
    public static int Number<TGame, TState>(TGame game, TState start)
        where TGame : struct, IImpartialGame<TState>
        where TState : notnull
        => Memoizer.Memoize(start, new ExcludedOptions<TGame, TState>(game));

    // One position's number: the minimum excludant of its options' numbers.
    private sealed class ExcludedOptions<TGame, TState>(TGame game) : IRecurrence<TState, int>
        where TGame : struct, IImpartialGame<TState>
    {
        // Positions with this many moves or fewer gather their options' numbers on the stack.
        private const int StackOptionLimit = 64;

        public int Replay(TState state, IRecurrence<TState, int> rest)
        {
            var moveCount = game.MoveCount(state);
            var options = moveCount <= StackOptionLimit ? stackalloc int[moveCount] : new int[moveCount];

            for (var move = 0; move < moveCount; move++)
            {
                var option = game.Next(state, move);
                options[move] = rest.Replay(option, rest);
            }

            return MinimumExcludant.Of(options);
        }
    }
}
