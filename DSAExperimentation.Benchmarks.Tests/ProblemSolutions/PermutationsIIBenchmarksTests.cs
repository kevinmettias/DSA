using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PermutationsIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - every distinct ordering of the duplicate-bearing input - so a
// harness whose arms disagree is timing two different problems. The class carries no [Params] and
// no [GlobalSetup]: the input is a fixed literal (three duplicate pairs), so there is nothing to
// rebuild and each arm is handed the same operand list directly.
public sealed partial class PermutationsIIBenchmarksTests
{
    [Fact]
    public void SpecializedRecursive_AgreesWithBacktracking()
    {
        var harness = new PermutationsIIBenchmarks();

        Assert.Equal(
            AnswerText.Of(harness.Backtracking()),
            AnswerText.Of(harness.SpecializedRecursive()));
    }

    [Fact]
    public void Backtracking_AgreesWithSpecializedRecursive()
    {
        var harness = new PermutationsIIBenchmarks();

        Assert.Equal(
            AnswerText.Of(harness.SpecializedRecursive()),
            AnswerText.Of(harness.Backtracking()));
    }
}
