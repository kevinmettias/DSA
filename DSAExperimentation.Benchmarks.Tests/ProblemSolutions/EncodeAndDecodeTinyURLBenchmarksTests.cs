using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EncodeAndDecodeTinyURLBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a flat list matched by a linear scan against this
// repo's own HashMap pair - so a harness whose arms disagree is timing two different problems. Setup
// fills both codecs with the same Length long URLs and pins the target to the LAST-inserted pair,
// which is the class comment's own point: decoding it forces each strategy through its worst case
// rather than an early hit. Both codecs sit in fields but Setup is what populates them and Decode
// only reads, so one harness is safe to read twice in either order. The last inserted pair is
// article Length - 1, which is 199 at the smallest Length the benchmark lists.
public sealed partial class EncodeAndDecodeTinyURLBenchmarksTests
{
    private const int SmallestLength = 200;
    private const string ExpectedDecodedUrlAtTheLastInsertedPair = "https://example.com/article/199";

    [Fact]
    public void Setup_TwoHundredEncodedUrls_DecodesTheLastInsertedPairAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDecodedUrlAtTheLastInsertedPair, harness.LinearScanDecode());
        Assert.Equal(harness.LinearScanDecode(), BuildHarness().LinearScanDecode());
    }

    [Fact]
    public void LinearScanDecode_LastInsertedPair_AgreesWithHashMapDecode()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapDecode(), harness.LinearScanDecode());
    }

    [Fact]
    public void HashMapDecode_LastInsertedPair_AgreesWithLinearScanDecode()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanDecode(), harness.HashMapDecode());
    }

    private static EncodeAndDecodeTinyURLBenchmarks BuildHarness()
    {
        var harness = new EncodeAndDecodeTinyURLBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
