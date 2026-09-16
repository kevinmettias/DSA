using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - the smallest integer the digit
// string can be rearranged into within the swap budget - so a harness whose arms disagree is timing
// two different problems. The budget is fixed effectively unlimited, so both arms must consider the
// whole remaining digit list at every slot and the answer is pinned by the multiset of digits alone;
// agreement still catches a Fenwick count that drifts from the physical list. Setup draws the digits
// from one fixed seed, so the same Length must rebuild the same string.
public sealed partial class MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceListRemoval(), BuildHarness().BruteForceListRemoval());

    [Fact]
    public void BruteForceListRemoval_SameDigitString_AgreesWithFenwickTreeGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeGreedy(), harness.BruteForceListRemoval());
    }

    [Fact]
    public void FenwickTreeGreedy_SameDigitString_AgreesWithBruteForceListRemoval()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceListRemoval(), harness.FenwickTreeGreedy());
    }

    private static MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsBenchmarks BuildHarness()
    {
        var harness = new MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
