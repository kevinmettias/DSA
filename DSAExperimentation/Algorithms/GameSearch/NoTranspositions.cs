using System.Numerics;

namespace DSAExperimentation.Algorithms.GameSearch;

// No table: every visit to a position searches it again, as plain negamax and textbook alpha-beta
// do. Its members are constants, so the search's lookups fold away.
internal readonly struct NoTranspositions<TState, TScore> : ITranspositionTable<TState, TScore>
    where TScore : INumber<TScore>, IMinMaxValue<TScore>
{
    public bool TryRecall(TState state, out TableEntry<TScore> entry)
    {
        entry = default;

        return false;
    }

    public void Remember(TState state, TableEntry<TScore> entry)
    {
    }
}
