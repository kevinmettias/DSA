using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

// A state that asks twice for state 0, whose result is 0 - the default of its result type. A
// cache that read "holds the default" as "not computed yet" would ask for state 0 twice.
internal sealed class ZeroReachedTwice : IRecurrence<int, long>
{
    private const int ZeroState = 0;

    public long Replay(int state, IRecurrence<int, long> rest)
    {
        if (state == ZeroState)
        {
            return 0;
        }

        return rest.Replay(ZeroState, rest) + rest.Replay(ZeroState, rest) + 1;
    }
}
