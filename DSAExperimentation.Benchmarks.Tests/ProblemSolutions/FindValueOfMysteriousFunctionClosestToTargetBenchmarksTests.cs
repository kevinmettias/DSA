using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindValueOfMysteriousFunctionClosestToTargetBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question, so a harness whose arms disagree is
// timing two different problems. Setup draws the values from a fixed seed and both arms take the
// same fixed target, so the same Length must rebuild the same workload.
public sealed partial class FindValueOfMysteriousFunctionClosestToTargetBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceAllSubarrays()),
            AnswerText.Of(BuildHarness().BruteForceAllSubarrays()));

    [Fact]
    public void BruteForceAllSubarrays_AgreesWithDistinctAndValuesHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DistinctAndValuesHashMap(), harness.BruteForceAllSubarrays());
    }

    [Fact]
    public void DistinctAndValuesHashMap_AgreesWithBruteForceAllSubarrays()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceAllSubarrays(), harness.DistinctAndValuesHashMap());
    }

    private static FindValueOfMysteriousFunctionClosestToTargetBenchmarks BuildHarness()
    {
        var harness = new FindValueOfMysteriousFunctionClosestToTargetBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
