using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FirstUniqueCharacterInAStringBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. Setup draws the string from a fixed seed, so the same Length must rebuild the
// same workload.
//
// The agreement is weaker than it looks and the assertions say only what the fixture supports: the
// workload is deliberately longer than the alphabet, so every one of the 26 letters recurs and both
// arms are forced to walk the whole string. What they agree on is that walk's outcome, and an arm
// that mis-indexed a surviving unique character would have to be mis-indexing it identically in the
// other strategy to escape this comparison.
public sealed partial class FirstUniqueCharacterInAStringBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithHashMapTwoPass()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapTwoPass(), harness.BruteForce());
    }

    [Fact]
    public void HashMapTwoPass_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.HashMapTwoPass());
    }

    private static FirstUniqueCharacterInAStringBenchmarks BuildHarness()
    {
        var harness = new FirstUniqueCharacterInAStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
