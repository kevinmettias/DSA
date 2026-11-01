using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestConsecutiveSequenceBenchmarks (ARCHITECTURE 17.9), for what the
// generic agreement check cannot supply: the workload's shape and an answer derived independently of
// the arms. Setup's workload comes from LongestConsecutiveSequenceWorkloads, seeded so it is
// reproducible, whose values all lie in [0, Length / 2). The expected longest run is recomputed here
// by taking the distinct values, ordering them and walking once, and checked against the baseline;
// the generic check holds the set-based arm to the baseline's answer.
public sealed partial class LongestConsecutiveSequenceBenchmarksTests
{
    private const int SmallestLength = 200;

    // The seed Setup hands LongestConsecutiveSequenceWorkloads.BuildArray.
    private const int WorkloadSeed = 128;

    private const int LowestFixtureValue = 0;

    // The fixture draws from [0, Length / 2), so its highest possible value is one below that.
    private const int HighestFixtureValue = (SmallestLength / 2) - 1;

    [Fact]
    public void Setup_SmallestLength_DrawsLengthValuesBelowHalfTheLength()
    {
        var nums = LongestConsecutiveSequenceWorkloads.BuildArray(SmallestLength, WorkloadSeed);

        Assert.Equal(SmallestLength, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, LowestFixtureValue, HighestFixtureValue));
    }

    [Fact]
    public void SortedScan_SmallestLength_CountsTheLongestConsecutiveRun()
    {
        var nums = LongestConsecutiveSequenceWorkloads.BuildArray(SmallestLength, WorkloadSeed);

        Assert.Equal(ExpectedLongestRun(nums), BuildHarness().SortedScan());
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
