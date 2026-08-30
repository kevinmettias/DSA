using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AirplaneSeatAssignmentProbability;

// LeetCode 1227. Airplane Seat Assignment Probability: f(1) = 1; for n >= 2, the
// first passenger picks uniformly among n seats - seat 1 (probability 1/n, everyone
// after sits in their own seat, so the nth passenger succeeds), seat n (probability
// 1/n, immediate failure), or some seat k in [2, n-1] (probability 1/n each), which
// reduces the remaining problem to f(n-k+1). Summing gives
// f(n) = (1 + sum_{j=2}^{n-1} f(j)) / n - natural-looking recursion via this repo's
// own Memoizer, the same shape DivisorGameTests/NimGameTests already use for their
// own recurrences. The recursion reduces to the well-known "1 if n == 1 else 0.5"
// closed form, which the benchmark compares against.
public sealed class AirplaneSeatAssignmentProbabilityTests
{
    [Theory]
    [InlineData(1, 1.0)]
    [InlineData(2, 0.5)]
    [InlineData(3, 0.5)]
    [InlineData(10, 0.5)]
    public void NthPersonGetsNthSeat_LeetCodeExamples_MatchesExpectedProbability(int n, double expected)
        => Assert.Equal(expected, NthPersonGetsNthSeat(n), precision: 9);

    private static double NthPersonGetsNthSeat(int n)
        => Memoizer.Memoize<int, double>(n, (current, probability) =>
        {
            if (current == 1)
            {
                return 1.0;
            }

            var sum = 1.0;

            for (var j = 2; j < current; j++)
            {
                sum += probability(j);
            }

            return sum / current;
        });
}
