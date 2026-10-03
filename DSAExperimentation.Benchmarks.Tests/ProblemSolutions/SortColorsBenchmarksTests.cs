using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortColorsBenchmarks (ARCHITECTURE 17.9): both arms are
// SortColorsSolution's in-place sorts of the same cycling input, each on its own copy, so a
// harness whose arms disagree is timing two different problems. Setup is a pure function of
// Length, so the same Length must rebuild the same values.
//
// Each arm returns its sorted copy, and the sorted array is derivable from Length alone: Setup's
// 2, 1, 0 cycle restated here and put in order by the BCL's own sort. Each arm is asserted against
// that array, which catches an arm that left the array unsorted, sorted it descending, or lost or
// duplicated a color.
public sealed partial class SortColorsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup cycles 2, 1, 0, so the smallest color is present in every workload the Params admit.
    private const int SmallestGeneratedColor = 0;

    // Restated from the benchmark's generator: colors 0..MaxColorValue, cycled from the largest.
    private const int MaxColorValue = 2;
    private const int ColorCount = 3;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ArraySort(), BuildHarness().ArraySort());

    [Fact]
    public void ArraySort_TwoHundredCyclingColors_SortsEveryColorAscending()
    {
        var sorted = BuildHarness().ArraySort();

        Assert.Equal(SmallestGeneratedColor, sorted[0]);
        Assert.Equal(ExpectedSorted(), sorted);
    }

    [Fact]
    public void ArrayIndexedDutchFlag_TwoHundredCyclingColors_SortsEveryColorAscending()
    {
        var sorted = BuildHarness().ArrayIndexedDutchFlag();

        Assert.Equal(SmallestGeneratedColor, sorted[0]);
        Assert.Equal(ExpectedSorted(), sorted);
    }

    private static int[] ExpectedSorted() =>
        [.. Enumerable.Range(0, SmallestLength).Select(i => MaxColorValue - (i % ColorCount)).Order()];

    private static SortColorsBenchmarks BuildHarness()
    {
        var harness = new SortColorsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
