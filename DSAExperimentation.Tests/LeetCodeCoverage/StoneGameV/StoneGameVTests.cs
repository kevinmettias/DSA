using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameV;

// LeetCode 1563. Stone Game V: repeatedly split the remaining row of stones into two
// non-empty rows, score (and keep splitting) the row with the smaller sum - either row
// on a tie - and discard the other, until one stone remains. An interval DP over
// (Left,Right) memoized by this repo's own Memoizer<TState,TResult>, the same shape
// StoneGameIIITests/UniqueBinarySearchTreesTests already use for a minimax/counting
// recurrence, just keyed on a (Left,Right) range instead of a single index.
public sealed partial class StoneGameVTests
{
    [Fact]
    public void StoneValueScore_LeetCodeExampleOne_ReturnsEighteen()
        => Assert.Equal(18, MaxScore([6, 2, 3, 4, 5, 5]));

    [Fact]
    public void StoneValueScore_LeetCodeExampleTwo_ReturnsTwentyEight()
        => Assert.Equal(28, MaxScore([7, 7, 7, 7, 7, 7, 7]));

    [Fact]
    public void StoneValueScore_SinglePile_ReturnsZero()
        => Assert.Equal(0, MaxScore([4]));

    private static int MaxScore(int[] stoneValue)
    {
        var n = stoneValue.Length;
        var prefix = new int[n + 1];

        for (var i = 0; i < n; i++)
        {
            prefix[i + 1] = prefix[i] + stoneValue[i];
        }

        return Memoizer.Memoize<(int Left, int Right), int>((0, n - 1), Best);

        int Best((int Left, int Right) range, Func<(int, int), int> best)
        {
            var (left, right) = range;

            if (left == right)
            {
                return 0;
            }

            var result = 0;

            for (var mid = left; mid < right; mid++)
            {
                var leftSum = prefix[mid + 1] - prefix[left];
                var rightSum = prefix[right + 1] - prefix[mid + 1];

                if (leftSum <= rightSum)
                {
                    result = Math.Max(result, leftSum + best((left, mid)));
                }

                if (rightSum <= leftSum)
                {
                    result = Math.Max(result, rightSum + best((mid + 1, right)));
                }
            }

            return result;
        }
    }
}
