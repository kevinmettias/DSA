using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LexicographicallySmallestGeneratedStringBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - re-verifying and rewriting the whole
// template at every constraint position against the O(1) overlap check this repo's ZFunction gives -
// so a harness whose arms disagree is timing two different problems. Setup builds str1 as one
// repeated 'T' and str2 as one repeated character, so every window overlap is consistent and neither
// arm short-circuits; the same Length must rebuild the same pair, otherwise two published numbers
// were never comparable in the first place.
public sealed partial class LexicographicallySmallestGeneratedStringBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameConstraintPair() =>
        Assert.Equal(BuildHarness().DirectFill(), BuildHarness().DirectFill());

    [Fact]
    public void DirectFill_ConsistentWindowOverlaps_AgreesWithZFunctionConsistency()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ZFunctionConsistency(), harness.DirectFill());
    }

    [Fact]
    public void ZFunctionConsistency_ConsistentWindowOverlaps_AgreesWithDirectFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DirectFill(), harness.ZFunctionConsistency());
    }

    private static LexicographicallySmallestGeneratedStringBenchmarks BuildHarness()
    {
        var harness = new LexicographicallySmallestGeneratedStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
