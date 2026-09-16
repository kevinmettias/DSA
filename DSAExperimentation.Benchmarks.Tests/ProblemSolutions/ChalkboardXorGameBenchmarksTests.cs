using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ChalkboardXorGameBenchmarks (ARCHITECTURE 17.9): its three arms are
// competing strategies for the same question - the unmemoized game-tree recursion, the same
// recursion remembered in the Memoizer, and the closed form the whole search reduces to - so a
// harness whose arms disagree is timing two different problems. Each arm answers with a bare bool,
// so each arm's test pins its verdict against both of the others rather than against one.
//
// The board Setup draws is private and every arm collapses it to a single bit, so the strongest
// reading a rebuild can be pinned to is that a second harness built from the same Length reaches
// the same three verdicts - which is what the one seeded draw the harness makes buys.
public sealed partial class ChalkboardXorGameBenchmarksTests
{
    private const int SmallestLength = 9;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameNineNumberBoard()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(
            (
                first.CanAliceWinByBruteForceRecursion(),
                first.CanAliceWinByMemoizedRecursion(),
                first.CanAliceWinByXorParityFormula()),
            (
                second.CanAliceWinByBruteForceRecursion(),
                second.CanAliceWinByMemoizedRecursion(),
                second.CanAliceWinByXorParityFormula()));
    }

    [Fact]
    public void CanAliceWinByBruteForceRecursion_SeededNineNumberBoard_AgreesWithBothOtherArms()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanAliceWinByMemoizedRecursion(), harness.CanAliceWinByBruteForceRecursion());
        Assert.Equal(harness.CanAliceWinByXorParityFormula(), harness.CanAliceWinByBruteForceRecursion());
    }

    [Fact]
    public void CanAliceWinByMemoizedRecursion_SeededNineNumberBoard_AgreesWithBothOtherArms()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanAliceWinByBruteForceRecursion(), harness.CanAliceWinByMemoizedRecursion());
        Assert.Equal(harness.CanAliceWinByXorParityFormula(), harness.CanAliceWinByMemoizedRecursion());
    }

    [Fact]
    public void CanAliceWinByXorParityFormula_SeededNineNumberBoard_AgreesWithBothRecursionArms()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanAliceWinByBruteForceRecursion(), harness.CanAliceWinByXorParityFormula());
        Assert.Equal(harness.CanAliceWinByMemoizedRecursion(), harness.CanAliceWinByXorParityFormula());
    }

    private static ChalkboardXorGameBenchmarks BuildHarness()
    {
        var harness = new ChalkboardXorGameBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
