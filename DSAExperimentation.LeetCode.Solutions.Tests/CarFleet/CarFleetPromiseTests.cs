using DSAExperimentation.LeetCode.Solutions.CarFleet;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CarFleet;

public sealed partial class CarFleetPromiseTests
{
    public static TheoryData<int, int[], int[], bool> Inputs =>
        new()
        {
            { 12, [10, 8, 0, 5, 3], [2, 4, 1, 1, 3], true },
            { 10, [3], [3], true },
            { 10, [3, 10], [1, 1], false },
            { 10, [3, 3], [1, 2], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryCarStartsShortOfTheTargetAtItsOwnPlace(int target, int[] position, int[] speed, bool expected)
        => Assert.Equal(expected, CarFleetPromise.IsKeptBy(target, position, speed));
}
