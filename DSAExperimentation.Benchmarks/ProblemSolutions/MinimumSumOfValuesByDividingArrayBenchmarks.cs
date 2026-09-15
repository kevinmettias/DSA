using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumSumOfValuesByDividingArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumSumOfValuesByDividingArraySolution's,
// the same methods MinimumSumOfValuesByDividingArrayTests proves correct.
// [GlobalSetup] cuts nums at random points and reads each resulting group's
// AND back off as andValues, so every workload has at least one guaranteed
// feasible partition - the search explores real candidate splits instead of
// bailing out on "impossible" immediately. Length stays modest: the
// Dictionary-memo baseline is still a recursion over every candidate group
// boundary, and this is the axis whose state count that recursion pays for.
[MemoryDiagnoser]
public class MinimumSumOfValuesByDividingArrayBenchmarks
{
    private const int Seed = 3117; // LC problem number
    private const int MaxValueExclusive = 1 << 17;
    private const int GroupCount = 4;

    private int[] _nums = [];

    private int[] _andValues = [];
    [Params(12, 20)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxValueExclusive)).ToArray();
        _andValues = BuildFeasibleAndValues(_nums, GroupCount, random);
    }

    // Picks GroupCount - 1 random cut points, then reads each resulting
    // group's own AND back off nums - the workload's andValues are the exact
    // targets that partition already satisfies.
    private static int[] BuildFeasibleAndValues(int[] nums, int groupCount, Random random)
    {
        var cutCount = Math.Min(groupCount, nums.Length) - 1;
        var cuts = Enumerable.Range(1, nums.Length - 1)
            .OrderBy(_ => random.Next())
            .Take(cutCount)
            .Order()
            .ToArray();

        var boundaries = cuts.Append(nums.Length).ToArray();
        var andValues = new int[boundaries.Length];
        var start = 0;

        for (var i = 0; i < boundaries.Length; i++)
        {
            andValues[i] = GroupAnd(nums, start, boundaries[i]);
            start = boundaries[i];
        }

        return andValues;
    }

    // The AND of nums[start..end), which is exactly the value that group must have
    // for the cut at `end` to be feasible.
    private static int GroupAnd(int[] nums, int start, int end)
    {
        var groupAnd = nums[start];

        for (var j = start + 1; j < end; j++)
        {
            groupAnd &= nums[j];
        }

        return groupAnd;
    }

    [Benchmark(Baseline = true)]
    public long DictionaryMemo() =>
        MinimumSumOfValuesByDividingArraySolution.MinimumValueSumByDictionaryMemo(_nums, _andValues);

    [Benchmark]
    public long MemoizedPartition() =>
        MinimumSumOfValuesByDividingArraySolution.MinimumValueSumByMemoizedPartition(_nums, _andValues);
}
