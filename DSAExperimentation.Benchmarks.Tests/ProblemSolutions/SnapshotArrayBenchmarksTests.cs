using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SnapshotArrayBenchmarks (ARCHITECTURE 17.9): both arms are
// SnapshotArraySolution's stateful ISnapshotArray, seeded identically by Setup, so a harness
// whose arms disagree is timing two different problems. Setup is a pure function of Length, so
// the same Length must rebuild the same seeded array and the same query id.
//
// Get only reads, so one harness is safe to call twice in either order. The seeding is
// arithmetic: Setup writes i * ValueScaleFactor to the single index before each of the Length
// snaps, so the history holds snap ids 0 .. Length - 1, and the query at Length + 1 must floor to
// the last of them. Both arms are asserted against that value and not only against each other,
// so agreement cannot be two arms reporting the same wrong floor.
public sealed partial class SnapshotArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the benchmark's own step between two consecutive written values.
    private const int ValueScaleFactor = 2;

    // The last snap id the seeding loop writes to, so the floor query lands on it.
    private const int LastSeededSnapId = SmallestLength - 1;

    private const int ExpectedFlooredValue = LastSeededSnapId * ValueScaleFactor;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearFloorScan(), BuildHarness().LinearFloorScan());

    [Fact]
    public void LinearFloorScan_QueryPastTheLastSnap_FloorsToTheLastWrittenValue()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedFlooredValue, harness.LinearFloorScan());
        Assert.Equal(harness.BinarySearchFloor(), harness.LinearFloorScan());
    }

    [Fact]
    public void BinarySearchFloor_QueryPastTheLastSnap_FloorsToTheLastWrittenValue()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedFlooredValue, harness.BinarySearchFloor());
        Assert.Equal(harness.LinearFloorScan(), harness.BinarySearchFloor());
    }

    private static SnapshotArrayBenchmarks BuildHarness()
    {
        var harness = new SnapshotArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
