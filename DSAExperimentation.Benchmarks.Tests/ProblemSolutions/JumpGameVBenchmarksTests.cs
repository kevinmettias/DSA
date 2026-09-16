using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameVBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the longest indices-visited run under LC 1340's jump rules - so a
// harness whose arms disagree is timing two different problems. [GlobalSetup] builds nothing but
// a seeded shuffle of 0..Length-1 with no ties, and both arms take that array directly; neither
// mutates it or holds state across calls, so one harness instance is safe to call twice in
// either order.
public sealed partial class JumpGameVBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameShuffledArray() =>
        Assert.Equal(
            BuildHarness().MemoizedDfsPerStart(),
            BuildHarness().MemoizedDfsPerStart());

    [Fact]
    public void MemoizedDfsPerStart_SeededShuffle_AgreesWithTopologicalSortLongestPath()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TopologicalSortLongestPath(), harness.MemoizedDfsPerStart());
    }

    [Fact]
    public void TopologicalSortLongestPath_SeededShuffle_AgreesWithMemoizedDfsPerStart()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedDfsPerStart(), harness.TopologicalSortLongestPath());
    }

    private static JumpGameVBenchmarks BuildHarness()
    {
        var harness = new JumpGameVBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
