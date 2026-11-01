using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BeautifulArrangementBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to
// each other, and this asserts the count is positive - a search that placed nothing would otherwise agree with a
// pruned search that also placed nothing. The class has no [GlobalSetup]: the single [Params] size is the whole
// input, so the harness is constructed per size and the arm called directly.
public sealed partial class BeautifulArrangementBenchmarksTests
{
    // The smaller of [Params(6, 8)] arrangement sizes.
    private const int SmallestSize = 6;

    [Fact]
    public void GenerateThenFilter_SixElementPermutations_CountsAtLeastOneArrangement() =>
        Assert.True(BuildHarness().GenerateThenFilter() > 0);

    private static BeautifulArrangementBenchmarks BuildHarness() =>
        new() { Size = SmallestSize };
}
