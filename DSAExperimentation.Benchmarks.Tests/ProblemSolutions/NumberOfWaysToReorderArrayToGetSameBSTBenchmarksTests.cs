using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfWaysToReorderArrayToGetSameBSTBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one count - splitting lists recursively against the primitive
// tree fold - so a harness whose arms disagree is timing two different problems. Setup owns the
// seeded shuffle, so the same Length must permute 1..Length into the very same array; the smallest
// tuned Length keeps the recursive counting cheap to call twice.
public sealed partial class NumberOfWaysToReorderArrayToGetSameBSTBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveListSplitting(), BuildHarness().NaiveListSplitting());

    [Fact]
    public void NaiveListSplitting_SmallestLength_AgreesWithPrimitiveComposed()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrimitiveComposed(), harness.NaiveListSplitting());
    }

    [Fact]
    public void PrimitiveComposed_SmallestLength_AgreesWithNaiveListSplitting()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveListSplitting(), harness.PrimitiveComposed());
    }

    private static NumberOfWaysToReorderArrayToGetSameBSTBenchmarks BuildHarness()
    {
        var harness = new NumberOfWaysToReorderArrayToGetSameBSTBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
