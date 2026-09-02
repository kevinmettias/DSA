namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCutsToDivideACircle;

// LeetCode 2481. Minimum Cuts to Divide a Circle: purely arithmetic. A cut through the
// center (a diameter) yields 2 equal slices per cut, so an even n needs exactly n / 2
// such cuts. An odd n can never pair opposite slices across the center - among n
// evenly-spaced points around the circle, no point has an antipodal partner also on
// that list unless n is even - so every cut must instead be a single radius cut,
// needing n of them. n == 1 needs no cut at all. No existing repo primitive
// (Stack/HashMap/DisjointSet/Memoizer/...) genuinely models this: it is a closed-form
// parity check with no recursive or search structure to compose over, the same
// "lighter repo-primitive fit" shape PowerOfTwoTests.cs/NumberOf1BitsTests.cs already
// use for bit/arithmetic-only Easy problems.
public sealed partial class MinimumCutsToDivideACircleTests
{
    [Theory]
    [InlineData(4, 2)]
    [InlineData(3, 3)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(100, 50)]
    public void NumberOfCuts_LeetCodeExamples_ReturnsExpectedMinimumCuts(int n, int expected)
        => Assert.Equal(expected, NumberOfCuts(n));

    private static int NumberOfCuts(int n) => n == 1 ? 0 : n % 2 == 0 ? n / 2 : n;
}
