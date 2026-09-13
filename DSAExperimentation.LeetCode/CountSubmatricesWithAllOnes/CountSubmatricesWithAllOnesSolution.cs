using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.CountSubmatricesWithAllOnes;

// LeetCode 1504. Count Submatrices With All Ones: how many all-ones submatrices does a
// binary matrix contain?
//
// Both strategies share the same reduction: build each column's running "consecutive ones
// ending at this row" height - exactly MaximalRectangle/LargestRectangleInHistogram's own
// per-row histogram - and then count the rectangles bottomed at that row. For a fixed row,
// summing min(height) over every column subrange [left, right] is LC 907's "sum of subarray
// minimums" applied to that row's height array, so every all-ones submatrix is counted
// exactly once as (bottom row, column range). The strategies differ only in how that per-row
// sum is obtained.
internal static class CountSubmatricesWithAllOnesSolution
{
    private const int One = 1;

    // The textbook answer: for each right boundary column, walk left while the run of ones
    // survives, tracking the running minimum height. O(rows * cols^2), plain BCL throughout -
    // this is the arm the monotonic-stack reduction below has to justify itself against.
    public static int CountByRunningMinScan(int[][] mat)
    {
        var cols = mat[0].Length;
        var heights = new int[cols];
        var total = 0;

        foreach (var row in mat)
        {
            UpdateHeights(heights, row);
            total += CountRowByRunningMin(heights);
        }

        return total;
    }

    private static int CountRowByRunningMin(int[] heights)
    {
        var rowTotal = 0;

        for (var right = 0; right < heights.Length; right++)
        {
            var minHeight = heights[right];

            for (var left = right; left >= 0 && heights[left] != 0; left--)
            {
                minHeight = Math.Min(minHeight, heights[left]);
                rowTotal += minHeight;
            }
        }

        return rowTotal;
    }

    // This repo's own Stack<int> as a previous-smaller-height monotonic stack, the same
    // precedent SumOfSubarrayMinimums and LargestRectangleInHistogram use:
    // dp[j] = dp[previousSmaller] + (j - previousSmaller) * height[j] accumulates the row's
    // whole sum-of-minimums in one O(cols) sweep, taking the total to O(rows * cols).
    public static int CountByMonotonicStackDp(int[][] mat)
    {
        var cols = mat[0].Length;
        var heights = new int[cols];
        var total = 0;

        foreach (var row in mat)
        {
            UpdateHeights(heights, row);
            total += CountRowByMonotonicStack(heights);
        }

        return total;
    }

    private static int CountRowByMonotonicStack(int[] heights)
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

    // The histogram both strategies read: a column's height grows while the column holds a
    // one and resets to zero the moment it does not.
    private static void UpdateHeights(int[] heights, int[] row)
    {
        for (var col = 0; col < heights.Length; col++)
        {
            heights[col] = row[col] == One ? heights[col] + 1 : 0;
        }
    }
}
