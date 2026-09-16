using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PartitionArrayIntoTwoEqualProductSubsetsBenchmarks (ARCHITECTURE 17.9): its
// two arms are PartitionArrayIntoTwoEqualProductSubsetsSolution's, competing searches for the same
// yes/no question - full bitmask enumeration against pruned backtracking - so a harness whose arms
// disagree is timing two different problems. Setup draws distinct values from one fixed seed, so
// the same Length must rebuild the same array.
//
// The agreement is weak by construction and the tests below say so rather than dressing it up:
// Target sits far below the workload's full product, so both arms answer false and the comparison
// can only catch an arm that ever answers true. Neither arm is asked for a witness, so nothing here
// can tell a correct "no" from an unpruned "no" - strengthening that is a harness decision, not a
// test's.
public sealed partial class PartitionArrayIntoTwoEqualProductSubsetsBenchmarksTests
{
    private const int SmallestLength = 12;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CanPartitionEquallyByBitmaskEnumeration(),
            BuildHarness().CanPartitionEquallyByBitmaskEnumeration());

    [Fact]
    public void CanPartitionEquallyByBitmaskEnumeration_UnreachableTarget_AgreesWithCanPartitionEquallyByPrunedBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanPartitionEquallyByPrunedBacktracking(),
            harness.CanPartitionEquallyByBitmaskEnumeration());
    }

    [Fact]
    public void CanPartitionEquallyByPrunedBacktracking_UnreachableTarget_AgreesWithCanPartitionEquallyByBitmaskEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanPartitionEquallyByBitmaskEnumeration(),
            harness.CanPartitionEquallyByPrunedBacktracking());
    }

    private static PartitionArrayIntoTwoEqualProductSubsetsBenchmarks BuildHarness()
    {
        var harness = new PartitionArrayIntoTwoEqualProductSubsetsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
