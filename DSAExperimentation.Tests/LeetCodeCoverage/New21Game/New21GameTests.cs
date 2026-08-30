using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.New21Game;

// LeetCode 837. New 21 Game: a probability recurrence over Alice's current running
// point total via this repo's own Memoizer<TState,TResult> - the same
// (state)->probability shape SoupServingsTests already uses, just over a single
// running total instead of a pair of remaining soup amounts.
public sealed partial class New21GameTests
{
    [Theory]
    [InlineData(10, 1, 10, 1.0)]
    [InlineData(6, 1, 10, 0.6)]
    [InlineData(21, 17, 10, 0.73278)]
    public void New21GameProbability_LeetCodeExamples_ReturnsExpectedProbability(int n, int k, int maxPts, double expected)
        => Assert.Equal(expected, New21GameProbability(n, k, maxPts), precision: 5);

    [Fact]
    public void New21GameProbability_KIsZero_AliceStopsImmediatelyAtZero()
        => Assert.Equal(1.0, New21GameProbability(n: 5, k: 0, maxPts: 3), precision: 5);

    private static double New21GameProbability(int n, int k, int maxPts)
    {
        return Memoizer.Memoize<int, double>(0, Probability);

        double Probability(int points, Func<int, double> probability)
        {
            if (points >= k)
            {
                return points <= n ? 1.0 : 0.0;
            }

            var total = 0.0;

            for (var draw = 1; draw <= maxPts; draw++)
            {
                total += probability(points + draw);
            }

            return total / maxPts;
        }
    }
}
