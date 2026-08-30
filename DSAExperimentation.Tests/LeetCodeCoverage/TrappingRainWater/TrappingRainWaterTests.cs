using RainStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TrappingRainWater;

// LeetCode 42. Trapping Rain Water: the classic monotonic-stack sweep over
// this repo's own Stack<int>, holding bar indices (not heights). Once a
// taller bar arrives, every shorter bar popped off the top had its floor
// bounded on the left by the new stack top and on the right by the current
// bar - min(leftWall, rightWall) - floor, times the gap width, is exactly
// the water that bar's position trapped.
public sealed partial class TrappingRainWaterTests
{
    [Fact]
    public void Trap_ClassicExample_ReturnsTotalTrappedWater()
    {
        int[] height = [0, 1, 0, 2, 1, 0, 1, 3, 2, 1, 2, 1];

        Assert.Equal(6, Trap(height));
    }

    [Fact]
    public void Trap_TwoBasinsBetweenThreeWalls_ReturnsSummedTrappedWater()
    {
        int[] height = [4, 2, 0, 3, 2, 5];

        Assert.Equal(9, Trap(height));
    }

    private static int Trap(int[] height)
    {
        var indices = new RainStack();
        var water = 0;

        for (var i = 0; i < height.Length; i++)
        {
            while (indices.TryPeek(out var top) && height[top] < height[i])
            {
                indices.TryPop(out _);

                if (!indices.TryPeek(out var left))
                {
                    break;
                }

                var width = i - left - 1;
                var boundedHeight = Math.Min(height[left], height[i]) - height[top];
                water += width * boundedHeight;
            }

            indices.Push(i);
        }

        return water;
    }
}
