using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.FibonacciNumber;

// LeetCode 509. Fibonacci Number: fib(n) = fib(n-1) + fib(n-2).
//
// FibByNaiveRecursion is the textbook exponential double recursion the composed
// solution has to justify itself against; FibByMemoizedTopDown expresses the same
// recurrence directly through this repo's Memoizer as O(n) top-down DP - the same
// ClimbingStairsTests precedent, minus the "+1 shift" Climbing Stairs applies on top
// of the same shape.
internal static class FibonacciNumberSolution
{
    // Offset back to the second predecessor in the recurrence fib(n-1) + fib(n-2).
    private const int SecondPredecessorOffset = 2;

    // The textbook O(2^n) double recursion, deliberately written without this
    // repo's primitives - the arm FibByMemoizedTopDown is measured against.
    public static int FibByNaiveRecursion(int n)
        => n <= 1
            ? n
            : SumOfPredecessors(FibByNaiveRecursion(n - 1), FibByNaiveRecursion(n - SecondPredecessorOffset));

    // The recurrence's own sum, reached only once both predecessors have been computed.
    private static int SumOfPredecessors(int first, int second) => first + second;

    public static int FibByMemoizedTopDown(int n) =>
        Memoizer.Memoize<int, int>(n, new SumOfTwoPredecessors());

    // The recurrence, as a named type: 0 and 1 are themselves, and every later value is
    // the sum of its two predecessors.
    private sealed class SumOfTwoPredecessors : IRecurrence<int, int>
    {
        public int Replay(int value, IRecurrence<int, int> rest)
        {
            if (value <= 1)
            {
                return value;
            }

            var firstPredecessor = rest.Replay(value - 1, rest);
            var secondPredecessor = rest.Replay(value - SecondPredecessorOffset, rest);

            return SumOfPredecessors(firstPredecessor, secondPredecessor);
        }
    }
}
