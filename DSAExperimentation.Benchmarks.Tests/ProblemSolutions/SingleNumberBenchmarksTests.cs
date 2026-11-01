using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SingleNumberBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the answer the workload is built to have. Setup builds its
// workload in closed form: every value it draws is contributed twice and therefore cancels, and the
// one unpaired value it appends is its own fixed singleton, so there is exactly one possible answer
// whatever the values and whatever order the shuffle put them in.
public sealed partial class SingleNumberBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup contributes every drawn value twice and appends this one unpaired value, so this is
    // the whole of what a correct fold over that array can return.
    private const int ExpectedSingleton = -1;

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
