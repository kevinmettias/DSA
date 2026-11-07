using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

// The game's moves last first: an ordering as unlike the game's own as any, to show that the order
// changes how much is searched and never the value.
internal readonly struct ReversedMoveOrder<TState> : IMoveOrdering<TState>
{
    public void Order(TState state, Span<int> moves)
    {
        for (var rank = 0; rank < moves.Length; rank++)
        {
            moves[rank] = moves.Length - 1 - rank;
        }
    }
}
