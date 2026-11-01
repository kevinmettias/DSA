using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for XOfAKindInADeckOfCardsBenchmarks (ARCHITECTURE 17.9): the generic check holds
// the arms to agreeing with each other. The deck is built partitionable into groups of the declared
// size, which is the decisive value the answer must be - a pair of arms that both said "no groups"
// would otherwise agree perfectly while timing a question the workload never asked.
public sealed partial class XOfAKindInADeckOfCardsBenchmarksTests
{
    private const int SmallestDeckSize = 400;

    [Fact]
    public void HasGroupsSizeXByDictionaryCount_PartitionableDeck_ReportsGroupsExist() =>
        Assert.True(BuildHarness().HasGroupsSizeXByDictionaryCount());

    private static XOfAKindInADeckOfCardsBenchmarks BuildHarness()
    {
        var harness = new XOfAKindInADeckOfCardsBenchmarks { DeckSize = SmallestDeckSize };
        harness.Setup();

        return harness;
    }
}
