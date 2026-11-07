using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch;

// The order the game numbers its moves in, 0 first: no ordering at all, for a search whose game
// has no better guess, and for plain negamax, which tries every move whatever the order.
/// <summary>Tries a position's moves in the order the game numbers them.</summary>
/// <typeparam name="TState">A position.</typeparam>
public readonly struct MoveNumberOrder<TState> : IMoveOrdering<TState>
{
    /// <inheritdoc/>
    public void Order(TState state, Span<int> moves)
    {
        for (var move = 0; move < moves.Length; move++)
        {
            moves[move] = move;
        }
    }
}
