using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

// Takes every position at the depth limit to be even, so a depth-limited value is what the moves
// within the depth gain and force, and nothing else.
internal readonly struct ZeroEvaluation<TState> : IStaticEvaluation<TState, int>
{
    public int Evaluate(TState state) => 0;
}
