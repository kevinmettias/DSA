using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SingleNumberIIBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the answer the workload is built to have. Setup builds its
// workload in closed form: every value it draws is contributed three times and therefore cancels
// in each bit's count mod three, and the one unpaired value it appends is its own fixed singleton,
// so there is exactly one possible answer whatever the values and whatever order the shuffle put
// them in.
public sealed partial class SingleNumberIIBenchmarksTests
{
    private const int SmallestTripleCount = 200;

    // Setup contributes every drawn value three times and appends this one unpaired value, so
    // this is the whole of what a correct bit-count-mod-three sweep over that array can return.
    private const int ExpectedSingleton = -1;

    [Fact]
    public void BitCountModThree_TripledValues_ReturnsTheSeededSingleton() =>
        Assert.Equal(ExpectedSingleton, BuildHarness().BitCountModThree());

    private static SingleNumberIIBenchmarks BuildHarness()
    {
        var harness = new SingleNumberIIBenchmarks { TripleCount = SmallestTripleCount };
        harness.Setup();

        return harness;
    }
}
