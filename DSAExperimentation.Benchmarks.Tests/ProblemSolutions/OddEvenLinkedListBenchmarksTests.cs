using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OddEvenLinkedListBenchmarks (ARCHITECTURE 17.9): both arms regroup the same
// chain - one rebuilds it from two value buffers, the other rewires the existing nodes in place - so
// a harness whose arms disagree is timing two different problems. Setup builds the chain 0..Length-1
// once and each arm clones it before mutating, so one harness is safe to call twice in either order.
//
// WEAK BY CONSTRUCTION, and reported as such: both arms return only the node count of the list they
// produced, not the order the nodes ended up in, so agreement witnesses that both regroups consumed
// the whole chain - an arm that dropped or duplicated a node is caught, an arm that grouped the
// parities differently is not. The count is also checked against the built chain's length, which is
// what makes the dropped-node case a real failure rather than a shared one.
public sealed partial class OddEvenLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().TwoListRebuild(), BuildHarness().TwoListRebuild());

    [Fact]
    public void TwoListRebuild_SmallestLength_KeepsEveryNode()
    {
        var harness = BuildHarness();

        Assert.Equal(SmallestLength, harness.TwoListRebuild());
        Assert.Equal(harness.InPlaceRewire(), harness.TwoListRebuild());
    }

    [Fact]
    public void InPlaceRewire_SmallestLength_KeepsEveryNode()
    {
        var harness = BuildHarness();

        Assert.Equal(SmallestLength, harness.InPlaceRewire());
        Assert.Equal(harness.TwoListRebuild(), harness.InPlaceRewire());
    }

    private static OddEvenLinkedListBenchmarks BuildHarness()
    {
        var harness = new OddEvenLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
