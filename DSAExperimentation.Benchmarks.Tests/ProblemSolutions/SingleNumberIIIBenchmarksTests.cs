using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SingleNumberIIIBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins each arm's answer to the two singletons the workload seeds.
//
// The genuinely unfixed order here is the order of the two singletons inside the returned pair:
// LeetCode 260 accepts them in either order, and the two arms enumerate differently - the
// brute force follows the array's own order, the frequency count follows its hash map's key
// order - so the pair is compared as a set, with each singleton still compared by value.
public sealed partial class SingleNumberIIIBenchmarksTests
{
    private const int SmallestLength = 200;

    // The two unpaired values the workload seeds into the array, and therefore the whole of
    // what either arm may report - in whichever of the two orders each arm happens to find them.
    private static readonly int[] SeededSingletons = [-1, -2];

    [Fact]
    public void BruteForce_TwoSeededSingletons_ReturnsBothSingletons() =>
        Assert.Equal(
            AnswerGraphText.OfUnordered(SeededSingletons),
            AnswerGraphText.OfUnordered(BuildHarness().BruteForce()));

    [Fact]
    public void HashMapFrequencyCount_TwoSeededSingletons_ReturnsBothSingletons() =>
        Assert.Equal(
            AnswerGraphText.OfUnordered(SeededSingletons),
            AnswerGraphText.OfUnordered(BuildHarness().HashMapFrequencyCount()));

    private static SingleNumberIIIBenchmarks BuildHarness()
    {
        var harness = new SingleNumberIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
