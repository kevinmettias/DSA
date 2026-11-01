using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarks (ARCHITECTURE 17.9):
// the generic arms check only compares the arms with each other, so this pins the baseline's verdict
// to the one the fixture guarantees. The workload is the adversarial KMP shape - one almost-all-zero
// array and one almost-all-zero group of half its length, each with a single 1 at the very end - so
// the group does match and the answer is true.
public sealed partial class FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedVerdict = true;

    [Fact]
    public void CanChooseByNaiveSubarrayScan_AdversarialKmpShape_FindsTheGroup() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().CanChooseByNaiveSubarrayScan());

    private static FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarks BuildHarness()
    {
        var harness = new FormArrayByConcatenatingSubarraysOfAnotherArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
