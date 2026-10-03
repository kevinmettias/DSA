using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SingleNumberIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a repeated inner scan against this repo's own
// HashMap frequency count - so a harness whose arms disagree is counting two different arrays.
// Setup draws the paired values from one fixed seed through SingleNumberIIIWorkloads, so the
// same Length must rebuild the same array; otherwise two published numbers were never
// comparable.
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
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(
            AnswerGraphText.OfUnordered(BuildHarness().BruteForce()),
            AnswerGraphText.OfUnordered(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_TwoSeededSingletons_AgreesWithHashMapFrequencyCount()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.OfUnordered(harness.HashMapFrequencyCount()),
            AnswerGraphText.OfUnordered(harness.BruteForce()));
        Assert.Equal(
            AnswerGraphText.OfUnordered(SeededSingletons),
            AnswerGraphText.OfUnordered(harness.BruteForce()));
    }

    [Fact]
    public void HashMapFrequencyCount_TwoSeededSingletons_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.OfUnordered(harness.BruteForce()),
            AnswerGraphText.OfUnordered(harness.HashMapFrequencyCount()));
        Assert.Equal(
            AnswerGraphText.OfUnordered(SeededSingletons),
            AnswerGraphText.OfUnordered(harness.HashMapFrequencyCount()));
    }

    private static SingleNumberIIIBenchmarks BuildHarness()
    {
        var harness = new SingleNumberIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
