using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RepeatedSubstringPatternBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - trying every proper divisor of the length against a
// prefix-length test derived from the KMP failure function - so a harness whose arms disagree is
// timing two different problems. Setup draws the text from one fixed seed and the seed is the whole
// of its state, so the same Length must rebuild the same text; otherwise two published numbers were
// never comparable in the first place.
//
// Both arms answer a yes/no question over that text, so agreement is weaker than for a sequence
// answer: it would also hold if both arms always said no. The class comment's "astronomically
// unlikely" random text is not a fact the assertion can lean on, so the agreement is stated for
// what it is rather than dressed up with a literal neither arm's contract guarantees.
public sealed partial class RepeatedSubstringPatternBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().HasRepeatedSubstringPatternByDivisorBruteForce(),
            BuildHarness().HasRepeatedSubstringPatternByDivisorBruteForce());

    [Fact]
    public void HasRepeatedSubstringPatternByDivisorBruteForce_SeededText_AgreesWithTheKmpFailureFunction()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.HasRepeatedSubstringPatternByKmpFailureFunction(),
            harness.HasRepeatedSubstringPatternByDivisorBruteForce());
    }

    [Fact]
    public void HasRepeatedSubstringPatternByKmpFailureFunction_SeededText_AgreesWithTheDivisorBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.HasRepeatedSubstringPatternByDivisorBruteForce(),
            harness.HasRepeatedSubstringPatternByKmpFailureFunction());
    }

    private static RepeatedSubstringPatternBenchmarks BuildHarness()
    {
        var harness = new RepeatedSubstringPatternBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
