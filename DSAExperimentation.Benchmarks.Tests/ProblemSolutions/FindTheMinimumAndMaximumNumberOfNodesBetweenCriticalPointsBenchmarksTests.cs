using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for one question - collecting every
// critical-point index before scanning them against tracking the last two on the walk itself - so
// a harness whose arms disagree is timing two different problems. Both answers are the
// {minimum, maximum} pair, so AnswerText.Of keeps that pair's own order checked. Setup builds the
// zigzag chain from one fixed seed, so the same Length must rebuild the same list.
public sealed partial class FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().MaterializeIndicesThenScan()),
            AnswerText.Of(BuildHarness().MaterializeIndicesThenScan()));

    [Fact]
    public void MaterializeIndicesThenScan_SmallestLength_AgreesWithSinglePassConstantSpace()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SinglePassConstantSpace()),
            AnswerText.Of(harness.MaterializeIndicesThenScan()));
    }

    [Fact]
    public void SinglePassConstantSpace_SmallestLength_AgreesWithMaterializeIndicesThenScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MaterializeIndicesThenScan()),
            AnswerText.Of(harness.SinglePassConstantSpace()));
    }

    private static FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsBenchmarks BuildHarness()
    {
        var harness = new FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsBenchmarks
        {
            Length = SmallestLength,
        };
        harness.Setup();

        return harness;
    }
}
