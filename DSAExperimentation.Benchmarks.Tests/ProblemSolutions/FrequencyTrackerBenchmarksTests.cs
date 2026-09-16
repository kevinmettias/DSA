using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FrequencyTrackerBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different problems.
// Both arms construct their own tracker inside the call and replay the same fixed script, so one
// harness instance is safe to call twice in either order. Setup draws the script from a fixed seed, so
// the same Length must rebuild the same workload.
//
// The agreement is weak by construction and the assertions say only what the arms expose: both arms
// return a single int - how many of the scripted hasFrequency queries answered true - so a
// disagreement about *which* queries are true, with the same true count, would go unnoticed. What is
// compared is the aggregate the arms actually return, not the per-query answers.
public sealed partial class FrequencyTrackerBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().SortedScanList()),
            AnswerText.Of(BuildHarness().SortedScanList()));

    [Fact]
    public void SortedScanList_AgreesWithPairedHashMaps()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairedHashMaps(), harness.SortedScanList());
    }

    [Fact]
    public void PairedHashMaps_AgreesWithSortedScanList()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortedScanList(), harness.PairedHashMaps());
    }

    private static FrequencyTrackerBenchmarks BuildHarness()
    {
        var harness = new FrequencyTrackerBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
