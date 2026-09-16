using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContainsDuplicateIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a windowed pairwise brute force against the index map - so a
// harness whose arms disagree is timing two different problems. Setup draws the array from one fixed
// seed, so the same Length must rebuild the same permutation; otherwise two published numbers were
// never comparable in the first place.
//
// The array is private, but its documented shape is decisive through either arm: it is a permutation
// of distinct integers, so no value repeats anywhere and certainly not within the window, and both
// strategies report false - the full worst-case scan the class comment says the workload exists to
// force, with no early exit to make the brute force look competitive.
public sealed partial class ContainsDuplicateIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePermutation()
    {
        Assert.False(BuildHarness().HasNearbyDuplicateByBruteForce());
        Assert.Equal(BuildHarness().HasNearbyDuplicateByHashMap(), BuildHarness().HasNearbyDuplicateByHashMap());
    }

    [Fact]
    public void HasNearbyDuplicateByBruteForce_DistinctPermutation_AgreesWithHasNearbyDuplicateByHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasNearbyDuplicateByHashMap(), harness.HasNearbyDuplicateByBruteForce());
    }

    [Fact]
    public void HasNearbyDuplicateByHashMap_DistinctPermutation_AgreesWithHasNearbyDuplicateByBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasNearbyDuplicateByBruteForce(), harness.HasNearbyDuplicateByHashMap());
    }

    private static ContainsDuplicateIIBenchmarks BuildHarness()
    {
        var harness = new ContainsDuplicateIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
