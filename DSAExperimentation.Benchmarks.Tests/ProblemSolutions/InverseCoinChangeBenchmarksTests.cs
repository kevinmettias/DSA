using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InverseCoinChangeBenchmarks (ARCHITECTURE 17.9). Both arms are competing
// strategies for the same question - the array tabulation against the memoized recurrence over the
// same denomination-count array - so a harness whose arms disagree is timing two different
// problems. Each arm returns the denomination set it recovered; LeetCode 3592 accepts any valid set
// but pins the order the denominations are reported in, and both arms report ascending, so
// AnswerText.Of is the right rendering. [GlobalSetup] simulates the real forward DP over a seeded
// denomination set rather than filling the array with random counts - a value neither strategy
// could ever match would return an empty set from the first amount and measure almost nothing - so
// the same Length must rebuild the same counts and therefore the same recovered set.
public sealed partial class InverseCoinChangeBenchmarksTests
{
    private const int SmallestLength = 30;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ArrayTabulation()),
            AnswerText.Of(BuildHarness().ArrayTabulation()));

    [Fact]
    public void ArrayTabulation_RecoveredDenominations_AgreeWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MemoizedRecurrence()),
            AnswerText.Of(harness.ArrayTabulation()));
    }

    [Fact]
    public void MemoizedRecurrence_RecoveredDenominations_AgreeWithArrayTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ArrayTabulation()),
            AnswerText.Of(harness.MemoizedRecurrence()));
    }

    private static InverseCoinChangeBenchmarks BuildHarness()
    {
        var harness = new InverseCoinChangeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
