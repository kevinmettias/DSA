using DSAExperimentation.LeetCode.Solutions.DetonateTheMaximumBombs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DetonateTheMaximumBombs;

// Harness only. Both per-candidate reachability strategies are
// DetonateTheMaximumBombsSolution's - the hand-rolled bool[] recursion and the same
// successor closure run through this repo's DepthFirstSearch - and this file pins
// them to the one-bomb and two-cluster cases the
// originals were missing.
public sealed partial class DetonateTheMaximumBombsSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // A lone bomb detonates only itself.
            { [[1, 1, 1]], 1 },
            // Two disjoint clusters - the answer comes from the larger one.
            { [[0, 0, 1], [1, 0, 1], [10, 10, 5], [12, 10, 5], [14, 10, 5]], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxDetonationsByManualRecursion_Cases_ReturnsLargestChainOverEveryTrigger(
        int[][] bombs, int expected) =>
        Assert.Equal(expected, DetonateTheMaximumBombsSolution.MaxDetonationsByManualRecursion(bombs));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxDetonationsByBlastGraphReduce_Cases_ReturnsLargestChainOverEveryTrigger(
        int[][] bombs, int expected) =>
        Assert.Equal(expected, DetonateTheMaximumBombsSolution.MaxDetonationsByBlastGraphReduce(bombs));
}
