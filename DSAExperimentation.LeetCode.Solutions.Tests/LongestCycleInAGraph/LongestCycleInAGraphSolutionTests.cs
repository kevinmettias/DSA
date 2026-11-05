using DSAExperimentation.LeetCode.Solutions.LongestCycleInAGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestCycleInAGraph;

// Harness only. Both strategies live in LongestCycleInAGraphSolution - including
// the per-start forward walk, which used to exist as an unasserted benchmark
// baseline - and this file pins them to the same cases so a failure names the
// strategy that broke.
public sealed partial class LongestCycleInAGraphSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            // Every node isolated - no edges anywhere.
            { [-1, -1], -1 },

            // The shortest possible cycle: a mutual pair.
            { [1, 0], 2 },

            // A cycle plus a dead-end tail hanging off it, so the walk from node 5
            // has to fall into the cycle rather than report its own path length.
            { [1, 2, 0, 4, 5, -1, 3], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestCycleByPerStartWalk_Cases_ReturnsLongestCycleLength(int[] edges, int expected) =>
        Assert.Equal(expected, LongestCycleInAGraphSolution.LongestCycleByPerStartWalk(edges));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestCycleByTarjanComponents_Cases_ReturnsLongestCycleLength(int[] edges, int expected) =>
        Assert.Equal(expected, LongestCycleInAGraphSolution.LongestCycleByTarjanComponents(edges));
}
