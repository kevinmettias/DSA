namespace DSAExperimentation.Domain.Mathematics.GameTheory;

// The order in which to try a position's moves. It changes no value: a search that tries every
// move finds the same best whatever the order. It changes how much a search with cutoffs must
// look at, since a move that refutes the opponent's found first makes every later move's search
// end early, so the best order tries the likeliest good move first. Moves are IAlternatingGame's
// numbers, 0 to MoveCount - 1, and an ordering is a permutation of them written into a buffer the
// search provides, so no list is allocated.
/// <summary>The order in which a search tries a position's moves.</summary>
/// <typeparam name="TState">A position, with the player to move implied.</typeparam>
public interface IMoveOrdering<TState>
{
    /// <summary>Writes every move of a position, each once, in the order to try them.</summary>
    /// <param name="state">The position.</param>
    /// <param name="moves">
    /// As long as the position's move count; it receives the move numbers 0 to its length - 1.
    /// </param>
    void Order(TState state, Span<int> moves);
}
