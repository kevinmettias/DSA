using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DecodeWaysBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - a bottom-up table against the same recurrence memoized from the top - so a
// harness whose arms disagree is timing two different problems. Setup builds a run of Length '1's, and
// a run of n '1's can be decoded either one digit at a time or as a pair, which is exactly the
// Fibonacci recurrence: n such digits have Fib(n + 1) decodings. That makes the reading decisive rather
// than arbitrary, and the same Length must rebuild the same run and with it the same count.
public sealed partial class DecodeWaysBenchmarksTests
{
    private const int SmallestLength = 20;

    // Fib(21) with Fib(1) = Fib(2) = 1: the decoding count of a run of twenty '1's.
    private const int ExpectedDecodingCount = 10946;

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

    private static DecodeWaysBenchmarks BuildHarness()
    {
        var harness = new DecodeWaysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
