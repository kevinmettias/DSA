using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConcatenatedWordsBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on a rebuilt workload, so this asserts only what the workload's
// documented shape decides - one long tiling of the dictionary word plus that word itself - which
// means exactly one entry is a concatenation.
public sealed partial class ConcatenatedWordsBenchmarksTests
{
    private const int SmallestLength = 6;

    [Fact]
    public void HashSetUnboundedScan_TiledDictionaryWord_FindsExactlyOneConcatenation() =>
        Assert.Single(BuildHarness().HashSetUnboundedScan());

    private static ConcatenatedWordsBenchmarks BuildHarness()
    {
        var harness = new ConcatenatedWordsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
