using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DecodeStringBenchmarks (ARCHITECTURE 17.9): the generic arm check compares the
// arms only with each other, so what stays here is the workload's documented shape. Setup builds the
// encoding from DecodeStringWorkloads' fixed five-character tile, which decodes to four characters, so
// the decoded answer's documented shape is four characters per tile the encoding held.
public sealed partial class DecodeStringBenchmarksTests
{
    private const int SmallestLength = 10;

    // DecodeStringWorkloads repeats "2[ab]" until the encoding reaches the requested length; each tile
    // is five encoded characters and decodes to the four characters of "abab".
    private const int EncodedTileLength = 5;
    private const int DecodedTileLength = 4;
    private const int ExpectedDecodedLength = (SmallestLength / EncodedTileLength) * DecodedTileLength;

    [Fact]
    public void StackScan_TenCharacterEncoding_DecodesFourCharactersPerTile() =>
        Assert.Equal(ExpectedDecodedLength, BuildHarness().StackScan().Length);

    private static DecodeStringBenchmarks BuildHarness()
    {
        var harness = new DecodeStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
