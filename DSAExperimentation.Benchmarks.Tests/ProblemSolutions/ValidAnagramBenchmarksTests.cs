using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidAnagramBenchmarks (ARCHITECTURE 17.9): its two arms are
// ValidAnagramSolution's competing strategies for the same question - the sorted brute-force
// comparison against the hash-map frequency count - so a harness whose arms disagree is answering
// two different questions.
//
// Setup makes target a rotation of source, so the two strings are one multiset and LC 242's answer
// is "yes", which is asserted alongside the agreement so a shared wrong verdict cannot pass. Both
// arms return bool, so this harness can only ever witness the verdict and not the frequency table
// behind it; the same rotation makes the workload equally true for every Length, so the Setup test
// pins the documented outcome rather than the drawn letters.
public sealed partial class ValidAnagramBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's documented outcome: target is a rotation of source, so the multiset is identical.
    private const bool ExpectedIsAnagram = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().IsAnagramByBruteForce(), BuildHarness().IsAnagramByBruteForce());
        Assert.Equal(ExpectedIsAnagram, BuildHarness().IsAnagramByBruteForce());
    }

    [Fact]
    public void IsAnagramByBruteForce_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsAnagramByHashMapFrequencyCount(), harness.IsAnagramByBruteForce());
        Assert.Equal(ExpectedIsAnagram, harness.IsAnagramByBruteForce());
    }

    [Fact]
    public void IsAnagramByHashMapFrequencyCount_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsAnagramByBruteForce(), harness.IsAnagramByHashMapFrequencyCount());
        Assert.Equal(ExpectedIsAnagram, harness.IsAnagramByHashMapFrequencyCount());
    }

    private static ValidAnagramBenchmarks BuildHarness()
    {
        var harness = new ValidAnagramBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
