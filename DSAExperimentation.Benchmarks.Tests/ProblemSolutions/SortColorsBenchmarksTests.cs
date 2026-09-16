using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortColorsBenchmarks (ARCHITECTURE 17.9): both arms are
// SortColorsSolution's in-place sorts of the same cycling input, each on its own copy, so a
// harness whose arms disagree is timing two different problems. Setup is a pure function of
// Length, so the same Length must rebuild the same values.
//
// WEAK BY CONSTRUCTION in one direction and reported as such: each arm reports only its sorted
// array's first element, not the array, so agreement witnesses that both arms produced the same
// element at index 0 - an arm that mis-sorted anything past the first element is not caught by
// the comparison alone. The element is also asserted against the smallest color the generator
// emits, which is what makes the proxy decisive for the sort: an arm that left the array
// unsorted reports the generator's first value, and an arm that sorted descending reports its
// largest.
public sealed partial class SortColorsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup cycles 2, 1, 0, so the smallest color is present in every workload the Params admit.
    private const int SmallestGeneratedColor = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ArraySort(), BuildHarness().ArraySort());

    [Fact]
    public void ArraySort_TwoHundredCyclingColors_AgreesWithArrayIndexedDutchFlag()
    {
        var harness = BuildHarness();

        Assert.Equal(SmallestGeneratedColor, harness.ArraySort());
        Assert.Equal(harness.ArrayIndexedDutchFlag(), harness.ArraySort());
    }

    [Fact]
    public void ArrayIndexedDutchFlag_TwoHundredCyclingColors_AgreesWithArraySort()
    {
        var harness = BuildHarness();

        Assert.Equal(SmallestGeneratedColor, harness.ArrayIndexedDutchFlag());
        Assert.Equal(harness.ArraySort(), harness.ArrayIndexedDutchFlag());
    }

    private static SortColorsBenchmarks BuildHarness()
    {
        var harness = new SortColorsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
