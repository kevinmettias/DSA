using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CyclicallyRotatingAGridBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the stepwise ring walk against the same walk with the
// step count reduced modulo each ring's length first - so a harness whose arms disagree is timing two
// different problems. Setup fills a square template grid once and each strategy copies it before
// rotating, so the same Size must rebuild the same template and with it the same rotation; a rotation
// only moves cells within each ring, so the rotated grid holds exactly the template's values, which is
// what pins the workload's shape here rather than the private template itself.
public sealed partial class CyclicallyRotatingAGridBenchmarksTests
{
    private const int SmallestSize = 10;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload()
    {
        var rotated = BuildHarness().StepwiseQueue();

        Assert.Equal(SmallestSize, rotated.Length);
        Assert.All(rotated, row => Assert.Equal(SmallestSize, row.Length));
        Assert.Equal(
            Enumerable.Range(0, SmallestSize * SmallestSize),
            rotated.SelectMany(row => row).Order());
        Assert.Equal(
            AnswerText.Of(BuildHarness().StepwiseQueue()),
            AnswerText.Of(BuildHarness().StepwiseQueue()));
    }

    [Fact]
    public void StepwiseQueue_TenByTenGridRotatedUnreducedSteps_AgreesWithDequeRings()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.DequeRings()), AnswerText.Of(harness.StepwiseQueue()));
    }

    [Fact]
    public void DequeRings_TenByTenGridRotatedUnreducedSteps_AgreesWithStepwiseQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.StepwiseQueue()), AnswerText.Of(harness.DequeRings()));
    }

    private static CyclicallyRotatingAGridBenchmarks BuildHarness()
    {
        var harness = new CyclicallyRotatingAGridBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
