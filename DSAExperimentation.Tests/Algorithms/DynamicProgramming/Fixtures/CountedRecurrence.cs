using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

// The recurrence a counting test passes, plus a tally of how many times the memo asked
// it to compute a state - which is the evidence that the cache, and not the recurrence,
// absorbed the repeats. Counting out here rather than inside each decision below keeps
// the body under test identical to the one the matching value test runs, so the tally
// and the expected value are always about the same body.
internal sealed class CountedRecurrence<TState, TResult>(IRecurrence<TState, TResult> inner)
    : IRecurrence<TState, TResult>
    where TState : notnull
{
    public int Calls { get; private set; }

    public TResult Replay(TState state, IRecurrence<TState, TResult> rest)
    {
        Calls++;

        return inner.Replay(state, rest);
    }
}
