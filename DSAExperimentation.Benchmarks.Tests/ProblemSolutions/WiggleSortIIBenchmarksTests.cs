using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WiggleSortIIBenchmarks (ARCHITECTURE 17.9): both arms are competing sorts
// feeding one shared interleave, so a harness whose arms disagree is timing two different problems.
// Each arm clones the shared workload internally rather than mutating a hoisted array, so one
// harness instance is safe to call twice in either order. The interleave is the part both arms
// share, which is exactly what arm agreement cannot witness - so LC 324's alternating inequality is
// also asserted directly against the returned arrangement.
public sealed partial class WiggleSortIIBenchmarksTests
{
    private const int SmallestLength = 200;

    // The arrangement alternates at every index, so which inequality applies is the index's parity -
    // that is, the index read modulo this.
    private const int IndexParityModulus = 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().SelectionSortInterleave()),
            AnswerText.Of(BuildHarness().SelectionSortInterleave()));

    [Fact]
    public void SelectionSortInterleave_AgreesWithMergeSortInterleave()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SelectionSortInterleave()),
            AnswerText.Of(harness.MergeSortInterleave()));
    }

    [Fact]
    public void MergeSortInterleave_AgreesWithSelectionSortInterleave()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MergeSortInterleave()),
            AnswerText.Of(harness.SelectionSortInterleave()));
    }

    [Fact]
    public void SelectionSortInterleave_ProducesAWiggleArrangement()
    {
        var values = BuildHarness().SelectionSortInterleave();

        Assert.Equal(SmallestLength, values.Length);
        Assert.True(IsWiggleArrangement(values));
    }

    private static WiggleSortIIBenchmarks BuildHarness()
    {
        var harness = new WiggleSortIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // nums[0] < nums[1] > nums[2] < nums[3] ...: the pair starting at an odd index ascends, the
    // pair starting at an even index descends.
    private static bool IsWiggleArrangement(int[] values) =>
        Enumerable.Range(1, values.Length - 1)
            .All(index => IsOdd(index) ? values[index - 1] < values[index] : values[index - 1] > values[index]);

    private static bool IsOdd(int index) => index % IndexParityModulus == 1;
}
