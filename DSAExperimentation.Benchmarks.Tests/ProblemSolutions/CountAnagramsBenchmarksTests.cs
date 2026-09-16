using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountAnagramsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - enumerating each word's distinct permutations into a HashSet
// against this repo's modular factorial/inverse-factorial table - so a harness whose arms disagree is
// timing two different problems. Both arms return a long, so they are compared directly. Setup draws
// from one fixed seed, so the same WordLength must rebuild the same sentence of twenty words; the
// sentence itself is private, so the two observables the arms expose are what pins it down.
public sealed partial class CountAnagramsBenchmarksTests
{
    private const int SmallestWordLength = 4;

    [Fact]
    public void Setup_SameWordLength_RebuildsTheSameSentence() =>
        Assert.Equal(BuildHarness().BruteForcePermutations(), BuildHarness().BruteForcePermutations());

    [Fact]
    public void BruteForcePermutations_FourLetterWords_AgreesWithModularFactorial()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ModularFactorial(), harness.BruteForcePermutations());
    }

    [Fact]
    public void ModularFactorial_FourLetterWords_AgreesWithBruteForcePermutations()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePermutations(), harness.ModularFactorial());
    }

    private static CountAnagramsBenchmarks BuildHarness()
    {
        var harness = new CountAnagramsBenchmarks { WordLength = SmallestWordLength };
        harness.Setup();

        return harness;
    }
}
