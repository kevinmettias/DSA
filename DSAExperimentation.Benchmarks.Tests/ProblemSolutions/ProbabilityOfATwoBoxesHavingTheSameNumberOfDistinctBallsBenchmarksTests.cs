using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for one question - the probability that both boxes end
// up with the same number of distinct colours - so a harness whose arms disagree is timing two
// different problems. TypeCount is the only [Params] axis and Setup derives the ball counts from it,
// so the same TypeCount must rebuild the same balls. The answer is a probability, so the arms are
// compared within the class's own tolerance rather than for exact double equality.
public sealed partial class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsBenchmarksTests
{
    private const int SmallestTypeCount = 4;

    private const double RelativeTolerance = 1e-9;

    [Fact]
    public void Setup_SameTypeCount_RebuildsTheSameBallCounts() =>
        Assert.Equal(
            BuildHarness().HandRolledRecursion(),
            BuildHarness().HandRolledRecursion(),
            RelativeTolerance);

    [Fact]
    public void HandRolledRecursion_AgreesWithBacktrackPrimitive()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BacktrackPrimitive(), harness.HandRolledRecursion(), RelativeTolerance);
    }

    [Fact]
    public void BacktrackPrimitive_AgreesWithHandRolledRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HandRolledRecursion(), harness.BacktrackPrimitive(), RelativeTolerance);
    }

    private static ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsBenchmarks BuildHarness()
    {
        var harness = new ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsBenchmarks
        {
            TypeCount = SmallestTypeCount,
        };
        harness.Setup();

        return harness;
    }
}
