using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question, so a harness whose arms disagree is
// timing two different problems. Setup derives both arrays from Length alone, so the same Length must
// rebuild the same workload.
//
// The agreement is weak by construction and the assertions say only what the fixture supports: the
// workload is the adversarial KMP shape - one almost-all-zero array and one almost-all-zero group of
// half its length, each with a single 1 at the very end - so the group does match and every call
// answers true. Agreement therefore witnesses "both arms found the match the fixture guarantees", not
// that they agree about a rejected input; the true-verdict assertions are what stop the two arms
// agreeing on a verdict neither of them computed.
public sealed partial class FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedVerdict = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CanChooseByNaiveSubarrayScan()),
            AnswerText.Of(BuildHarness().CanChooseByNaiveSubarrayScan()));

    [Fact]
    public void CanChooseByNaiveSubarrayScan_AgreesWithCanChooseByCharCompressedKmpSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedVerdict, harness.CanChooseByNaiveSubarrayScan());
        Assert.Equal(harness.CanChooseByCharCompressedKmpSearch(), harness.CanChooseByNaiveSubarrayScan());
    }

    [Fact]
    public void CanChooseByCharCompressedKmpSearch_AgreesWithCanChooseByNaiveSubarrayScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedVerdict, harness.CanChooseByCharCompressedKmpSearch());
        Assert.Equal(harness.CanChooseByNaiveSubarrayScan(), harness.CanChooseByCharCompressedKmpSearch());
    }

    private static FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarks BuildHarness()
    {
        var harness = new FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
