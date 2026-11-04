using DSAExperimentation.LeetCode.Solutions.MaximumAreaRectangleWithPointConstraintsII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumAreaRectangleWithPointConstraintsII;

public sealed partial class MaximumAreaRectangleWithPointConstraintsIIPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [1, 1, 3, 3], [1, 3, 1, 3], true },
            { [1, 1, 1, 1], [1, 1, 1, 1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenThePointsAreUnique(int[] xCoord, int[] yCoord, bool expected)
        => Assert.Equal(expected, MaximumAreaRectangleWithPointConstraintsIIPromise.IsKeptBy(xCoord, yCoord));
}
