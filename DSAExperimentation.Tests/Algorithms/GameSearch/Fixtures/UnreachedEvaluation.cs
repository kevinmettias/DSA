using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

// An evaluation a search to the end of the game must never ask for: it throws, so a test that
// searches deep enough to finish every line of play also proves no estimate entered its value.
internal readonly struct UnreachedEvaluation<TState> : IStaticEvaluation<TState, int>
{
    private const string Asked = "A search to the end of the game asked for an estimate.";

    public int Evaluate(TState state) => throw new InvalidOperationException(Asked);
}
