using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SingleNumberBenchmarks (ARCHITECTURE 17.9). There is only one arm - an
// XOR fold - so there is no second strategy to agree with and the expected answer has to come
// from the fixture instead. Setup builds its workload in closed form: every value it draws is
// contributed twice and therefore cancels in the XOR, and the one unpaired value it appends is
// its own fixed singleton, so the fold has exactly one possible answer whatever the values and
// whatever order the shuffle put them in. Setup draws from a fixed seed, so the same Length
// must rebuild the same array; otherwise two published numbers were never comparable.
public sealed partial class SingleNumberBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup contributes every drawn value twice and appends this one unpaired value, so this is
    // the whole of what a correct fold over that array can return.
    private const int ExpectedSingleton = -1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(BuildHarness().XorFold(), BuildHarness().XorFold());

    [Fact]
    public void XorFold_PairedValues_ReturnsTheSeededSingleton() =>
        Assert.Equal(ExpectedSingleton, BuildHarness().XorFold());

    private static SingleNumberBenchmarks BuildHarness()
    {
        var harness = new SingleNumberBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
