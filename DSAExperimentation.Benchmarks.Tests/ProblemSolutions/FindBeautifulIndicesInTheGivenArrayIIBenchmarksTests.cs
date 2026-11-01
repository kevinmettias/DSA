using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindBeautifulIndicesInTheGivenArrayIIBenchmarks (ARCHITECTURE 17.9): the
// generic arms check only compares the arms with each other, and arms that all answer empty agree
// as readily when they gave up as when they searched, so the emptiness is asserted against the
// fixture's construction directly. Setup is #3008's own version of the worst-case construction: the
// haystack is 'a' alone and both patterns are that character repeated with a mismatched final one,
// which makes every naive scan attempt run almost the whole pattern before failing.
public sealed partial class FindBeautifulIndicesInTheGivenArrayIIBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup's patterns each end in a character the haystack never contains, so neither occurs and a
    // correct search has no index to report.
    [Fact]
    public void BruteForce_NoOccurrenceWorstCase_ReportsNoIndex() =>
        Assert.Empty(BuildHarness().BruteForce());

    private static FindBeautifulIndicesInTheGivenArrayIIBenchmarks BuildHarness()
    {
        var harness = new FindBeautifulIndicesInTheGivenArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
