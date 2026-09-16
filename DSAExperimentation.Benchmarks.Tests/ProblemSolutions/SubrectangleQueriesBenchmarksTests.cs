using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubrectangleQueriesBenchmarks (ARCHITECTURE 17.9): both arms replay one
// seeded script of LC 1476 overwrites against their own backing store - a jagged array versus a
// DynamicArray of DynamicArrays - so a harness whose arms disagree is timing two different
// problems. Each arm constructs its own store inside the call, so one harness instance is safe
// to call twice in either order; Setup builds the script, so the same size must rebuild it.
//
// Both arms return one cell of the grid rather than the whole store, so agreement witnesses
// that both stores agree at the far corner, not that they agree everywhere. That is the
// strongest check the arms' return type allows; the cell is written by the script's overlapping
// updates, so it is not a fixed constant the arms could both be ignoring.
public sealed partial class SubrectangleQueriesBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ArrayBacked()),
            AnswerText.Of(BuildHarness().ArrayBacked()));

    [Fact]
    public void ArrayBacked_AgreesWithDynamicArrayBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayBacked(), harness.DynamicArrayBacked());
    }

    [Fact]
    public void DynamicArrayBacked_AgreesWithArrayBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicArrayBacked(), harness.ArrayBacked());
    }

    private static SubrectangleQueriesBenchmarks BuildHarness()
    {
        var harness = new SubrectangleQueriesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
