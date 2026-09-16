using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheStringWithLCPBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. Setup draws the seed string from a fixed seed and derives its LCP matrix,
// so the same Length must rebuild the same workload.
//
// Both arms return a bare string, so the two calls are compared directly.
public sealed partial class FindTheStringWithLCPBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DirectSweep(), BuildHarness().DirectSweep());

    [Fact]
    public void DirectSweep_AgreesWithDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSet(), harness.DirectSweep());
    }

    [Fact]
    public void DisjointSet_AgreesWithDirectSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DirectSweep(), harness.DisjointSet());
    }

    private static FindTheStringWithLCPBenchmarks BuildHarness()
    {
        var harness = new FindTheStringWithLCPBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
