using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - one substring search per candidate
// code against one sliding bitmask that marks the codes it has seen - so a harness whose arms
// disagree is timing two different problems. Both arms answer with a bare bool, so agreement
// between them says the two strategies reached the same verdict on the same text.
public sealed partial class CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarksTests
{
    private const int SmallestCodeLength = 8;

    // Setup's text is the fixture's covering text: every code of CodeLength concatenated once, in
    // order. Every one of the 2^CodeLength codes therefore occurs in it by construction - which is
    // exactly why the fixture builds it that way, to deny the substring arm an early "missing code"
    // exit - so a rebuilt text must answer yes, for every code length the harness lists.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void Setup_SameCodeLength_RebuildsTheCoveringText()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(ExpectedVerdict, first.HasAllCodesByCodeSubstringSearch());
        Assert.Equal(ExpectedVerdict, second.HasAllCodesBySlidingBitmask());
    }

    [Fact]
    public void HasAllCodesByCodeSubstringSearch_CoveringTextOfAllCodes_AgreesWithSlidingBitmask()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasAllCodesBySlidingBitmask(), harness.HasAllCodesByCodeSubstringSearch());
    }

    [Fact]
    public void HasAllCodesBySlidingBitmask_CoveringTextOfAllCodes_AgreesWithCodeSubstringSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasAllCodesByCodeSubstringSearch(), harness.HasAllCodesBySlidingBitmask());
    }

    private static CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks BuildHarness()
    {
        var harness = new CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks { CodeLength = SmallestCodeLength };
        harness.Setup();

        return harness;
    }
}
