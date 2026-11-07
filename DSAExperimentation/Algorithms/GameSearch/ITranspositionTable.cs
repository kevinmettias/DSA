using System.Numerics;

namespace DSAExperimentation.Algorithms.GameSearch;

// Where a depth-limited search remembers the positions it has searched. Positions recur along
// different move orders (in Connect Four, any two moves in different columns can be made in either
// order), so a search that remembers a position's result answers its next visit at the same depth
// without searching it again, and tries the best move it found first at another depth. A struct
// witness, so a search with no table compiles every lookup away.
internal interface ITranspositionTable<TState, TScore>
    where TScore : INumber<TScore>, IMinMaxValue<TScore>
{
    // Finds what an earlier search of the position left, when one did.
    bool TryRecall(TState state, out TableEntry<TScore> entry);

    // Keeps what a search of the position found, in place of anything older.
    void Remember(TState state, TableEntry<TScore> entry);
}
