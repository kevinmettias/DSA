using DSAExperimentation.LeetCode.MinimumCostOfAPathWithSpecialRoads;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostOfAPathWithSpecialRoads;

// Harness only. Both strategies are MinimumCostOfAPathWithSpecialRoadsSolution's -
// this file just pins them to LeetCode's published examples plus the shape cases the
// pre-migration test already carried. The array-scan Dijkstra baseline was previously
// scaffolding inlined in the benchmark and asserted by nothing; it is held to the same
// examples as the composed Dijkstra here for the first time.
public sealed class MinimumCostOfAPathWithSpecialRoadsTests
{
    public static TheoryData<int[], int[], int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: walk to (1,2), take the road to (3,3), walk to (3,4),
            // take the road to (4,5) - 1 + 2 + 1 + 1 = 5.
            { [1, 1], [4, 5], [[1, 2, 3, 3, 2], [3, 4, 4, 5, 1]], 5 },

            // LeetCode example 2: every special road is a detour, so the answer is the
            // plain Manhattan walk from (3,2) to (5,7).
            { [3, 2], [5, 7], [[3, 2, 3, 4, 4], [3, 3, 5, 5, 5], [3, 4, 5, 6, 6]], 7 },

            // A single overpriced road that must be ignored in favour of walking.
            { [0, 0], [3, 4], [[0, 0, 3, 4, 100]], 7 },

            // A single road straight to the target, cheaper than any walk.
            { [0, 0], [10, 10], [[0, 0, 10, 10, 1]], 1 },

            // Roads are one-way: the only road runs target -> start, so it is useless.
            { [0, 0], [5, 5], [[5, 5, 0, 0, 1]], 10 },

            // No special roads at all: the answer is the bare Manhattan distance.
            { [2, 3], [8, 1], [], 8 },

            // Start already at the target costs nothing, even with a road on offer.
            { [4, 4], [4, 4], [[4, 4, 9, 9, 1]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByArrayScanDijkstra_LeetCodeExamples_ReturnsCheapestRouteCost(
        int[] start, int[] target, int[][] specialRoads, int expected)
    {
        var actual =
            MinimumCostOfAPathWithSpecialRoadsSolution.MinimumCostByArrayScanDijkstra(start, target, specialRoads);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByHeapDijkstra_LeetCodeExamples_ReturnsCheapestRouteCost(
        int[] start, int[] target, int[][] specialRoads, int expected)
    {
        var actual = MinimumCostOfAPathWithSpecialRoadsSolution.MinimumCostByHeapDijkstra(start, target, specialRoads);

        Assert.Equal(expected, actual);
    }
}
