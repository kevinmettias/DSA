using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumGameBenchmarks (ARCHITECTURE 17.9): the state [GlobalSetup] builds is
// decisive, not merely agreed: both halves carry the same number of blanks and a known-digit
// difference of zero, so the levelling margin the closed form names (9 per pair of blanks Bob is
// owed) is exactly that difference and Bob holds the halves level - Alice does not win. The
// baseline is pinned to that false, because three arms that were wrong in the same way would still
// pass the generic arm check's agreement.
public sealed partial class SumGameBenchmarksTests
{
    private const int SmallestBlanksPerSide = 2;

    [Fact]
    public void CanAliceWinByClosedForm_EqualBlankBoard_AliceDoesNotWin() =>
        Assert.False(BuildHarness().CanAliceWinByClosedForm(SmallestBlanksPerSide));

    private static SumGameBenchmarks BuildHarness()
    {
        var harness = new SumGameBenchmarks();
        harness.Setup();

        return harness;
    }
}
