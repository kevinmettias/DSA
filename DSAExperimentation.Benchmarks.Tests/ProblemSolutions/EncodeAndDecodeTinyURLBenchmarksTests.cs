using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EncodeAndDecodeTinyURLBenchmarks (ARCHITECTURE 17.9): an expected URL known
// from the workload's shape. Setup fills the codecs with Length long URLs and pins the target to the
// LAST-inserted pair, which is the class comment's own point: decoding it forces each strategy
// through its worst case rather than an early hit. The last inserted pair is article Length - 1,
// which is 199 at the smallest Length the benchmark lists.
public sealed partial class EncodeAndDecodeTinyURLBenchmarksTests
{
    private const int SmallestLength = 200;
    private const string ExpectedDecodedUrlAtTheLastInsertedPair = "https://example.com/article/199";

    [Fact]
    public void LinearScanDecode_TwoHundredEncodedUrls_DecodesTheLastInsertedPair() =>
        Assert.Equal(ExpectedDecodedUrlAtTheLastInsertedPair, BuildHarness().LinearScanDecode());

    private static EncodeAndDecodeTinyURLBenchmarks BuildHarness()
    {
        var harness = new EncodeAndDecodeTinyURLBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
