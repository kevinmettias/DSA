using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MoveZeroesBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies for
// the same question - the jump-to-next-nonzero scan against the array-indexed two-pointer pass - so a harness
// whose arms disagree is timing two different problems. Both arms copy _values and move zeroes in that private
// copy, so the hoisted field is never mutated and one harness instance is safe to call twice in either order.
// Setup derives the front-loaded-zeroes array from Length alone, so the same Length must rebuild the same
// array; otherwise two published numbers were never comparable in the first place.
//
// WEAK AGREEMENT, by construction. Both arms return a single element of the moved array - nums[0], chosen as
// the cheapest observable proof that the move ran - so agreement witnesses that both arms left the same value
// at index 0, not that they produced the same array. An arm that moved every zero correctly but disagreed
// anywhere past index 0 would still pass. The return type is the benchmark's to change, not this harness's,
// so the honest, index-only assertion is used and the limitation is recorded here rather than papered over.
public sealed partial class MoveZeroesBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameFrontLoadedArray() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_FrontLoadedZeroes_AgreesWithArrayIndexedTwoPointer()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayIndexedTwoPointer(), harness.LinearScan());
    }

    [Fact]
    public void ArrayIndexedTwoPointer_FrontLoadedZeroes_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.ArrayIndexedTwoPointer());
    }

    private static MoveZeroesBenchmarks BuildHarness()
    {
        var harness = new MoveZeroesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
