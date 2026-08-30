using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumScoreTriangulationOfPolygon;

// LeetCode 1039. Minimum Score Triangulation of Polygon: interval DP over (left,
// right) vertex-index boundary pairs - for each pair, the last triangle chosen for
// that sub-polygon fixes an apex k strictly between them, whose two sides recurse
// independently into (left, k) and (k, right). This is BurstBalloonsTests' exact
// (Left, Right)-keyed Memoizer<TState,TResult> shape with min replacing max and no
// padding sentinel (a polygon has no "outside" balloon), not a second primitive.
public sealed partial class MinimumScoreTriangulationOfPolygonTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3 }, 6)]
    [InlineData(new[] { 3, 7, 4, 5 }, 144)]
    [InlineData(new[] { 1, 3, 1, 4, 1, 5 }, 13)]
    public void MinScoreTriangulation_LeetCodeExamples_ReturnsLowestTotalTriangleProductSum(
        int[] values, int expected)
        => Assert.Equal(expected, MinScoreTriangulation(values));

    private static int MinScoreTriangulation(int[] values)
    {
        return Memoizer.Memoize<(int Left, int Right), int>((0, values.Length - 1), ScoreBetween);

        int ScoreBetween((int Left, int Right) range, Func<(int Left, int Right), int> score)
        {
            var (left, right) = range;
            if (right - left < 2)
            {
                return 0;
            }

            var best = int.MaxValue;

            for (var apex = left + 1; apex < right; apex++)
            {
                var triangleScore = (values[left] * values[apex] * values[right])
                    + score((left, apex)) + score((apex, right));
                best = Math.Min(best, triangleScore);
            }

            return best;
        }
    }
}
