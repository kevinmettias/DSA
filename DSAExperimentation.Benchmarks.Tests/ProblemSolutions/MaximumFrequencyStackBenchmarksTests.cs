using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumFrequencyStackBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for one question - the sequence of values popped while replaying the push/pop script -
// so a harness whose arms disagree is timing two different problems. Setup builds the script from a
// fixed seed, so the same operation count must rebuild the same script; the arms are safe to call on
// one harness in either order because each builds its own mutable stack inside its own call and the
// script itself is only ever read.
public sealed partial class MaximumFrequencyStackBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    [Fact]
    public void Setup_SameOperationCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RescanListOnEveryPop(), BuildHarness().RescanListOnEveryPop());

    [Fact]
    public void RescanListOnEveryPop_AgreesWithHashMapAndStackByFrequency()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RescanListOnEveryPop(), harness.HashMapAndStackByFrequency());
    }

    [Fact]
    public void HashMapAndStackByFrequency_AgreesWithRescanListOnEveryPop()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapAndStackByFrequency(), harness.RescanListOnEveryPop());
    }

    private static MaximumFrequencyStackBenchmarks BuildHarness()
    {
        var harness = new MaximumFrequencyStackBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
