using DSAExperimentation.LeetCode.Solutions.FlippingAnImage;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FlippingAnImage;

public sealed partial class FlippingAnImagePromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 1, 0], [1, 0, 1], [0, 0, 0]], true },
            { [[1, 1, 1], [0, 0, 0]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASquareImage(int[][] image, bool expected)
        => Assert.Equal(expected, FlippingAnImagePromise.IsKeptBy(image));
}
