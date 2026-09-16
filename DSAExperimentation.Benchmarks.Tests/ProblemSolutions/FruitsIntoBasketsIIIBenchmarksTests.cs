using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FruitsIntoBasketsIIIBenchmarks (ARCHITECTURE 17.9): both arms are
// FruitsIntoBasketsIIISolution's - the O(n^2) rescan against the SegmentTree-backed search - so a
// harness whose arms disagree is timing two different problems. Fruits and baskets are drawn
// independently across the problem's full capacity range, which is what keeps the rescan from
// short-circuiting on an easy shape and forces both strategies through their real placement work.
// Both arms answer with the bare count of unplaced fruits, so agreement witnesses that the two
// strategies placed the same number of them over the same arrays. Setup draws both arrays off one
// seeded generator, so the same Length must rebuild both.
public sealed partial class FruitsIntoBasketsIIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_RandomCapacitiesAcrossTheFullRange_AgreesWithSegmentTreeSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeSearch(), harness.BruteForce());
    }

    [Fact]
    public void SegmentTreeSearch_RandomCapacitiesAcrossTheFullRange_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SegmentTreeSearch());
    }

    private static FruitsIntoBasketsIIIBenchmarks BuildHarness()
    {
        var harness = new FruitsIntoBasketsIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
