using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

// The climbing-stairs rule, named: the ways to reach a step are the ways to reach the
// two steps below it, and either of the first two steps is its own answer.
internal sealed class WaysFromPreviousTwoSteps : IRecurrence<int, long>
{
    private const int RecurrenceOrder = 2;

    public long Replay(int state, IRecurrence<int, long> rest)
    {
        if (state <= 1)
        {
            return state;
        }

        return rest.Replay(state - 1, rest) + rest.Replay(state - RecurrenceOrder, rest);
    }
}
