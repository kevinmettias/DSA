using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FourDivisorsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. Setup draws the numbers from a fixed seed, so the same Length must rebuild the same
// workload.
public sealed partial class FourDivisorsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().FullRangeScan()),
            AnswerText.Of(BuildHarness().FullRangeScan()));

    [Fact]
    public void FullRangeScan_AgreesWithBinarySearchAnchored()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchAnchored(), harness.FullRangeScan());
    }

    [Fact]
    public void BinarySearchAnchored_AgreesWithFullRangeScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullRangeScan(), harness.BinarySearchAnchored());
    }

    private static FourDivisorsBenchmarks BuildHarness()
    {
        var harness = new FourDivisorsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
