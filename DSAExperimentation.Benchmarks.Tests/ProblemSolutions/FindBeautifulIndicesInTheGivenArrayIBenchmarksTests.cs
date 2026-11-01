using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindBeautifulIndicesInTheGivenArrayIBenchmarks (ARCHITECTURE 17.9): the
// generic arms check only compares the arms with each other, and arms that all answer empty agree
// as readily when they gave up as when they searched, so the emptiness is asserted against the
// fixture's construction directly. Setup builds the searched text from 'a' alone and both patterns
// as that character repeated with a mismatched final one, which is the documented worst case: the
// patterns never occur, so every naive start position is compared almost the whole pattern through
// before failing.
public sealed partial class FindBeautifulIndicesInTheGivenArrayIBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup's patterns each end in a character the searched text never contains, so neither occurs
    // and a correct search has no index to report.
    [Fact]
    public void BruteForce_NoOccurrenceWorstCase_ReportsNoIndex() =>
        Assert.Empty(BuildHarness().BruteForce());

    private static FindBeautifulIndicesInTheGivenArrayIBenchmarks BuildHarness()
    {
        var harness = new FindBeautifulIndicesInTheGivenArrayIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
