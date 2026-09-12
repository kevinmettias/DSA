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
        => n <= 1 ? n : FibByNaiveRecursion(n - 1) + FibByNaiveRecursion(n - SecondPredecessorOffset);

    public static int FibByMemoizedTopDown(int n)
        => Memoizer.Memoize<int, int>(
            n,
            (value, fib) => value <= 1 ? value : fib(value - 1) + fib(value - SecondPredecessorOffset));
}
