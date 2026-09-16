using DSAExperimentation.LeetCode.CountWaysToBuildRoomsInAnAntColony;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountWaysToBuildRoomsInAnAntColony;

// Harness only. Pinning both algebras to the same examples is what proves the
// precomputed-factorial table agrees with the straightforward per-node one - a
// check the benchmark alone could never make.
public sealed partial class CountWaysToBuildRoomsInAnAntColonyTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [-1, 0, 1], 1 },
            { [-1, 0, 0, 1, 2], 6 },
            { [-1, 0, 0, 0], 6 },
            { [-1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaysToBuildOrderByPerNodeFactorial_LeetCodeExamples_ReturnsDistinctBuildOrderCount(
        int[] prevRoom, int expected) =>
        Assert.Equal(
            expected,
            CountWaysToBuildRoomsInAnAntColonySolution.WaysToBuildOrderByPerNodeFactorial(prevRoom));

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaysToBuildOrderByPrecomputedFactorials_LeetCodeExamples_ReturnsDistinctBuildOrderCount(
        int[] prevRoom, int expected) =>
        Assert.Equal(
            expected,
            CountWaysToBuildRoomsInAnAntColonySolution.WaysToBuildOrderByPrecomputedFactorials(prevRoom));
}
