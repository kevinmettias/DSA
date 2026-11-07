using System.Numerics;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames.Fixtures;

// Connect Four's columns from the edges in, CentreFirstColumns' order reversed: the worst guess at
// which move is good, to measure what the centre-first order saves.
internal readonly struct EdgesFirstColumns(BitboardGame<Gravity<ConnectFourBoard>> game)
    : IMoveOrdering<BitboardPosition>
{
    private static readonly int[] EdgesIn = [6, 0, 5, 1, 4, 2, 3];

    public void Order(BitboardPosition state, Span<int> moves)
    {
        var open = game.ChoicesOf(state);
        var next = 0;

        foreach (var column in EdgesIn)
        {
            if (((open >> column) & 1) != 0)
            {
                moves[next++] = BitOperations.PopCount(open & ((1UL << column) - 1));
            }
        }
    }
}
