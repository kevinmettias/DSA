using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.NthTribonacciNumber;

// LeetCode 1137. N-th Tribonacci Number: T0 = 0, T1 = T2 = 1 and
// Tn = Tn-1 + Tn-2 + Tn-3 thereafter.
//
// TribonacciByNaiveRecursion is the textbook exponential triple recursion the
// composed solution has to justify itself against; TribonacciByMemoizedTopDown
// expresses the same recurrence directly through this repo's Memoizer as O(n)
// top-down DP - the FibonacciNumberSolution shape with three prior terms summed
// instead of two.
internal static class NthTribonacciNumberSolution
{
    private const int SecondPriorTermOffset = 2;

    private const int ThirdPriorTermOffset = 3;

    // Deliberately written without this repo's primitives: every call re-derives
    // all three predecessors, so the call tree branches three ways at every level.
    public static int TribonacciByNaiveRecursion(int n)
        => n switch
        {
            0 => 0,
            1 or SecondPriorTermOffset => 1,
            _ => TribonacciByNaiveRecursion(n - 1)
                + TribonacciByNaiveRecursion(n - SecondPriorTermOffset)
                + TribonacciByNaiveRecursion(n - ThirdPriorTermOffset)
        };

    public static int TribonacciByMemoizedTopDown(int n)
        => Memoizer.Memoize<int, int>(
            n,
            (value, trib) => value switch
            {
                0 => 0,
                1 or SecondPriorTermOffset => 1,
                _ => trib(value - 1) + trib(value - SecondPriorTermOffset) + trib(value - ThirdPriorTermOffset)
            });
}
