using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthLargestElementInAStreamBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - the running kth-largest value LC 703 reports - so a
// harness whose arms disagree is timing two different problems. The fixture builds one fixed
// interleaved stream and each arm replays it against a stream instance it creates itself inside
// the call, so one harness instance is safe to call twice in either order.
//
// Each arm answers with the LAST kth-largest value the replay produced, which is the one order
// statistic the problem asks for; agreement witnesses that both strategies held the same running
// kth-largest at the end of the same stream, not that they ever held the same k values.
public sealed partial class KthLargestElementInAStreamBenchmarksTests
{
    private const int SmallestStreamLength = 100;

    [Fact]
    public void Setup_SameStreamLength_RebuildsTheSameStream() =>
        Assert.Equal(
            BuildHarness().SortOnEveryAdd(),
            BuildHarness().SortOnEveryAdd());

    [Fact]
    public void SortOnEveryAdd_SeededStream_AgreesWithSizeKMinHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SizeKMinHeap(), harness.SortOnEveryAdd());
    }

    [Fact]
    public void SizeKMinHeap_SeededStream_AgreesWithSortOnEveryAdd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortOnEveryAdd(), harness.SizeKMinHeap());
    }

    private static KthLargestElementInAStreamBenchmarks BuildHarness()
    {
        var harness = new KthLargestElementInAStreamBenchmarks { StreamLength = SmallestStreamLength };
        harness.Setup();

        return harness;
    }
}
