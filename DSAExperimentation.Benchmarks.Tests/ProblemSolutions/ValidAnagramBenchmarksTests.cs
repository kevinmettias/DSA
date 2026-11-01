using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidAnagramBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms
// to agreeing with each other, which a shared wrong verdict would pass, so this pins the baseline's
// verdict to the one the workload's construction fixes.
//
// Setup makes target a rotation of source, so the two strings are one multiset and LC 242's answer
// is "yes". Both arms return bool, so this harness can only ever witness the verdict and not the
// frequency table behind it; the same rotation makes the workload equally true for every Length, so
// the test pins the documented outcome rather than the drawn letters.
public sealed partial class ValidAnagramBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's documented outcome: target is a rotation of source, so the multiset is identical.
    private const bool ExpectedIsAnagram = true;

    [Fact]
    public void IsAnagramByBruteForce_RotatedTarget_ReportsAnAnagram() =>
        Assert.Equal(ExpectedIsAnagram, BuildHarness().IsAnagramByBruteForce());

    private static ValidAnagramBenchmarks BuildHarness()
    {
        var harness = new ValidAnagramBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
