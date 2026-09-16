using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

// A state whose two branches ask the recursion for the SAME sub-state, and keep both
// answers: the run answers the second request out of the entry the first one stored,
// so the two observations are one object and the recurrence was asked once. A run
// that recomputed instead of reading its cache would hand back two distinct results
// here - which is the difference under test, and why the sub-state's own result is a
// reference type rather than a call count.
internal sealed class TwoBranchesOntoOneState : IRecurrence<int, int[]>
{
    private const int SubState = 0;

    public int[]? FirstObservation { get; private set; }
    public int[]? SecondObservation { get; private set; }
    public int Computations { get; private set; }

    public int[] Replay(int state, IRecurrence<int, int[]> rest)
    {
        if (state == SubState)
        {
            Computations++;

            return [state];
        }

        FirstObservation = rest.Replay(SubState, rest);
        SecondObservation = rest.Replay(SubState, rest);

        return FirstObservation;
    }
}
