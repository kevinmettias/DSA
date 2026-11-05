using DSAExperimentation.LeetCode.Solutions.DeliveringBoxesFromStorageToPorts;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DeliveringBoxesFromStorageToPorts;

public sealed partial class DeliveringBoxesFromStorageToPortsPromiseTests
{
    public static TheoryData<int[][], int, int, int, bool> Inputs =>
        new()
        {
            { [[1, 1], [2, 1], [1, 1]], 2, 3, 3, true },
            { [[1, 4], [1, 2], [2, 1], [2, 1], [3, 2], [3, 4]], 3, 6, 6, true },
            { [[1, 1], [5, 5]], 5, 2, 4, false },
            { [[3, 1]], 2, 1, 1, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryBoxGoesToAPortAndFitsOneTrip(int[][] boxes, int portsCount, int maxBoxes, int maxWeight, bool expected)
        => Assert.Equal(expected, DeliveringBoxesFromStorageToPortsPromise.IsKeptBy(boxes, portsCount, maxBoxes, maxWeight));
}
