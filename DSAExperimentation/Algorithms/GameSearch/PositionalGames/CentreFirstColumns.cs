using System.Numerics;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch.PositionalGames;

// Tries the columns of a game under gravity from the centre out. A central cell lies on more lines
// than an edge cell (Connect Four's centre column on up to seven, its edge columns on as few as
// three), so a central move is the likelier good one, and alpha-beta that tries it first cuts the
// rest sooner. Columns equally far from the centre go left first, so 7 columns run 3, 2, 4, 1, 5,
// 0, 6 and 4 columns run 1, 2, 0, 3. A move is the game's number for its column: the count of open
// columns left of it.
/// <summary>Orders a gravity game's moves by their columns, from the centre out.</summary>
/// <typeparam name="TShape">The board's columns and rows.</typeparam>
/// <param name="game">The game whose moves to order, which says which columns are open.</param>
public readonly struct CentreFirstColumns<TShape>(BitboardGame<Gravity<TShape>> game)
    : IMoveOrdering<BitboardPosition>
    where TShape : struct, IMnkShape
{
    private static readonly int[] CentreOut =
        [.. Enumerable.Range(0, TShape.Columns).OrderBy(DistanceFromCentre)];

    /// <inheritdoc/>
    public void Order(BitboardPosition state, Span<int> moves)
    {
        var open = game.ChoicesOf(state);
        var next = 0;

        foreach (var column in CentreOut)
        {
            if (((open >> column) & 1) != 0)
            {
                moves[next++] = BitOperations.PopCount(open & ((1UL << column) - 1));
            }
        }
    }

    // Twice a column's distance from the board's centre, which is a whole number on any width.
    private static int DistanceFromCentre(int column) =>
        Math.Abs(column + column - (TShape.Columns - 1));
}
