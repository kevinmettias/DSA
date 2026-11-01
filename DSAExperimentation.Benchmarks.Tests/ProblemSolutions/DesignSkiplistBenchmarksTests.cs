using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignSkiplistBenchmarks (ARCHITECTURE 17.9): setup draws a call script that
// adds every value it later searches and erases, so the replay total is known from OperationCount
// alone, without running either arm.
public sealed partial class DesignSkiplistBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    // Every drawn value is added first, then searched once, then erased once: all
    // OperationCount searches hit, and each value's copies are erased one call at a time until
    // none are left, so every OperationCount erases removes an occurrence too.
    private const int ExpectedReplayTotal = 2 * SmallestOperationCount;

    // The multiset is empty before the script runs, so neither half of the replay can report
    // more than its own OperationCount calls, and a value still holding an occurrence when it
    // is searched or an occurrence left behind by the erases would leave the total short.
    [Fact]
    public void LinearScanList_AddSearchEraseScript_HitsEverySearchAndErase() =>
        Assert.Equal(ExpectedReplayTotal, BuildHarness().LinearScanList());

    private static DesignSkiplistBenchmarks BuildHarness()
    {
        var harness = new DesignSkiplistBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
