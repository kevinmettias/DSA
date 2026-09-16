using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for XOfAKindInADeckOfCardsBenchmarks (ARCHITECTURE 17.9): both arms are competing
// tallies for one question over one deck, so a harness whose arms disagree is timing two different
// problems. The deck is built partitionable into groups of the declared size, which is the decisive
// value the answer must be - a pair of arms that both said "no groups" would otherwise agree
// perfectly while timing a question the workload never asked.
public sealed partial class XOfAKindInADeckOfCardsBenchmarksTests
{
    private const int SmallestDeckSize = 400;

    [Fact]
    public void Setup_SameDeckSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().HasGroupsSizeXByDictionaryCount()),
            AnswerText.Of(BuildHarness().HasGroupsSizeXByDictionaryCount()));

    [Fact]
    public void HasGroupsSizeXByDictionaryCount_AgreesWithHashMapCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasGroupsSizeXByDictionaryCount(), harness.HasGroupsSizeXByHashMapCount());
    }

    [Fact]
    public void HasGroupsSizeXByHashMapCount_AgreesWithDictionaryCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasGroupsSizeXByHashMapCount(), harness.HasGroupsSizeXByDictionaryCount());
    }

    [Fact]
    public void HasGroupsSizeXByDictionaryCount_PartitionableDeck_ReportsGroupsExist() =>
        Assert.True(BuildHarness().HasGroupsSizeXByDictionaryCount());

    [Fact]
    public void HasGroupsSizeXByHashMapCount_PartitionableDeck_ReportsGroupsExist() =>
        Assert.True(BuildHarness().HasGroupsSizeXByHashMapCount());

    private static XOfAKindInADeckOfCardsBenchmarks BuildHarness()
    {
        var harness = new XOfAKindInADeckOfCardsBenchmarks { DeckSize = SmallestDeckSize };
        harness.Setup();

        return harness;
    }
}
