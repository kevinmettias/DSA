using System.Numerics;

namespace DSAExperimentation.Algorithms.GameSearch;

// A transposition table over a dictionary keyed by position: one entry a position, the latest
// search's. A struct around the dictionary, so a search's copy of it shares the one table.
internal readonly struct TranspositionTable<TState, TScore>() : ITranspositionTable<TState, TScore>
    where TState : notnull
    where TScore : INumber<TScore>, IMinMaxValue<TScore>
{
    private readonly Dictionary<TState, TableEntry<TScore>> _entries = [];

    public bool TryRecall(TState state, out TableEntry<TScore> entry) =>
        _entries.TryGetValue(state, out entry);

    public void Remember(TState state, TableEntry<TScore> entry) => _entries[state] = entry;
}
