using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubmatricesWithAllOnes;

// LeetCode 1504. Count Submatrices With All Ones: build each column's running
// "consecutive ones ending at this row" height, exactly MaximalRectangle/
// LargestRectangleInHistogram's own per-row histogram - then, instead of just the
// largest rectangle, COUNT every all-ones rectangle bottomed at each row via this
// repo's own Stack<int> as a previous-smaller-height monotonic stack. For a fixed row,
// summing min(height) over every column subrange [k, j] is exactly LC 907's own "sum of
// subarray minimums" applied to that row's height array (SumOfSubarrayMinimumsTests'
// same Stack<int> precedent): dp[j] = dp[previousSmallerIndex] + (j - previousSmallerIndex)
// * height[j] accumulates that sum in one O(cols) sweep, and every all-ones submatrix is
// counted exactly once as (bottom row, column range).
public sealed class CountSubmatricesWithAllOnesTests
{
    [Fact]
    public void NumSubmat_ClassicExample_ReturnsThirteen()
    {
        int[][] mat = [[1, 0, 1], [1, 1, 0], [1, 1, 0]];

        Assert.Equal(13, NumSubmat(mat));
    }

    [Fact]
    public void NumSubmat_SecondLeetCodeExample_ReturnsTwentyFour()
    {
        int[][] mat = [[0, 1, 1, 0], [0, 1, 1, 1], [1, 1, 1, 0]];

        Assert.Equal(24, NumSubmat(mat));
    }

    private static int NumSubmat(int[][] mat)
    {
        var cols = mat[0].Length;
        var heights = new int[cols];
        var total = 0;

        foreach (var row in mat)
        {
            for (var col = 0; col < cols; col++)
            {
                heights[col] = row[col] == 1 ? heights[col] + 1 : 0;
            }

            total += CountRowSubmatrices(heights);
        }

        return total;
    }

    private static int CountRowSubmatrices(int[] heights)
    {
        var indices = new RepoStack();
        var dp = new int[heights.Length];
        var rowTotal = 0;

        for (var j = 0; j < heights.Length; j++)
        {
            while (indices.TryPeek(out var top) && heights[top] >= heights[j])
            {
                indices.TryPop(out _);
            }

            dp[j] = indices.TryPeek(out var previousSmaller)
                ? dp[previousSmaller] + ((j - previousSmaller) * heights[j])
                : (j + 1) * heights[j];

            indices.Push(j);
            rowTotal += dp[j];
        }

        return rowTotal;
    }
}
