using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestConsecutiveSequenceBenchmarks (ARCHITECTURE 17.9): both arms are
// competing strategies for the same question - this repo's set-based run expansion and a sorted
// scan - so a harness whose arms disagree is timing two different problems, and the oracle below
// derives the answer independently. Setup's workload comes from
// LongestConsecutiveSequenceWorkloads, which is seeded and so reproducible: the same Length
// rebuilds the same array, whose values all lie in [0, Length / 2). The expected longest run is
// recomputed here by taking the distinct values, ordering them and walking once - close in spirit
// to the sorted arm, so the arms' agreement test is what actually pins the two strategies together.
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
            AnswerGraphText.Of(BuildHarness().SetRunExpansion()),
            AnswerGraphText.Of(BuildHarness().SetRunExpansion()));
    }

    [Fact]
    public void SetRunExpansion_SmallestLength_CountsTheLongestConsecutiveRun()
    {
        var nums = LongestConsecutiveSequenceWorkloads.BuildArray(SmallestLength, WorkloadSeed);

        Assert.Equal(ExpectedLongestRun(nums), BuildHarness().SetRunExpansion());
    }

    [Fact]
    public void SortedScan_SmallestLength_CountsTheLongestConsecutiveRun()
    {
        var nums = LongestConsecutiveSequenceWorkloads.BuildArray(SmallestLength, WorkloadSeed);

        Assert.Equal(ExpectedLongestRun(nums), BuildHarness().SortedScan());
    }

    [Fact]
    public void SortedScan_AgreesWithSetRunExpansion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SetRunExpansion(), harness.SortedScan());
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
