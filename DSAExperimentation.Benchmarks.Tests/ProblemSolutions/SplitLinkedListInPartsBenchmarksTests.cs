using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SplitLinkedListInPartsBenchmarks (ARCHITECTURE 17.9): both arms are
// SplitLinkedListInPartsSolution's splits of the same prepared chain into the same number of
// parts - one rebuilding from an array, the other rewiring in place - so a harness whose arms
// disagree is timing two different problems. Setup is a pure function of Length and each arm
// clones the chain before mutating it, so the same Length must rebuild the same values and one
// harness is safe to call twice in either order.
//
// WEAK BY CONSTRUCTION, and reported as such: each arm reports only how many of its parts are
// non-null, not the parts themselves, so agreement witnesses that both splits consumed the chain
// - an arm that dropped a node is caught, an arm that cut the parts at different places is not.
// The count is also asserted against the requested part count, which is what makes it decisive
// for that much: Length is far larger than the part count, so every part must be non-empty and
// the count must equal the part count exactly.
public sealed partial class SplitLinkedListInPartsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the part count the benchmark itself splits into.
    private const int ExpectedPartCount = 7;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameChain() =>
        Assert.Equal(BuildHarness().ArrayRebuild(), BuildHarness().ArrayRebuild());

    [Fact]
    public void ArrayRebuild_TwoHundredNodes_AgreesWithInPlaceRewire()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPartCount, harness.ArrayRebuild());
        Assert.Equal(harness.InPlaceRewire(), harness.ArrayRebuild());
    }

    [Fact]
    public void InPlaceRewire_TwoHundredNodes_AgreesWithArrayRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPartCount, harness.InPlaceRewire());
        Assert.Equal(harness.ArrayRebuild(), harness.InPlaceRewire());
    }

    private static SplitLinkedListInPartsBenchmarks BuildHarness()
    {
        var harness = new SplitLinkedListInPartsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
