using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignATextEditorBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a List<char> buffer editing at the cursor against the classic
// two-stack cursor design over this repo's Stack<T> - so a harness whose arms disagree is timing two
// different problems. The class has no [GlobalSetup]: the whole workload is OperationCount paired with a
// fixed five-character chunk, so the smallest OperationCount is 200, and each arm builds its own editor
// inside the call. Every iteration adds the chunk at the cursor and then walks the cursor back over it,
// which leaves the cursor exactly where that iteration began - the head of the buffer - and LeetCode 2296
// reports the last ten characters left of the cursor, so on this workload both arms report the empty
// window. That reported window is the harness's declared reading, so it is asserted alongside the
// agreement rather than left implicit.
public sealed partial class DesignATextEditorBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    private const string ExpectedReportedWindow = "";

    [Fact]
    public void ListBacked_TwoHundredChunkWalks_AgreesWithStackBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedReportedWindow, harness.ListBacked());
        Assert.Equal(harness.StackBacked(), harness.ListBacked());
    }

    [Fact]
    public void StackBacked_TwoHundredChunkWalks_AgreesWithListBacked()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedReportedWindow, harness.StackBacked());
        Assert.Equal(harness.ListBacked(), harness.StackBacked());
    }

    private static DesignATextEditorBenchmarks BuildHarness() =>
        new() { OperationCount = SmallestOperationCount };
}
