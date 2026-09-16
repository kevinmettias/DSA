using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - enumerating all
// m^n arrays against the closed binomial-times-power form - so a harness whose arms disagree is
// timing two different problems, not two ways of answering one. Setup derives the match count from
// ArrayLength alone, so the same ArrayLength must rebuild the same match count and with it the same
// workload; otherwise two published numbers were never comparable in the first place.
//
// The match count is private and the array count is the only thing either arm reports, so the
// documented shape is asserted through that: the answer counts length-n arrays over a 2-value
// alphabet, and there are only 2^n such arrays in total.
public sealed partial class CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarksTests
{
    private const int SmallestArrayLength = 10;

    private const long ArrayCount = 1L << SmallestArrayLength;

    [Fact]
    public void Setup_SameArrayLength_RebuildsTheSameMatchCount()
    {
        Assert.InRange(BuildHarness().BruteForce(), 0, ArrayCount);
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_BinaryAlphabetWithHalfTheGapsMatching_AgreesWithModularCombinatorics()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ModularCombinatorics(), harness.BruteForce());
    }

    [Fact]
    public void ModularCombinatorics_BinaryAlphabetWithHalfTheGapsMatching_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ModularCombinatorics());
    }

    private static CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarks
        {
            ArrayLength = SmallestArrayLength,
        };
        harness.Setup();

        return harness;
    }
}
