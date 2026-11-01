using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindAllGoodStringsBenchmarks (ARCHITECTURE 17.9): the generic arms check only
// compares the arms with each other, so this pins the baseline's count to a value derived
// independently of every arm. Setup builds s1 as all 'a's and s2 as all 'z's at the same length, so
// the bounds span every string of that length: 26^3 candidates, less the 52 that contain the
// forbidden "ab" (26 starting at index 0, 26 starting at index 1, with no overlap possible between
// them).
public sealed partial class FindAllGoodStringsBenchmarksTests
{
    private const int SmallestLength = 3;

    // Setup's bounds span all 26^3 = 17576 three-letter strings. A forbidden two-character substring
    // can start at either of the two positions inside a three-character string - it can never overlap
    // itself - and each start fixes the other character freely, so 2 * 26 = 52 candidates contain it
    // and 17524 do not.
    private const int ExpectedGoodStringCount = 17524;

    [Fact]
    public void EnumerationScan_FullAlphabetBounds_CountsEveryStringWithoutTheForbiddenPair() =>
        Assert.Equal(ExpectedGoodStringCount, BuildHarness().EnumerationScan(SmallestLength));

    private static FindAllGoodStringsBenchmarks BuildHarness()
    {
        var harness = new FindAllGoodStringsBenchmarks();
        harness.Setup();

        return harness;
    }
}
