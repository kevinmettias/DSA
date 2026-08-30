using StackOfInt = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlippingAnImage;

// LeetCode 832. Flipping an Image: horizontally flipping a row is exactly a LIFO
// reversal - push every value in a row onto this repo's own Stack<T> (the same
// digit-reversal primitive ReverseIntegerTests/RotateImageTests already use for
// LC 7/48), then pop the values back out into the row while inverting each bit
// (1 - value) as it comes off the stack. Popping naturally reverses row order, so
// flip-then-invert happens in the single pop pass.
public sealed partial class FlippingAnImageTests
{
    [Fact]
    public void FlipAndInvertImage_ThreeByThreeExample_FlipsThenInvertsEachRow()
    {
        int[][] image = [[1, 1, 0], [1, 0, 1], [0, 0, 0]];

        var result = FlipAndInvertImage(image);

        int[][] expected = [[1, 0, 0], [0, 1, 0], [1, 1, 1]];
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FlipAndInvertImage_FourByFourExample_FlipsThenInvertsEachRow()
    {
        int[][] image = [[1, 1, 0, 0], [1, 0, 0, 1], [0, 1, 1, 1], [1, 0, 1, 0]];

        var result = FlipAndInvertImage(image);

        int[][] expected = [[1, 1, 0, 0], [0, 1, 1, 0], [0, 0, 0, 1], [1, 0, 1, 0]];
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FlipAndInvertImage_SingleColumn_InvertsEachCellWithoutOrderChange()
    {
        int[][] image = [[1], [0], [1]];

        var result = FlipAndInvertImage(image);

        int[][] expected = [[0], [1], [0]];
        Assert.Equal(expected, result);
    }

    private static int[][] FlipAndInvertImage(int[][] image)
    {
        foreach (var row in image)
        {
            var pending = new StackOfInt();

            foreach (var value in row)
            {
                pending.Push(value);
            }

            var index = 0;
            while (pending.TryPop(out var value))
            {
                row[index++] = 1 - value;
            }
        }

        return image;
    }
}
