using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DecodeWaysBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - a bottom-up table against the same recurrence memoized from the top - so a
// harness whose arms disagree is timing two different problems. Setup builds a run of Length '1's, and
// a run of n '1's can be decoded either one digit at a time or as a pair, which is exactly the
// Fibonacci recurrence: n such digits have Fib(n + 1) decodings. That makes the reading decisive rather
// than arbitrary, and the same Length must rebuild the same run and with it the same count. Past 44
// digits the run stops and '7's fill the string to LC 91's cap, which is what keeps the largest
// length's count inside the int LC 91 promises.
public sealed partial class DecodeWaysBenchmarksTests
{
    private const int SmallestLength = 20;

    // Fib(21) with Fib(1) = Fib(2) = 1: the decoding count of a run of twenty '1's.
    private const int ExpectedDecodingCount = 10946;

    private const int LongestLength = 100;

    // The run of 44 '1's and the first '7' after it all pair with their neighbours, and no later '7'
    // pairs with anything, so the longest string decodes as many ways as 45 pairable digits do.
    private const int PairableDigitsAtLongestLength = 45;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedDecodingCount, BuildHarness().Tabulation());
        Assert.Equal(BuildHarness().Tabulation(), BuildHarness().Tabulation());
    }

    [Fact]
    public void Tabulation_TwentyConsecutiveOnes_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
    }

    [Fact]
    public void Memoized_TwentyConsecutiveOnes_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
    }

    [Fact]
    public void Tabulation_LongestLength_CountsTheLargestFibonacciDecodingsAnIntHolds()
    {
        var countIndex = PairableDigitsAtLongestLength + 1;
        var expected = Fibonacci(countIndex);
        var oneMorePairableDigit = Fibonacci(countIndex + 1);

        Assert.InRange(expected, 0L, int.MaxValue);
        Assert.True(oneMorePairableDigit > int.MaxValue);
        Assert.Equal(expected, BuildHarness(LongestLength).Tabulation());
    }

    private static DecodeWaysBenchmarks BuildHarness(int length = SmallestLength)
    {
        var harness = new DecodeWaysBenchmarks { Length = length };
        harness.Setup();

        return harness;
    }

    // Fib(1) = Fib(2) = 1, counted up directly rather than read off either arm.
    private static long Fibonacci(int index)
    {
        var (current, next) = (0L, 1L);

        for (var step = 0; step < index; step++)
        {
            (current, next) = (next, current + next);
        }

        return current;
    }
}
