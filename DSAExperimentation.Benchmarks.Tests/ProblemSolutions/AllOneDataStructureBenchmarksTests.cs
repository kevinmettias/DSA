using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AllOneDataStructureBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin:
// that the replay observes real keys. Both arms answer with every key GetMaxKey/GetMinKey reported, in order, and
// legitimately pick different keys from a tied count, so ArmAgreement only runs each to completion. The script
// seeds every key before its first Get* call, so LC 432's empty-string "no key" answer must never appear; one
// that did would mean the script never placed a key.
public sealed partial class AllOneDataStructureBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] script lengths.
    private const int SmallestLength = 200;

    // Each of the Length rounds asks GetMaxKey and GetMinKey once.
    private const int ReportsPerRound = 2;

    [Fact]
    public void BruteForceDictionaryScan_TwoHundredKeyScript_ReportsAKeyForEveryGet() =>
        AssertReportsAKeyForEveryGet(BuildHarness().BruteForceDictionaryScan());

    [Fact]
    public void BucketedLinkedListOnePass_TwoHundredKeyScript_ReportsAKeyForEveryGet() =>
        AssertReportsAKeyForEveryGet(BuildHarness().BucketedLinkedListOnePass());

    private static void AssertReportsAKeyForEveryGet(string[] reported)
    {
        Assert.Equal(SmallestLength * ReportsPerRound, reported.Length);
        Assert.DoesNotContain(string.Empty, reported);
    }

    private static AllOneDataStructureBenchmarks BuildHarness()
    {
        var harness = new AllOneDataStructureBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
