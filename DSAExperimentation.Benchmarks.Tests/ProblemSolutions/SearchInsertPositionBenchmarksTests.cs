using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchInsertPositionBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the answer the workload is built to have. Setup builds Length
// even values by the class's own step and aims one past the largest of them, so the target falls
// strictly beyond every element and the answer is fixed independently of either strategy: the whole
// array precedes it, so its insertion position is exactly Length.
public sealed partial class SearchInsertPositionBenchmarksTests
{
    private const int SmallestLength = 200;

    // The target sits past every element, so every element precedes it.
    private const int ExpectedInsertIndex = SmallestLength;

    [Fact]
    public void LinearScan_TargetPastEveryValue_InsertsAtTheEnd() =>
        Assert.Equal(ExpectedInsertIndex, BuildHarness().LinearScan());

    private static SearchInsertPositionBenchmarks BuildHarness()
    {
        var harness = new SearchInsertPositionBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
