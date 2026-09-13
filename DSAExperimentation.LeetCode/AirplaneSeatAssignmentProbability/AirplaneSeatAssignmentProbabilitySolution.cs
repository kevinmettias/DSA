using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.AirplaneSeatAssignmentProbability;

// LeetCode 1227. Airplane Seat Assignment Probability: f(1) = 1; for n >= 2 the first
// passenger picks uniformly among n seats - seat 1 (probability 1/n, everyone after
// sits in their own seat, so the nth passenger succeeds), seat n (probability 1/n,
// immediate failure), or some seat k in [2, n-1] (probability 1/n each), which reduces
// the remaining problem to f(n - k + 1). Summing gives
// f(n) = (1 + sum_{j=2}^{n-1} f(j)) / n.
//
// NthPersonGetsNthSeatByMemoizedRecursion states that recurrence literally, over this
// repo's own Memoizer - the same shape DivisorGame and NimGame use for theirs.
// NthPersonGetsNthSeatByClosedForm is the O(1) formula the recursion provably reduces
// to ("1 if n == 1 else 0.5"), which is the arm the recursion is measured against;
// keeping both here is what finally puts them under the same assertions instead of one
// living in a test and the other only in a benchmark.
internal static class AirplaneSeatAssignmentProbabilitySolution
{
    // First seat index after the base case (seat 1) that the recursion sums over.
    private const int FirstAlternativeSeat = 2;

    // Closed-form probability for every seat count past the first: the recursion
    // itself reduces to this constant.
    private const double NonFirstSeatProbability = 0.5;

    // The definition: O(n^2) memoized recursion summing every shorter seat count.
    public static double NthPersonGetsNthSeatByMemoizedRecursion(int n)
        => Memoizer.Memoize<int, double>(n, (current, probability) =>
        {
            if (current == 1)
            {
                return 1.0;
            }

            var sum = 1.0;

            for (var j = FirstAlternativeSeat; j < current; j++)
            {
                sum += probability(j);
            }

            return sum / current;
        });

    // The closed form: a single passenger always takes their own seat, and for every
    // larger plane the first and last seats are symmetric, so the nth passenger's
    // chance settles at one half.
    public static double NthPersonGetsNthSeatByClosedForm(int n)
        => n == 1 ? 1.0 : NonFirstSeatProbability;
}
