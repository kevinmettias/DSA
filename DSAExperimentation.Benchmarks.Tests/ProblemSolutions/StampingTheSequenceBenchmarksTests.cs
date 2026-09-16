using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for StampingTheSequenceBenchmarks (ARCHITECTURE 17.9): both arms are
// StampingTheSequenceSolution's recovery of the same move sequence for the same target, running
// the identical reverse simulation and differing only in how the discovered indices are put back
// into forward order - a list that prepends, against a stack that unwinds once - so a harness
// whose arms disagree is timing two different problems. Setup is a pure function of Repeats, so
// the same Repeats must rebuild the same target.
//
// The target is the stamp repeated, so the reverse simulation discovers one stamp per repeat and
// the arms are meant to return the very same sequence rather than two sequences that both replay
// onto the target: the order-sensitive rendering compares them move by move.
public sealed partial class StampingTheSequenceBenchmarksTests
{
    private const int SmallestRepeats = 200;

    [Fact]
    public void Setup_SameRepeats_RebuildsTheSameTarget() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ListPrepend()),
            AnswerText.Of(BuildHarness().ListPrepend()));

    [Fact]
    public void ListPrepend_TwoHundredStampRepeats_AgreesWithStackAndReverse()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.StackAndReverse()), AnswerText.Of(harness.ListPrepend()));
    }

    [Fact]
    public void StackAndReverse_TwoHundredStampRepeats_AgreesWithListPrepend()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.ListPrepend()), AnswerText.Of(harness.StackAndReverse()));
    }

    private static StampingTheSequenceBenchmarks BuildHarness()
    {
        var harness = new StampingTheSequenceBenchmarks { Repeats = SmallestRepeats };
        harness.Setup();

        return harness;
    }
}
