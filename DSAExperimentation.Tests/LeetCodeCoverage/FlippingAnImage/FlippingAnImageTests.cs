using DSAExperimentation.LeetCode.FlippingAnImage;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlippingAnImage;

// Harness only. Both strategies are FlippingAnImageSolution's - this file just
// pins them to LeetCode's published examples. Each row is cloned before flipping
// so the two theory methods never share a rewritten image.
public sealed class FlippingAnImageTests
{
    public static TheoryData<int[][], int[][]> Examples =>
        new()
        {
            {
                [[1, 1, 0], [1, 0, 1], [0, 0, 0]],
                [[1, 0, 0], [0, 1, 0], [1, 1, 1]]
            },
            {
                [[1, 1, 0, 0], [1, 0, 0, 1], [0, 1, 1, 1], [1, 0, 1, 0]],
                [[1, 1, 0, 0], [0, 1, 1, 0], [0, 0, 0, 1], [1, 0, 1, 0]]
            },
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
    [MemberData(nameof(Examples))]
    public void FlipAndInvertImageByTwoPointerReverse_LeetCodeExamples_FlipsThenInvertsEachRow(
        int[][] image, int[][] expected) =>
        Assert.Equal(expected, FlippingAnImageSolution.FlipAndInvertImageByTwoPointerReverse(Clone(image)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FlipAndInvertImageByStackReverse_LeetCodeExamples_FlipsThenInvertsEachRow(
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
