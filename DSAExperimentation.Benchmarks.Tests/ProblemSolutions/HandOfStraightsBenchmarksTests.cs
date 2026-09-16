using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HandOfStraightsBenchmarks (ARCHITECTURE 17.9): both arms are
// HandOfStraightsSolution's - the BCL Dictionary tally with Array.Sort against this repo's HashMap
// tally with MergeSort - so a harness whose arms disagree is timing two different problems. The
// hand is laid down as whole, non-overlapping runs of exactly GroupSize consecutive values and then
// shuffled, so it always straightens: the verdict is decisive from the fixture alone, and it is
// what forces each strategy through its full group-forming sweep instead of returning early on the
// first missing card. Both arms answer with a bare bool, so agreement witnesses only that the two
// strategies reached the same verdict - an all-straights fixture cannot tell two strategies that
// both answer true from two that agree on something weaker. Setup shuffles off one seed, so the
// same HandCount must rebuild the same hand.
public sealed partial class HandOfStraightsBenchmarksTests
{
    private const int SmallestHandCount = 500;

    // The fixture's runs are complete groups of GroupSize consecutive values, so the hand always
    // straightens.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void Setup_SameHandCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsNStraightHandByBclDictionary(),
            BuildHarness().IsNStraightHandByBclDictionary());

    [Fact]
    public void IsNStraightHandByBclDictionary_WholeConsecutiveRuns_AgreesWithHashMapMergeSort()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedVerdict, harness.IsNStraightHandByBclDictionary());
        Assert.Equal(harness.IsNStraightHandByHashMapMergeSort(), harness.IsNStraightHandByBclDictionary());
    }

    [Fact]
    public void IsNStraightHandByHashMapMergeSort_WholeConsecutiveRuns_AgreesWithBclDictionary()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedVerdict, harness.IsNStraightHandByHashMapMergeSort());
        Assert.Equal(harness.IsNStraightHandByBclDictionary(), harness.IsNStraightHandByHashMapMergeSort());
    }

    private static HandOfStraightsBenchmarks BuildHarness()
    {
        var harness = new HandOfStraightsBenchmarks { HandCount = SmallestHandCount };
        harness.Setup();

        return harness;
    }
}
