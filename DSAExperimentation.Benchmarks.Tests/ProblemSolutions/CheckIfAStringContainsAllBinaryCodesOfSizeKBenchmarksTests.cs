using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks (ARCHITECTURE 17.9):
// the generic check holds the arms to each other, but both answer with a bare bool, so agreement
// says only that the two strategies reached the same verdict on the same text. This pins the
// baseline's verdict to the one the covering text's construction decides without either arm.
public sealed partial class CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarksTests
{
    private const int SmallestCodeLength = 8;

    // Setup's text is the fixture's covering text: every code of CodeLength concatenated once, in
    // order. Every one of the 2^CodeLength codes therefore occurs in it by construction - which is
    // exactly why the fixture builds it that way, to deny the substring arm an early "missing code"
    // exit - so the text must answer yes, for every code length the harness lists.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void HasAllCodesByCodeSubstringSearch_CoveringTextOfAllCodes_ReturnsTrue() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().HasAllCodesByCodeSubstringSearch(SmallestCodeLength));

    private static CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks BuildHarness()
    {
        var harness = new CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks();
        harness.Setup();

        return harness;
    }
}
