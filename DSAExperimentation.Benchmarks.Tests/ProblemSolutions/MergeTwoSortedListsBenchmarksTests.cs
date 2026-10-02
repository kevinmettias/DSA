using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeTwoSortedListsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. Setup's value arrays are seeded, so the same length must rebuild the same workload -
// otherwise two published numbers were never comparable in the first place.
public sealed partial class MergeTwoSortedListsBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RecursiveSelection()),
            AnswerText.Of(BuildHarness().RecursiveSelection()));

    [Fact]
    public void RecursiveSelection_AgreesWithDummyHeadSplice()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RecursiveSelection()),
            AnswerText.Of(harness.DummyHeadSplice()));
    }

    private static MergeTwoSortedListsBenchmarks BuildHarness()
    {
        var harness = new MergeTwoSortedListsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
