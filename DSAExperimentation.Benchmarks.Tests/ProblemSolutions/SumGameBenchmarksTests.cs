using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumGameBenchmarks (ARCHITECTURE 17.9): its three arms are competing strategies
// for the same question - the unmemoized minimax, the same recursion behind this repo's Memoizer, and
// the closed form the recursion collapses to - so a harness whose arms disagree is timing three
// different games. Each arm answers with a bare bool, so each arm's test pins its verdict against
// both of the others.
//
// The state [GlobalSetup] builds is also decisive, not merely agreed: both halves carry the same
// number of blanks and a known-digit difference of zero, so the levelling margin the closed form names
// (9 per pair of blanks Bob is owed) is exactly that difference and Bob holds the halves level - Alice
// does not win. Each arm is pinned to that false next to the agreement, because three arms that were
// wrong in the same way would still agree.
public sealed partial class SumGameBenchmarksTests
{
    private const int SmallestBlanksPerSide = 2;

    [Fact]
    public void Setup_SameBlanksPerSide_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(
            (
                first.CanAliceWinByBruteForceRecursion(SmallestBlanksPerSide),
                first.CanAliceWinByMemoizedRecursion(SmallestBlanksPerSide),
                first.CanAliceWinByClosedForm(SmallestBlanksPerSide)),
            (
                second.CanAliceWinByBruteForceRecursion(SmallestBlanksPerSide),
                second.CanAliceWinByMemoizedRecursion(SmallestBlanksPerSide),
                second.CanAliceWinByClosedForm(SmallestBlanksPerSide)));
    }

    [Fact]
    public void CanAliceWinByBruteForceRecursion_EqualBlankBoard_AgreesWithBothOtherArms()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanAliceWinByMemoizedRecursion(SmallestBlanksPerSide),
            harness.CanAliceWinByBruteForceRecursion(SmallestBlanksPerSide));
        Assert.Equal(
            harness.CanAliceWinByClosedForm(SmallestBlanksPerSide),
            harness.CanAliceWinByBruteForceRecursion(SmallestBlanksPerSide));
        Assert.False(harness.CanAliceWinByBruteForceRecursion(SmallestBlanksPerSide));
    }

    [Fact]
    public void CanAliceWinByMemoizedRecursion_EqualBlankBoard_AgreesWithBothOtherArms()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanAliceWinByBruteForceRecursion(SmallestBlanksPerSide),
            harness.CanAliceWinByMemoizedRecursion(SmallestBlanksPerSide));
        Assert.Equal(
            harness.CanAliceWinByClosedForm(SmallestBlanksPerSide),
            harness.CanAliceWinByMemoizedRecursion(SmallestBlanksPerSide));
        Assert.False(harness.CanAliceWinByMemoizedRecursion(SmallestBlanksPerSide));
    }

    [Fact]
    public void CanAliceWinByClosedForm_EqualBlankBoard_AgreesWithBothOtherArms()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanAliceWinByBruteForceRecursion(SmallestBlanksPerSide),
            harness.CanAliceWinByClosedForm(SmallestBlanksPerSide));
        Assert.Equal(
            harness.CanAliceWinByMemoizedRecursion(SmallestBlanksPerSide),
            harness.CanAliceWinByClosedForm(SmallestBlanksPerSide));
        Assert.False(harness.CanAliceWinByClosedForm(SmallestBlanksPerSide));
    }

    private static SumGameBenchmarks BuildHarness()
    {
        var harness = new SumGameBenchmarks();
        harness.Setup();

        return harness;
    }
}
