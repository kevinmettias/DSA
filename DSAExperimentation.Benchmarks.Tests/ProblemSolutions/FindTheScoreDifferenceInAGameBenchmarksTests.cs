using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheScoreDifferenceInAGameBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. Setup draws from a fixed seed, so the same Length must rebuild the same
// workload - otherwise two published numbers were never comparable in the first place.
public sealed partial class FindTheScoreDifferenceInAGameBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScan()),
            AnswerText.Of(BuildHarness().LinearScan()));

    [Fact]
    public void LinearScan_AgreesWithMinHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MinHeap(), harness.LinearScan());
    }

    [Fact]
    public void MinHeap_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.MinHeap());
    }

    private static FindTheScoreDifferenceInAGameBenchmarks BuildHarness()
    {
        var harness = new FindTheScoreDifferenceInAGameBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
