using DSAExperimentation.LeetCode.Solutions.CountWaysToBuildRoomsInAnAntColony;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountWaysToBuildRoomsInAnAntColony;

// Harness only. Pinning the plain recursion and both algebras to the same cases is
// what proves the precomputed-factorial table agrees with the straightforward per-node
// one, and the fold with the recursion it stands for - a check the benchmark alone could
// never make.
public sealed partial class CountWaysToBuildRoomsInAnAntColonySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [-1], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void WaysToBuildOrderByPostOrderRecursion_Cases_ReturnsDistinctBuildOrderCount(
        int[] prevRoom, int expected) =>
        Assert.Equal(
            expected,
            CountWaysToBuildRoomsInAnAntColonySolution.WaysToBuildOrderByPostOrderRecursion(prevRoom));

    [Theory]
    [MemberData(nameof(Cases))]
    public void WaysToBuildOrderByPerNodeFactorial_Cases_ReturnsDistinctBuildOrderCount(
        int[] prevRoom, int expected) =>
        Assert.Equal(
            expected,
            CountWaysToBuildRoomsInAnAntColonySolution.WaysToBuildOrderByPerNodeFactorial(prevRoom));

    [Theory]
    [MemberData(nameof(Cases))]
    public void WaysToBuildOrderByPrecomputedFactorials_Cases_ReturnsDistinctBuildOrderCount(
        int[] prevRoom, int expected) =>
        Assert.Equal(
            expected,
            CountWaysToBuildRoomsInAnAntColonySolution.WaysToBuildOrderByPrecomputedFactorials(prevRoom));
}
