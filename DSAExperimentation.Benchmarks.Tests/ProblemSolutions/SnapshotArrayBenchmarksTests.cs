using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SnapshotArrayBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the answer the workload is built to have. The seeding is
// arithmetic: Setup writes i * ValueScaleFactor to the single index before each of the Length
// snaps, so the history holds snap ids 0 .. Length - 1, and the query at the last of them must
// floor to the value written just before it. The baseline is asserted against that value, so
// agreement cannot be two arms reporting the same wrong floor.
public sealed partial class SnapshotArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the benchmark's own step between two consecutive written values.
    private const int ValueScaleFactor = 2;

    // The last snap id the seeding loop writes to, which is the id the floor query asks for.
    private const int LastSeededSnapId = SmallestLength - 1;

    private const int ExpectedFlooredValue = LastSeededSnapId * ValueScaleFactor;

    [Fact]
    public void LinearFloorScan_QueryAtTheLastSnap_FloorsToTheLastWrittenValue() =>
        Assert.Equal(ExpectedFlooredValue, BuildHarness().LinearFloorScan());

    private static SnapshotArrayBenchmarks BuildHarness()
    {
        var harness = new SnapshotArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
