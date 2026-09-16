using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindAllGoodStringsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - enumerating every candidate and substring-checking it with the
// BCL against the KMP-automaton digit DP - so a harness whose arms disagree is counting two
// different languages. Both arms return the count, a plain int. Setup builds s1 as all 'a's and s2
// as all 'z's at the same length, so the bounds span every string of that length and the count is
// decisive rather than merely agreed: 26^3 candidates, less the 52 that contain the forbidden "ab"
// (26 starting at index 0, 26 starting at index 1, with no overlap possible between them). The same
// Length must rebuild the same bounds and the same forbidden substring.
public sealed partial class FindAllGoodStringsBenchmarksTests
{
    private const int SmallestLength = 3;

    // Setup's bounds span all 26^3 = 17576 three-letter strings. A forbidden two-character substring
    // can start at either of the two positions inside a three-character string - it can never overlap
    // itself - and each start fixes the other character freely, so 2 * 26 = 52 candidates contain it
    // and 17524 do not.
    private const int ExpectedGoodStringCount = 17524;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameBounds()
    {
        Assert.Equal(ExpectedGoodStringCount, BuildHarness().EnumerationScan());

        Assert.Equal(BuildHarness().EnumerationScan(), BuildHarness().EnumerationScan());
    }

    [Fact]
    public void EnumerationScan_FullAlphabetBounds_AgreesWithAutomatonDigitDp()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedGoodStringCount, harness.EnumerationScan());

        Assert.Equal(harness.AutomatonDigitDp(), harness.EnumerationScan());
    }

    [Fact]
    public void AutomatonDigitDp_FullAlphabetBounds_AgreesWithEnumerationScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedGoodStringCount, harness.AutomatonDigitDp());

        Assert.Equal(harness.EnumerationScan(), harness.AutomatonDigitDp());
    }

    private static FindAllGoodStringsBenchmarks BuildHarness()
    {
        var harness = new FindAllGoodStringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
