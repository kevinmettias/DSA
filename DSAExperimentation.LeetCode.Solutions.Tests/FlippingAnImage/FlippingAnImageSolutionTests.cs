using DSAExperimentation.LeetCode.Solutions.FlippingAnImage;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FlippingAnImage;

// Harness only. Both strategies are FlippingAnImageSolution's - this file just
// pins them to a single-column and a single-row image. Each row is cloned before
// flipping so the two theory methods never share a rewritten image. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class FlippingAnImageSolutionTests
{
    public static TheoryData<int[][], int[][]> Cases =>
        new()
        {
            {
                [[1], [0], [1]],
                [[0], [1], [0]]
            },
            {
                [[0, 1]],
                [[0, 1]]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FlipAndInvertImageByTwoPointerReverse_Cases_FlipsThenInvertsEachRow(
        int[][] image, int[][] expected) =>
        Assert.Equal(expected, FlippingAnImageSolution.FlipAndInvertImageByTwoPointerReverse(Clone(image)));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FlipAndInvertImageByStackReverse_Cases_FlipsThenInvertsEachRow(
        int[][] image, int[][] expected) =>
        Assert.Equal(expected, FlippingAnImageSolution.FlipAndInvertImageByStackReverse(Clone(image)));

    private static int[][] Clone(int[][] image)
    {
        var copy = new int[image.Length][];

        for (var r = 0; r < image.Length; r++)
        {
            copy[r] = (int[])image[r].Clone();
        }

        return copy;
    }
}
