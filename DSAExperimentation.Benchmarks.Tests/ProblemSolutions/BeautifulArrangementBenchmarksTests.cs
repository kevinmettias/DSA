using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BeautifulArrangementBenchmarks (ARCHITECTURE 17.9): its two arms are
// BeautifulArrangementSolution's competing strategies for the same question - generating every permutation and
// rejecting it afterwards against folding the divisibility rule into the search's own candidates - so a harness
// whose arms disagree has counted two different arrangement sets. The class has no [GlobalSetup]: the single
// [Params] size is the whole input, so the harness is constructed per size and the arms called directly. Both
// report a count, which is asserted positive as well as equal - a search that placed nothing would otherwise agree
// with a pruned search that also placed nothing.
public sealed partial class BeautifulArrangementBenchmarksTests
{
    // The smaller of [Params(6, 8)] arrangement sizes.
    private const int SmallestSize = 6;

    [Fact]
    public void GenerateThenFilter_SixElementPermutations_AgreesWithPrunedBacktracking()
    {
        var harness = BuildHarness();

        Assert.True(harness.GenerateThenFilter() > 0);
        Assert.Equal(harness.PrunedBacktracking(), harness.GenerateThenFilter());
    }

    [Fact]
    public void PrunedBacktracking_SixElementPermutations_AgreesWithGenerateThenFilter()
    {
        var harness = BuildHarness();

        Assert.True(harness.PrunedBacktracking() > 0);
        Assert.Equal(harness.GenerateThenFilter(), harness.PrunedBacktracking());
    }

    private static BeautifulArrangementBenchmarks BuildHarness() =>
        new() { Size = SmallestSize };
}
