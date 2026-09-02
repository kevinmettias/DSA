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
        var prefix = BuildPrefixSums(stoneValue);
        return Memoizer.Memoize<(int Left, int Right), int>(
            (0, stoneValue.Length - 1), (range, best) => Best(range, prefix, best));
    }

    private static int[] BuildPrefixSums(int[] stoneValue)
    {
        var n = stoneValue.Length;
        var prefix = new int[n + 1];

        for (var i = 0; i < n; i++)
        {
            prefix[i + 1] = prefix[i] + stoneValue[i];
        }

        return prefix;
    }

    private static int Best((int Left, int Right) range, int[] prefix, Func<(int, int), int> best)
    {
        var (left, right) = range;

        if (left == right)
        {
            return 0;
        }

        var result = 0;

        for (var mid = left; mid < right; mid++)
        {
            var score = ScoreForSplit(prefix, range, mid, best);
            result = Math.Max(result, score);
        }

        return result;
    }

    // The score contributed by splitting `range` at `mid`: whichever side sums to no
    // more than the other is the one scored (and recursed into) this round, per Stone
    // Game V's rule that a tie lets either side be kept - the block `Best` repeats
    // once per candidate `mid`.
    private static int ScoreForSplit(int[] prefix, (int Left, int Right) range, int mid, Func<(int, int), int> best)
    {
        var (left, right) = range;
        var leftSum = prefix[mid + 1] - prefix[left];
        var rightSum = prefix[right + 1] - prefix[mid + 1];
        var score = 0;

        if (leftSum <= rightSum)
        {
            score = Math.Max(score, leftSum + best((left, mid)));
        }

        if (rightSum <= leftSum)
        {
            score = Math.Max(score, rightSum + best((mid + 1, right)));
        }

        return score;
    }
}
