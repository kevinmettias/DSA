using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PermutationsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - every ordering of the numbers 1..Length - so a harness whose arms
// disagree is timing two different problems. Length is the only [Params] axis and Setup derives the
// operand list from it alone, so the same Length must rebuild the same list before either arm runs.
public sealed partial class PermutationsBenchmarksTests
{
    private const int SmallestLength = 6;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().SpecializedRecursive()),
            AnswerText.Of(BuildHarness().SpecializedRecursive()));

    [Fact]
    public void SpecializedRecursive_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.Backtracking()),
            AnswerText.Of(harness.SpecializedRecursive()));
    }

    [Fact]
    public void Backtracking_AgreesWithSpecializedRecursive()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SpecializedRecursive()),
            AnswerText.Of(harness.Backtracking()));
    }

    private static PermutationsBenchmarks BuildHarness()
    {
        var harness = new PermutationsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
