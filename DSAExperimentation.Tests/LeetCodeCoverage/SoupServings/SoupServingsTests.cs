using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SoupServings;

// LeetCode 808. Soup Servings: a probability recurrence over the remaining (soupA,
// soupB) amounts, quantized to units of 25ml (each of the four serving choices
// subtracts a combined 4 units per turn), via this repo's own Memoizer<TState,TResult>
// - the same (state)->probability shape PredictTheWinnerTests.cs already uses, just
// over a pair of remaining amounts instead of an interval's (left, right) bounds.
public sealed partial class SoupServingsTests
{
    // Past this point the probability is within 1e-5 of 1.0 (LeetCode's own accepted
    // precedent for this problem), so the recursion is short-circuited rather than run
    // out to a state space that keeps growing with N for no observable change in the
    // (rounded) answer.
    private const int LargeNThreshold = 4800;

    [Theory]
    [InlineData(50, 0.625)]
    [InlineData(100, 0.71875)]
    public void SoupServingsProbability_LeetCodeExamples_ReturnsExpectedProbability(int n, double expected)
        => Assert.Equal(expected, SoupServingsProbability(n), precision: 5);

    [Fact]
    public void SoupServingsProbability_LargeN_ReturnsOne()
        => Assert.Equal(1.0, SoupServingsProbability(10_000), precision: 5);

    private static double SoupServingsProbability(int n)
    {
        if (n >= LargeNThreshold)
        {
            return 1.0;
        }

        var servings = (n + 24) / 25;
        return Memoizer.Memoize<(int A, int B), double>((servings, servings), Probability);

        double Probability((int A, int B) remaining, Func<(int A, int B), double> probability)
        {
            var (a, b) = remaining;
            if (a <= 0 && b <= 0)
            {
                return 0.5;
            }

            if (a <= 0)
            {
                return 1.0;
            }

            if (b <= 0)
            {
                return 0.0;
            }

            return 0.25 * (
                probability((a - 4, b))
                + probability((a - 3, b - 1))
                + probability((a - 2, b - 2))
                + probability((a - 1, b - 3)));
        }
    }
}
