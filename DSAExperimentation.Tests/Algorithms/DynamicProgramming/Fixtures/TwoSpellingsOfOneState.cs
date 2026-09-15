using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

// Three state names, so that two of them can be spellings the comparer under test
// answers for alike: the start state branches into both spellings, and the second one
// is the cache hit the comparer is there to produce. They arrive as constructor
// parameters rather than as literals here, so the fixture states the shape of the
// subproblem and the test names the keys.
internal sealed class TwoSpellingsOfOneState(string start, string firstSpelling, string secondSpelling)
    : IRecurrence<string, int>
{
    public int Replay(string state, IRecurrence<string, int> rest)
    {
        if (state != start)
        {
            return 1;
        }

        return rest.Replay(firstSpelling, rest) + rest.Replay(secondSpelling, rest);
    }
}
