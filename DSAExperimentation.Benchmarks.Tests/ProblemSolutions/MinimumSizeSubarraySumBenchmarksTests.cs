using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumSizeSubarraySumBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the shortest subarray whose sum reaches the target - so a harness
// whose arms disagree is timing two different problems. The target is fixed one above the array's own
// total, which is what forces both arms through their worst case rather than letting either return on
// the first window it finds; it also means both arms answer "no such subarray" here, so this agreement
// is weak by construction: it catches an arm that claims a window the scan cannot find, but not an arm
// whose window arithmetic is wrong in a way that still finds nothing. Setup draws the array from one
// fixed seed, so the same Length must rebuild the same array and target.
public sealed partial class MinimumSizeSubarraySumBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BinarySearchPrefixSum_SameArrayAndTarget_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.BinarySearchPrefixSum());
    }

    [Fact]
    public void BruteForce_SameArrayAndTarget_AgreesWithBinarySearchPrefixSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchPrefixSum(), harness.BruteForce());
    }

    private static MinimumSizeSubarraySumBenchmarks BuildHarness()
    {
        var harness = new MinimumSizeSubarraySumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
