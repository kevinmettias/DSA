using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReplaceNonCoprimeNumbersInArrayBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a full front-to-back rescan restarted after
// every merge against a single left-to-right sweep whose stack top is the only element a new value
// can merge with - so a harness whose arms disagree is timing two different problems. Setup draws
// the values from one fixed seed, so the same Length must rebuild the same array; otherwise two
// published numbers were never comparable in the first place.
//
// Both arms build their own working copy and never write into the hoisted array, so one harness is
// safe to call twice in either order and the single-harness rule holds.
public sealed partial class ReplaceNonCoprimeNumbersInArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RepeatedFullRescan()),
            AnswerText.Of(BuildHarness().RepeatedFullRescan()));

    [Fact]
    public void RepeatedFullRescan_SeededValues_AgreesWithStackCascadingMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.StackCascadingMerge()),
            AnswerText.Of(harness.RepeatedFullRescan()));
    }

    [Fact]
    public void StackCascadingMerge_SeededValues_AgreesWithRepeatedFullRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RepeatedFullRescan()),
            AnswerText.Of(harness.StackCascadingMerge()));
    }

    private static ReplaceNonCoprimeNumbersInArrayBenchmarks BuildHarness()
    {
        var harness = new ReplaceNonCoprimeNumbersInArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
