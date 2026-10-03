using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignBrowserHistoryBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins every page the replay's Back calls land on, from the script
// alone.
//
// Every iteration visits url<i>, steps one page back and visits branch<i>, where <i> is i spelled by
// LowercaseNames.Of. The step back lands on the page before url<i> - the home page the first time,
// branch<i-1> after that - and the branch visit then discards url<i> as forward history. So the
// history only ever holds home and the branches so far, growing by one page per iteration, and a
// Visit that appended without truncating would leave url<i> behind for a later Back to land on.
public sealed partial class DesignBrowserHistoryBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    // Mirrors the benchmark's own home page and url format.
    private const string HomePageUrl = "home.com";
    private const string BranchPrefix = "branch";
    private const string UrlSuffix = ".com";

    [Fact]
    public void ListBacked_VisitBackBranchCycle_LandsOnThePreviousBranchEachTime() =>
        Assert.Equal(ExpectedBackPages(), BuildHarness().ListBacked());

    [Fact]
    public void DynamicArrayBacked_VisitBackBranchCycle_LandsOnThePreviousBranchEachTime() =>
        Assert.Equal(ExpectedBackPages(), BuildHarness().DynamicArrayBacked());

    private static string[] ExpectedBackPages() =>
        [HomePageUrl, .. Enumerable.Range(0, SmallestOperationCount - 1).Select(i => BranchPrefix + LowercaseNames.Of(i) + UrlSuffix)];

    private static DesignBrowserHistoryBenchmarks BuildHarness()
    {
        var harness = new DesignBrowserHistoryBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
