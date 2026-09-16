using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestConsecutiveSequenceBenchmarks (ARCHITECTURE 17.9): it carries a
// single arm - this repo's set-based run expansion - so there is nothing to agree with and the
// oracle is derived instead. Setup's workload comes from LongestConsecutiveSequenceWorkloads,
// which is seeded and so reproducible: the same Length rebuilds the same array, whose values all
// lie in [0, Length / 2). The expected longest run is recomputed here by sorting and walking the
// array once - a different algorithm from the arm's predecessor-lookup check - so the assertion
// is not the arm restated.
public sealed partial class LongestConsecutiveSequenceBenchmarksTests
{
    private const int SmallestLength = 200;

    // The seed Setup hands LongestConsecutiveSequenceWorkloads.BuildArray.
    private const int WorkloadSeed = 128;

    private const int LowestFixtureValue = 0;

    // The fixture draws from [0, Length / 2), so its highest possible value is one below that.
    private const int HighestFixtureValue = (SmallestLength / 2) - 1;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var nums = LongestConsecutiveSequenceWorkloads.BuildArray(SmallestLength, WorkloadSeed);

        Assert.Equal(SmallestLength, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, LowestFixtureValue, HighestFixtureValue));
        Assert.Equal(
            AnswerText.Of(BuildHarness().SetRunExpansion()),
            AnswerText.Of(BuildHarness().SetRunExpansion()));
    }

    [Fact]
    public void SetRunExpansion_SmallestLength_CountsTheLongestConsecutiveRun()
    {
        var nums = LongestConsecutiveSequenceWorkloads.BuildArray(SmallestLength, WorkloadSeed);

        Assert.Equal(ExpectedLongestRun(nums), BuildHarness().SetRunExpansion());
    }

    // Sorted-and-walked oracle: after ordering the distinct values, a run of successive integers
    // is exactly a maximal stretch where each value is its predecessor plus one.
    private static int ExpectedLongestRun(int[] nums)
    {
        var ordered = nums.Distinct().Order().ToArray();
        var longest = 0;
        var current = 0;

        for (var i = 0; i < ordered.Length; i++)
        {
            current = i > 0 && ordered[i] == ordered[i - 1] + 1 ? current + 1 : 1;
            longest = Math.Max(longest, current);
        }

        return longest;
    }

    private static LongestConsecutiveSequenceBenchmarks BuildHarness()
    {
        var harness = new LongestConsecutiveSequenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
