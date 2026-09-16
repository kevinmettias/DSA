using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignSkiplistBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a flat list scanned for the value against this repo's own
// FenwickTree used as a frequency array - so a harness whose arms disagree is timing two different
// problems. Setup draws the whole call script from one fixed seed, so the same OperationCount must
// rebuild the same script, and that script adds every value it later searches and erases.
public sealed partial class DesignSkiplistBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    // Every drawn value is added first, then searched once, then erased once: all
    // OperationCount searches hit, and each value's copies are erased one call at a time until
    // none are left, so every OperationCount erases removes an occurrence too.
    private const int ExpectedReplayTotal = 2 * SmallestOperationCount;

    [Fact]
    public void Setup_SameOperationCount_RebuildsTheSameAddSearchEraseScript()
    {
        // The multiset is empty before the script runs, so neither half of the replay can report
        // more than its own OperationCount calls, and a value still holding an occurrence when it
        // is searched or an occurrence left behind by the erases would leave the total short.
        Assert.Equal(ExpectedReplayTotal, BuildHarness().LinearScanList());
        Assert.Equal(BuildHarness().LinearScanList(), BuildHarness().LinearScanList());
    }

    [Fact]
    public void LinearScanList_AddSearchEraseScript_AgreesWithFenwickTreeFrequencyMultiset()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeFrequencyMultiset(), harness.LinearScanList());
    }

    [Fact]
    public void FenwickTreeFrequencyMultiset_AddSearchEraseScript_AgreesWithLinearScanList()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanList(), harness.FenwickTreeFrequencyMultiset());
    }

    private static DesignSkiplistBenchmarks BuildHarness()
    {
        var harness = new DesignSkiplistBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
