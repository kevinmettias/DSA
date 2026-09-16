using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TimeBasedKeyValueStoreBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the linear scan back through one key's timestamps
// against the binary search for the floor - so a harness whose arms disagree is timing two
// different problems. Both arms return the value stored at the floor timestamp as a string, so
// they are compared directly. Setup seeds each store with the same Length strictly increasing
// timestamps, so the same Length must rebuild the same store; the query timestamp sits just past
// the final entry, which is what forces both arms through a full floor lookup.
public sealed partial class TimeBasedKeyValueStoreBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameStore() =>
        Assert.Equal(BuildHarness().LinearFloorScan(), BuildHarness().LinearFloorScan());

    [Fact]
    public void LinearFloorScan_SmallestLength_AgreesWithBinarySearchFloor()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchFloor(), harness.LinearFloorScan());
    }

    [Fact]
    public void BinarySearchFloor_SmallestLength_AgreesWithLinearFloorScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearFloorScan(), harness.BinarySearchFloor());
    }

    private static TimeBasedKeyValueStoreBenchmarks BuildHarness()
    {
        var harness = new TimeBasedKeyValueStoreBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
