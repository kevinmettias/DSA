using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DecodeStringBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a recursive-descent walk against a single stack scan - so a
// harness whose arms disagree is timing two different problems. Setup builds the encoding from
// DecodeStringWorkloads' fixed five-character tile, which decodes to four characters, so the decoded
// answer's documented shape is four characters per tile the encoding held; the same Length must rebuild
// the same encoding and with it the same decoded string.
public sealed partial class DecodeStringBenchmarksTests
{
    private const int SmallestLength = 200;

    // DecodeStringWorkloads repeats "2[ab]" until the encoding reaches the requested length; each tile
    // is five encoded characters and decodes to the four characters of "abab".
    private const int EncodedTileLength = 5;
    private const int DecodedTileLength = 4;
    private const int ExpectedDecodedLength = (SmallestLength / EncodedTileLength) * DecodedTileLength;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedDecodedLength, BuildHarness().StackScan().Length);
        Assert.Equal(BuildHarness().StackScan(), BuildHarness().StackScan());
    }

    [Fact]
    public void RecursiveDescent_TwoHundredCharacterEncoding_AgreesWithStackScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackScan(), harness.RecursiveDescent());
    }

    [Fact]
    public void StackScan_TwoHundredCharacterEncoding_AgreesWithRecursiveDescent()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveDescent(), harness.StackScan());
    }

    private static DecodeStringBenchmarks BuildHarness()
    {
        var harness = new DecodeStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
