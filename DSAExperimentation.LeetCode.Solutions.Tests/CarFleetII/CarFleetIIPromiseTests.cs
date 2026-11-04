using DSAExperimentation.LeetCode.Solutions.CarFleetII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CarFleetII;

public sealed partial class CarFleetIIPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 2], [2, 1], [4, 3], [7, 2]], true },
            { [[3, 4], [5, 4], [6, 3], [9, 1]], true },
            { [[10, 10], [8, 5], [6, 3]], false },
            { [[1, 1], [1, 2]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEachCarStartsAheadOfTheLast(int[][] cars, bool expected)
        => Assert.Equal(expected, CarFleetIIPromise.IsKeptBy(cars));
}
