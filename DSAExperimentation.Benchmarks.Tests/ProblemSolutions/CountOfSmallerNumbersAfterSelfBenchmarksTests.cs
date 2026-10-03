using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountOfSmallerNumbersAfterSelfBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n^2) pairwise scan against the Fenwick sweep -
// so a harness whose arms disagree is timing two different problems. Setup draws nums from a fixed
// seed through CountOfSmallerNumbersAfterSelfWorkloads, so the same Length must rebuild the same
// array. AnswerGraphText.Of, not OfUnordered: the answer is one count per index, so the position of
// each count in the array is part of it.
public sealed partial class CountOfSmallerNumbersAfterSelfBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the benchmark's own seed, so the workload asserted here is the one Setup builds.
    private const int RandomSeed = 315;

    // The fixture's own value bound: values are drawn from [-ValueBound, ValueBound).
    private const int ValueBound = 10_000;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var nums = CountOfSmallerNumbersAfterSelfWorkloads.BuildNums(SmallestLength, RandomSeed);

        // The documented shape: exactly Length random values spanning both signs of the fixture's
        // bound, so the counts are a real permutation-of-ranks question rather than one value.
        Assert.Equal(SmallestLength, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, -ValueBound, ValueBound - 1));
        Assert.Equal(AnswerGraphText.Of(BuildHarness().PairwiseScan()), AnswerGraphText.Of(BuildHarness().PairwiseScan()));
    }

    [Fact]
    public void PairwiseScan_SmallestLength_AgreesWithFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerGraphText.Of(harness.FenwickTreeSweep()), AnswerGraphText.Of(harness.PairwiseScan()));
    }

    [Fact]
    public void FenwickTreeSweep_SmallestLength_AgreesWithPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerGraphText.Of(harness.PairwiseScan()), AnswerGraphText.Of(harness.FenwickTreeSweep()));
    }

    private static CountOfSmallerNumbersAfterSelfBenchmarks BuildHarness()
    {
        var harness = new CountOfSmallerNumbersAfterSelfBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
