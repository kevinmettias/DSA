using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheNumberOfPossibleWaysForAnEventBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - enumerating every score assignment against a
// memo over (performer, score) stage partitions - so a harness whose arms disagree is timing two
// different problems. The class has no Setup: Stages and MaxScore are fixed fields of the problem,
// so Performers is the only axis, and both arms must count the same ways at the same bound.
public sealed partial class FindTheNumberOfPossibleWaysForAnEventBenchmarksTests
{
    private const int SmallestPerformerCount = 6;

    [Fact]
    public void BruteForceEnumeration_SmallestPerformerCount_AgreesWithStagePartitionMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StagePartitionMemo(), harness.BruteForceEnumeration());
    }

    [Fact]
    public void StagePartitionMemo_SmallestPerformerCount_AgreesWithBruteForceEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceEnumeration(), harness.StagePartitionMemo());
    }

    private static FindTheNumberOfPossibleWaysForAnEventBenchmarks BuildHarness() =>
        new() { Performers = SmallestPerformerCount };
}
