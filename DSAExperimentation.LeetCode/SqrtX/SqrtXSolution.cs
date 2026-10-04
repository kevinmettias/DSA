using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.LeetCode.SqrtX;

// LeetCode 69. Sqrt(x): the floor of the square root of a non-negative integer,
// without using any built-in exponent/root operator.
//
// Both strategies honour that rule. The baseline is Newton's method in integer
// arithmetic, what you would write by hand; the binary-search strategy treats
// "does i^2 exceed value" as a monotone predicate over a virtual sequence and finds
// the first index where it flips - promoted to the library as IntegerSquareRoot.Floor,
// which this arm calls.
internal static class SqrtXSolution
{
    // Below this, a value is its own root (0 and 1).
    private const int SmallestValueWithSmallerRoot = 2;

    // Newton's update averages a guess with value / guess.
    private const int AverageDivisor = 2;

    // floor(sqrt(int.MaxValue)): no int has a larger root, so no guess need start higher.
    private const int LargestRoot = 46_340;

    // Newton's method on integers. Any start at or above the root works, so it starts
    // at the smaller of value / 2 (at or above the root from 2 on) and the largest
    // root an int has. While guess^2 > value - tested as guess > value / guess, so
    // nothing is squared - the update floor((guess + value / guess) / 2) is at least
    // floor(sqrt(value)) by the AM-GM inequality and strictly below guess, since
    // value / guess < guess. The guess therefore falls to exactly the floor of the root
    // and stops there; the sum never exceeds 2 * guess, so it never overflows.
    public static int RootByNewtonIteration(int value)
    {
        if (value < SmallestValueWithSmallerRoot)
        {
            return value;
        }

        var guess = Math.Min(value / AverageDivisor, LargestRoot);

        while (guess > value / guess)
        {
            guess = (guess + (value / guess)) / AverageDivisor;
        }

        return guess;
    }

    public static int RootByBinarySearch(int value) => IntegerSquareRoot.Floor(value);
}
