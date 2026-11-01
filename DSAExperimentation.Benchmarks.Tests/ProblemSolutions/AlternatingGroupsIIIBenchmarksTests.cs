using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AlternatingGroupsIIIBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to
// each other on Setup's seeded script of intermixed repaints and size queries, and this asserts the script asks
// at least one size query - an empty answer would make that agreement vacuous.
public sealed partial class AlternatingGroupsIIIBenchmarksTests
{
    // The smaller of Setup's [Params(1_000, 5_000)] circle lengths.
    private const int SmallestLength = 1_000;

    [Fact]
    public void BruteForce_ThousandTileCircle_AnswersAtLeastOneSizeQuery() =>
        Assert.NotEmpty(BuildHarness().BruteForce());

    private static AlternatingGroupsIIIBenchmarks BuildHarness()
    {
        var harness = new AlternatingGroupsIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
