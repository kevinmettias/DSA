using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FirstBadVersionBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. Setup derives the first bad version from VersionCount alone, so the same VersionCount
// must rebuild the same workload.
public sealed partial class FirstBadVersionBenchmarksTests
{
    private const int SmallestVersionCount = 1_000;

    [Fact]
    public void Setup_SameVersionCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScan()),
            AnswerText.Of(BuildHarness().LinearScan()));

    [Fact]
    public void LinearScan_AgreesWithBinarySearchLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchLowerBound(), harness.LinearScan());
    }

    [Fact]
    public void BinarySearchLowerBound_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.BinarySearchLowerBound());
    }

    private static FirstBadVersionBenchmarks BuildHarness()
    {
        var harness = new FirstBadVersionBenchmarks { VersionCount = SmallestVersionCount };
        harness.Setup();

        return harness;
    }
}
