using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestUploadedPrefixBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning the upload flags from video 1 on every
// query against the set behind a frontier that only advances - so a harness whose arms disagree
// is timing two different problems. Each arm constructs its own server inside the call, so one
// harness is safe to call twice in either order and the single-harness rule holds. Both arms
// return the last reported prefix length, a scalar compared directly. Setup shuffles 1..VideoCount
// from a fixed seed and uploads all of them, so the final answer is every video: that count is the
// decisive value both arms must reach, and the same VideoCount must rebuild the same order.
public sealed partial class LongestUploadedPrefixBenchmarksTests
{
    private const int SmallestVideoCount = 200;

    // The shuffled order is a permutation of 1..VideoCount and the replay uploads all of it, so
    // the prefix eventually covers every video.
    private const int ExpectedLongestUploadedPrefix = SmallestVideoCount;

    [Fact]
    public void Setup_SmallestVideoCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedLongestUploadedPrefix, BuildHarness().SetFrontierAdvance());
        Assert.Equal(BuildHarness().RescanArray(), BuildHarness().RescanArray());
    }

    [Fact]
    public void RescanArray_SmallestVideoCount_AgreesWithSetFrontierAdvance()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestUploadedPrefix, harness.RescanArray());
        Assert.Equal(harness.SetFrontierAdvance(), harness.RescanArray());
    }

    [Fact]
    public void SetFrontierAdvance_SmallestVideoCount_AgreesWithRescanArray()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestUploadedPrefix, harness.SetFrontierAdvance());
        Assert.Equal(harness.RescanArray(), harness.SetFrontierAdvance());
    }

    private static LongestUploadedPrefixBenchmarks BuildHarness()
    {
        var harness = new LongestUploadedPrefixBenchmarks { VideoCount = SmallestVideoCount };
        harness.Setup();

        return harness;
    }
}
