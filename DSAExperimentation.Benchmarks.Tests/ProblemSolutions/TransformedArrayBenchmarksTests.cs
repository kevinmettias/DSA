using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TransformedArrayBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the step-walk baseline that follows each shift one index at a
// time against the modulo walk that computes the destination directly - so a harness whose arms
// disagree is timing two different problems. Both arms return one transformed value per input
// index, the order LeetCode's own result pins, so the arrays are rendered order-sensitively. Setup
// draws the shifts from the full [-Length, Length] range from a fixed seed, so the same Length must
// rebuild the same workload.
public sealed partial class TransformedArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().StepWalk()),
            AnswerText.Of(BuildHarness().StepWalk()));

    [Fact]
    public void StepWalk_SmallestLength_AgreesWithModuloWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ModuloWalk()), AnswerText.Of(harness.StepWalk()));
    }

    [Fact]
    public void ModuloWalk_SmallestLength_AgreesWithStepWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.StepWalk()), AnswerText.Of(harness.ModuloWalk()));
    }

    private static TransformedArrayBenchmarks BuildHarness()
    {
        var harness = new TransformedArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
