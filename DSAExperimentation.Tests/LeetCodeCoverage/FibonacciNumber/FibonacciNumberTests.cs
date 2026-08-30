using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FibonacciNumber;

// LeetCode 509. Fibonacci Number: fib(n) = fib(n-1) + fib(n-2), the textbook
// recurrence this repo's Memoizer exists to express directly - the same
// ClimbingStairsTests precedent, minus the "+1 shift" Climbing Stairs applies on
// top of the same shape.
public sealed partial class FibonacciNumberTests
{
    [Fact]
    public void Fib_BaseCases_ReturnInputUnchanged()
    {
        Assert.Equal(0, Fib(0));
        Assert.Equal(1, Fib(1));
    }

    [Fact]
    public void Fib_ClassicExample_ReturnsCorrectValue()
    {
        Assert.Equal(5, Fib(5));
    }

    [Fact]
    public void Fib_LargerInput_ReturnsCorrectValue()
    {
        Assert.Equal(6765, Fib(20));
    }

    private static int Fib(int n)
        => Memoizer.Memoize<int, int>(n, (value, fib) => value <= 1 ? value : fib(value - 1) + fib(value - 2));
}
