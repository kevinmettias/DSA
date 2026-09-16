using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConcatenatedDivisibilityBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - enumerating permutations against the bitmask DP - so
// a harness whose arms disagree is timing two different problems. Setup draws the numbers and the
// divisor from `Seed + NumberCount`, so the same NumberCount must rebuild the same numbers and the
// same divisor; otherwise two published numbers were never comparable in the first place.
//
// The numbers and the divisor are both private, and IList<int> is the only thing either arm reports,
// so the workload's documented shape is asserted through that: LC 3533's answer is either the empty
// arrangement (no permutation's concatenation is divisible) or a full SmallestNumberCount-element
// permutation - never a partial one.
public sealed partial class ConcatenatedDivisibilityBenchmarksTests
{
    private const int SmallestNumberCount = 6;

    [Fact]
    public void Setup_SameNumberCount_RebuildsTheSameNumbersAndDivisor()
    {
        Assert.True(BuildHarness().Backtracking().Count is 0 or SmallestNumberCount);
        Assert.Equal(
            AnswerText.Of(BuildHarness().Backtracking()),
            AnswerText.Of(BuildHarness().Backtracking()));
    }

    [Fact]
    public void Backtracking_SixSeededNumbers_AgreesWithBitmaskMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.BitmaskMemo()), AnswerText.Of(harness.Backtracking()));
    }

    [Fact]
    public void BitmaskMemo_SixSeededNumbers_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.Backtracking()), AnswerText.Of(harness.BitmaskMemo()));
    }

    private static ConcatenatedDivisibilityBenchmarks BuildHarness()
    {
        var harness = new ConcatenatedDivisibilityBenchmarks { NumberCount = SmallestNumberCount };
        harness.Setup();

        return harness;
    }
}
