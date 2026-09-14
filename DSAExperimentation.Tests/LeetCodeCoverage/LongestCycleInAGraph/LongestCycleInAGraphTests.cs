using DSAExperimentation.LeetCode.LongestCycleInAGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCycleInAGraph;

// Harness only. Both strategies live in LongestCycleInAGraphSolution - including
// the per-start forward walk, which used to exist as an unasserted benchmark
// baseline - and this file pins them to the same examples so a failure names the
// strategy that broke.
public sealed class LongestCycleInAGraphTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            // LeetCode example 1: a tail feeding into the cycle {2, 3, 4}.
            { [3, 3, 4, 2, 3], 3 },

            // LeetCode example 2: 0 -> 2 -> 3 -> 1 -> nowhere, so no cycle at all.
            { [2, -1, 3, 1], -1 },

            // Two disjoint cycles: 0 -> 1 -> 2 -> 0 and 3 -> 4 -> 3.
            { [1, 2, 0, 4, 3], 3 },

            // Every node isolated - no edges anywhere.
            { [-1, -1], -1 },

            // The shortest possible cycle: a mutual pair.
            { [1, 0], 2 },

            // One cycle spanning every node, the benchmark's own workload shape.
            { [1, 2, 3, 4, 0], 5 },

            // A cycle plus a dead-end tail hanging off it, so the walk from node 5
            // has to fall into the cycle rather than report its own path length.
            { [1, 2, 0, 4, 5, -1, 3], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestCycleByPerStartWalk_LeetCodeExamples_ReturnsLongestCycleLength(int[] edges, int expected) =>
        Assert.Equal(expected, LongestCycleInAGraphSolution.LongestCycleByPerStartWalk(edges));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestCycleByTarjanComponents_LeetCodeExamples_ReturnsLongestCycleLength(int[] edges, int expected) =>
        Assert.Equal(expected, LongestCycleInAGraphSolution.LongestCycleByTarjanComponents(edges));
}
