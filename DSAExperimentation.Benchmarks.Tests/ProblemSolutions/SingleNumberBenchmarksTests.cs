using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SingleNumberBenchmarks (ARCHITECTURE 17.9). Its two arms - an XOR fold and
// a HashSet toggle - are competing strategies for the same question, so a harness whose arms
// disagree is solving two different problems. Setup builds its workload in closed form: every value
// it draws is contributed twice and therefore cancels, and the one unpaired value it appends is its
// own fixed singleton, so either arm has exactly one possible answer whatever the values and
// whatever order the shuffle put them in. Setup draws from a fixed seed, so the same Length must
// rebuild the same array; otherwise two published numbers were never comparable.
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

    [Fact]
    public void SetToggling_PairedValues_ReturnsTheSeededSingleton() =>
        Assert.Equal(ExpectedSingleton, BuildHarness().SetToggling());

    [Fact]
    public void SetToggling_AgreesWithXorFold()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.XorFold(), harness.SetToggling());
    }

    private static SingleNumberBenchmarks BuildHarness()
    {
        var harness = new SingleNumberBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
