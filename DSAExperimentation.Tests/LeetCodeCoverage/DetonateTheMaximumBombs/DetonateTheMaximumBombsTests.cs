using DSAExperimentation.LeetCode.DetonateTheMaximumBombs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DetonateTheMaximumBombs;

// Harness only. Both per-candidate reachability strategies are
// DetonateTheMaximumBombsSolution's - the hand-rolled bool[] recursion and the same
// successor closure run through this repo's DepthFirstSearch - and this file pins
// them to LeetCode's published examples plus the one-bomb and two-cluster cases the
// originals were missing.
public sealed partial class DetonateTheMaximumBombsTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LC example 1: bomb 1's blast reaches bomb 0's center but not the reverse.
            { [[2, 1, 3], [6, 1, 4]], 2 },
            // LC example 2: neither bomb reaches the other.
            { [[1, 1, 5], [10, 10, 5]], 1 },
            // LC example 3: a chain detonated in full from its first bomb.
            { [[1, 2, 3], [2, 3, 1], [3, 4, 2], [4, 5, 3], [5, 6, 4]], 5 },
            // A lone bomb detonates only itself.
            { [[1, 1, 1]], 1 },
            // Two disjoint clusters - the answer comes from the larger one.
            { [[0, 0, 1], [1, 0, 1], [10, 10, 5], [12, 10, 5], [14, 10, 5]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDetonationsByManualRecursion_LeetCodeExamples_ReturnsLargestChainOverEveryTrigger(
        int[][] bombs, int expected) =>
        Assert.Equal(expected, DetonateTheMaximumBombsSolution.MaxDetonationsByManualRecursion(bombs));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDetonationsByRepoDepthFirstSearch_LeetCodeExamples_ReturnsLargestChainOverEveryTrigger(
        int[][] bombs, int expected) =>
        Assert.Equal(expected, DetonateTheMaximumBombsSolution.MaxDetonationsByRepoDepthFirstSearch(bombs));
}
