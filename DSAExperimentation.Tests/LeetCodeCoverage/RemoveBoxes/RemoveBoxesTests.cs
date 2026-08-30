using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveBoxes;

// LeetCode 546. Remove Boxes: interval DP over (left, right, extra) triples - extra
// counts same-colored boxes already known to sit just past right, so removing
// boxes[right] together with them scores (extra+1)^2, or boxes[right] can be
// deferred by first clearing out some inner sub-range so an earlier same-colored
// box at i becomes adjacent to it, growing its own extra count by one. This repo's
// own Memoizer<TState,TResult> supplies the cache, keyed by that 3-tuple, the same
// tuple-state shape BurstBalloonsTests already uses for (left, right).
public sealed partial class RemoveBoxesTests
{
    [Theory]
    [InlineData(new[] { 1, 3, 2, 2, 2, 3, 4, 3, 1 }, 23)]
    [InlineData(new[] { 1, 1, 1 }, 9)]
    [InlineData(new[] { 1 }, 1)]
    public void RemoveBoxes_LeetCodeExamples_ReturnsMaximumPoints(int[] boxes, int expected)
        => Assert.Equal(expected, MaxPoints(boxes));

    private static int MaxPoints(int[] boxes)
    {
        return Memoizer.Memoize<(int Left, int Right, int Extra), int>((0, boxes.Length - 1, 0), Points);

        int Points((int Left, int Right, int Extra) state, Func<(int Left, int Right, int Extra), int> points)
        {
            var (left, right, extra) = state;
            if (left > right)
            {
                return 0;
            }

            var best = points((left, right - 1, 0)) + (extra + 1) * (extra + 1);

            for (var i = left; i < right; i++)
            {
                if (boxes[i] == boxes[right])
                {
                    best = Math.Max(best, points((left, i, extra + 1)) + points((i + 1, right - 1, 0)));
                }
            }

            return best;
        }
    }
}
