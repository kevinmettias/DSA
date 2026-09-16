using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TwoSumBenchmarks (ARCHITECTURE 17.9): its two arms are TwoSumSolution's
// competing strategies for the same question - the additive pair scan against the one-pass hash
// map - so a harness whose arms disagree is timing two different problems.
//
// The target is deliberately unreachable (every generated value is positive, the target is -1),
// which is what forces both strategies through a worst-case scan. It is also why both arms answer
// false on every workload this harness can build, so the agreement below witnesses that the two
// arms return the same verdict for the same values, not that either arm found a pair: with both
// [Benchmark] return types bool and the matched indices discarded into out parameters, the pair
// itself is not observable from this harness at all, and pinning the values by content would need
// the benchmark to expose them - a harness decision, not this file's.
public sealed partial class TwoSumBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's documented outcome: no two positive values sum to the negative target.
    private const bool ExpectedHasPair = false;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().TryFindIndicesByBruteForce(), BuildHarness().TryFindIndicesByBruteForce());
        Assert.Equal(ExpectedHasPair, BuildHarness().TryFindIndicesByBruteForce());
    }

    [Fact]
    public void TryFindIndicesByBruteForce_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TryFindIndicesByHashMap(), harness.TryFindIndicesByBruteForce());
        Assert.Equal(ExpectedHasPair, harness.TryFindIndicesByBruteForce());
    }

    [Fact]
    public void TryFindIndicesByHashMap_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TryFindIndicesByBruteForce(), harness.TryFindIndicesByHashMap());
        Assert.Equal(ExpectedHasPair, harness.TryFindIndicesByHashMap());
    }

    private static TwoSumBenchmarks BuildHarness()
    {
        var harness = new TwoSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
