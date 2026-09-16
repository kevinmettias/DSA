using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming;

// Companion of MemoRun, the recursion Memoizer hands a recurrence: its Replay is the
// whole of the cache's read-and-write, and the claim pinned here is that a second
// request for a state the run has already computed is answered with the very result
// it stored rather than by asking the recurrence again. The run is private to
// Memoizer, so it is driven through the only surface that reaches it - one Memoize
// call whose recurrence branches twice onto the same sub-state.
public sealed partial class MemoRunTests
{
    [Fact]
    public void Replay_AnswersASecondRequestForTheSameState_WithTheResultItAlreadyStored()
    {
        var recurrence = new TwoBranchesOntoOneState();

        var result = Memoizer.Memoize<int, int[]>(1, recurrence);

        Assert.Same(recurrence.FirstObservation, recurrence.SecondObservation);
        Assert.Same(recurrence.FirstObservation, result);
        Assert.Equal(1, recurrence.Computations);
    }
}
