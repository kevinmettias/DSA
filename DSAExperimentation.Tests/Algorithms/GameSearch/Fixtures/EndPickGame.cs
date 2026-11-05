using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

// A row of numbers from which each turn takes the leftmost or the rightmost, scoring it: a scoring game
// valued as the difference the player to move can force. A position is the row's remaining span.
internal readonly struct EndPickGame(int[] row) : IAlternatingGame<(int Left, int Right), int>
{
    private const int TakeLeft = 0;

    // An empty row has no move, one number one, and a longer row two: its left end and its right.
    public int MoveCount((int Left, int Right) state) => Math.Clamp(state.Right - state.Left + 1, 0, 2);

    public (int Left, int Right) Next((int Left, int Right) state, int move)
    {
        if (move == TakeLeft)
        {
            return (state.Left + 1, state.Right);
        }

        return (state.Left, state.Right - 1);
    }

    public int Gain((int Left, int Right) state, int move)
    {
        var taken = move == TakeLeft ? state.Left : state.Right;

        return row[taken];
    }

    public int ValueWhenOver((int Left, int Right) state) => 0;
}
