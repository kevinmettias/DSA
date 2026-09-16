using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSequencesToKBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - enumerating every sequence of operations against the memoized
// prime-exponent fold - so a harness whose arms disagree is timing two different problems. Setup
// seeds nums, so the same Length must rebuild the same array.
public sealed partial class CountSequencesToKBenchmarksTests
{
    private const int SmallestLength = 8;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: the target is fixed at 1, which leaving every element unchanged
        // always reaches, so both arms do real search work instead of short-circuiting on an
        // unreachable target.
        Assert.InRange(first.BruteForceSearch(), 1L, long.MaxValue);
        Assert.Equal(first.BruteForceSearch(), second.BruteForceSearch());
    }

    [Fact]
    public void BruteForceSearch_ReachableTarget_AgreesWithPrimeExponentMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrimeExponentMemo(), harness.BruteForceSearch());
    }

    [Fact]
    public void PrimeExponentMemo_ReachableTarget_AgreesWithBruteForceSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSearch(), harness.PrimeExponentMemo());
    }

    private static CountSequencesToKBenchmarks BuildHarness()
    {
        var harness = new CountSequencesToKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
