using DSAExperimentation.LeetCode.DeliveringBoxesFromStorageToPorts;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeliveringBoxesFromStorageToPorts;

// Harness only: both strategies are DeliveringBoxesFromStorageToPortsSolution's.
// This file pins them to LeetCode's published examples plus the boundaries it
// never published - one trip that carries everything, a box count of one that
// forces a trip each, and a weight limit rather than a count limit deciding where
// the window closes.
public sealed partial class DeliveringBoxesFromStorageToPortsTests
{
    // Each box is LeetCode's [port, weight]; the answer is the total number of
    // legs the ship travels.
    public static TheoryData<int[][], int, int, int> Examples =>
        new()
        {
            // LeetCode's three published examples.
            { [[1, 1], [2, 1], [1, 1]], 3, 3, 4 },
            { [[1, 2], [3, 3], [3, 1], [3, 1], [2, 4]], 3, 6, 6 },
            { [[1, 4], [1, 2], [2, 1], [2, 1], [3, 2], [3, 4]], 6, 7, 6 },

            // One port throughout and room for every box: a single trip, out and
            // back, with no switches to pay for.
            { [[1, 1], [1, 1], [1, 1]], 3, 3, 2 },

            // A box count of one forces a trip per box however light they are.
            { [[1, 1], [1, 1]], 1, 10, 4 },

            // Here the weight limit, not the box count, closes the window: two
            // boxes per trip at most, so two trips to the same port.
            { [[1, 3], [1, 3], [1, 3], [1, 3]], 10, 6, 4 },

            // The shortest input the constraints allow.
            { [[1, 1]], 1, 1, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTripsByWindowRescan_LeetCodeExamples_ReturnsFewestLegs(
        int[][] boxes, int maxBoxes, int maxWeight, int expected)
    {
        var actual = DeliveringBoxesFromStorageToPortsSolution.MinTripsByWindowRescan(boxes, maxBoxes, maxWeight);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTripsByMonotonicDeque_LeetCodeExamples_ReturnsFewestLegs(
        int[][] boxes, int maxBoxes, int maxWeight, int expected)
    {
        var actual = DeliveringBoxesFromStorageToPortsSolution.MinTripsByMonotonicDeque(boxes, maxBoxes, maxWeight);
        Assert.Equal(expected, actual);
    }
}
