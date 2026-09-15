using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

// A recurrence that never branches: every state is its own answer, so `rest` goes
// unread and the memo has nothing to absorb. Worth keeping beside the decisions above
// because the interface hands every recurrence a recursion it is allowed to ignore.
internal sealed class TheStateIsItsOwnAnswer : IRecurrence<int, int>
{
    public int Replay(int state, IRecurrence<int, int> rest) => state;
}
