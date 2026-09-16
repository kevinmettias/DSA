using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidPalindromeIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// ValidPalindromeIISolution's competing strategies for the same question - trying every single
// deletion from both ends against one scan that skips only the mismatch it finds - so a harness whose
// arms disagree is answering two different questions about LC 680.
//
// Setup's _text is all 'a' with a 'b' and a 'c' written at mirror-image positions one third of the
// way in from either end. Neither single deletion repairs it: removing the 'b' leaves the 'c'
// opposite an 'a', and removing the 'c' leaves the 'b' opposite an 'a' - in both cases the survivor's
// mirror is an 'a' at an index the other deletion did not touch. LC 680's answer is therefore false,
// and that decisive literal is asserted alongside the arms' agreement so agreement cannot hold on a
// shared wrong verdict.
public sealed partial class ValidPalindromeIIBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's two mismatched characters are mirrored, so no single deletion can pair them off.
    private const bool ExpectedIsValid = false;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            BuildHarness().IsValidPalindromeByMismatchSkip(),
            BuildHarness().IsValidPalindromeByMismatchSkip());
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidPalindromeByMismatchSkip());
    }

    [Fact]
    public void TryEachSingleDeletion_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsValidPalindromeByMismatchSkip(), harness.TryEachSingleDeletion());
        Assert.Equal(ExpectedIsValid, harness.TryEachSingleDeletion());
    }

    [Fact]
    public void IsValidPalindromeByMismatchSkip_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TryEachSingleDeletion(), harness.IsValidPalindromeByMismatchSkip());
        Assert.Equal(ExpectedIsValid, harness.IsValidPalindromeByMismatchSkip());
    }

    private static ValidPalindromeIIBenchmarks BuildHarness()
    {
        var harness = new ValidPalindromeIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
