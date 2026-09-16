using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PoorPigsBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for one question - the fewest pigs that can identify the poisoned bucket - so a harness whose arms
// disagree is timing two different problems. Buckets is the only [Params] axis and the minutes pair
// is fixed, so each axis value is its own harness.
public sealed partial class PoorPigsBenchmarksTests
{
    private const int SmallestBuckets = 100;

    [Fact]
    public void LinearRecompute_AgreesWithBinarySearchOverPowers()
    {
        var harness = Harness(SmallestBuckets);

        Assert.Equal(harness.BinarySearchOverPowers(), harness.LinearRecompute());
    }

    [Fact]
    public void BinarySearchOverPowers_AgreesWithLinearRecompute()
    {
        var harness = Harness(SmallestBuckets);

        Assert.Equal(harness.LinearRecompute(), harness.BinarySearchOverPowers());
    }

    private static PoorPigsBenchmarks Harness(int buckets) => new() { Buckets = buckets };
}
