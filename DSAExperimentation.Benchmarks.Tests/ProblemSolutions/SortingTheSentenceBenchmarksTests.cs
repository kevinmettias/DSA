using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortingTheSentenceBenchmarks (ARCHITECTURE 17.9): both arms are
// SortingTheSentenceSolution's reconstructions of the same shuffled word array, so a harness
// whose arms disagree is timing two different problems. Setup is a pure function of Length and
// its own fixed seed, so the same Length must rebuild the same shuffle.
//
// The merge-sort arm clones the array it is handed and the scan arm writes into its own result,
// so neither mutates the prepared words and one harness is safe to call twice in either order.
// Each arm returns the reconstructed sentence itself, so comparing the two strings compares the
// whole answer rather than a proxy for it.
public sealed partial class SortingTheSentenceBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameShuffle() =>
        Assert.Equal(BuildHarness().PositionScan(), BuildHarness().PositionScan());

    [Fact]
    public void PositionScan_TwoHundredShuffledWords_AgreesWithMergeSortByPosition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortByPosition(), harness.PositionScan());
    }

    [Fact]
    public void MergeSortByPosition_TwoHundredShuffledWords_AgreesWithPositionScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PositionScan(), harness.MergeSortByPosition());
    }

    private static SortingTheSentenceBenchmarks BuildHarness()
    {
        var harness = new SortingTheSentenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
