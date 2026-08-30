using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NthTribonacciNumber;

// LeetCode 1137. N-th Tribonacci Number: Tn = Tn-1 + Tn-2 + Tn-3, the same
// textbook recurrence FibonacciNumberTests already expresses directly through
// this repo's Memoizer<TState,TResult> - just three prior terms instead of
// two.
public sealed partial class NthTribonacciNumberTests
{
    [Fact]
    public void Tribonacci_BaseCases_ReturnKnownSeedValues()
    {
        Assert.Equal(0, Tribonacci(0));
        Assert.Equal(1, Tribonacci(1));
        Assert.Equal(1, Tribonacci(2));
    }

    [Fact]
    public void Tribonacci_LeetCodeExampleOne_ReturnsFour()
        => Assert.Equal(4, Tribonacci(4));

    [Fact]
    public void Tribonacci_LeetCodeExampleTwo_ReturnsLargeValue()
        => Assert.Equal(1_389_537, Tribonacci(25));

    private static int Tribonacci(int n)
        => Memoizer.Memoize<int, int>(
            n,
            (value, trib) => value switch
            {
                0 => 0,
                1 or 2 => 1,
                _ => trib(value - 1) + trib(value - 2) + trib(value - 3)
            });
}
