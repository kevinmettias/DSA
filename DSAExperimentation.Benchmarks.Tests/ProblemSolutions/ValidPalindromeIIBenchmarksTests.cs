using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidPalindromeIIBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to agreeing with each other, which a shared wrong verdict would pass, so this pins the
// baseline's verdict to the one the workload's construction fixes.
//
// Setup's _text is all 'a' with a 'b' and a 'c' written at mirror-image positions one third of the
// way in from either end. Neither single deletion repairs it: removing the 'b' leaves the 'c'
// opposite an 'a', and removing the 'c' leaves the 'b' opposite an 'a' - in both cases the survivor's
// mirror is an 'a' at an index the other deletion did not touch. LC 680's answer is therefore false.
public sealed partial class ValidPalindromeIIBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's two mismatched characters are mirrored, so no single deletion can pair them off.
    private const bool ExpectedIsValid = false;

    [Fact]
    public void TryEachSingleDeletion_MirroredMismatches_RejectsTheText() =>
        Assert.Equal(ExpectedIsValid, BuildHarness().TryEachSingleDeletion());

    private static ValidPalindromeIIBenchmarks BuildHarness()
    {
        var harness = new ValidPalindromeIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
