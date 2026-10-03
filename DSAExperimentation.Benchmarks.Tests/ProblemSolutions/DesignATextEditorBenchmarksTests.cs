using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignATextEditorBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins what every report must be, from the script alone.
//
// Every iteration adds the chunk at the cursor and then walks the cursor back over it, which leaves
// the cursor exactly where that iteration began - the head of the buffer - and LeetCode 2296 reports
// the last ten characters left of the cursor, so on this workload every report is the empty window.
// That is the harness's declared reading, so it is asserted for each of the OperationCount reports.
public sealed partial class DesignATextEditorBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    [Fact]
    public void ListBacked_TwoHundredChunkWalks_ReportsAnEmptyWindowEveryTime() =>
        Assert.Equal(EmptyWindows(), BuildHarness().ListBacked());

    [Fact]
    public void StackBacked_TwoHundredChunkWalks_ReportsAnEmptyWindowEveryTime() =>
        Assert.Equal(EmptyWindows(), BuildHarness().StackBacked());

    private static string[] EmptyWindows() => Enumerable.Repeat(string.Empty, SmallestOperationCount).ToArray();

    private static DesignATextEditorBenchmarks BuildHarness()
    {
        var harness = new DesignATextEditorBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
