using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumPartitionScore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumPartitionScoreSolution's, the same
// methods MinimumPartitionScoreSolutionTests proves correct. GroupCount is fixed at a
// quarter of Length so the recursion always has real partitioning choices at
// every depth, not a nearly-forced split. Length stays modest: both arms are
// O(Length^2 * k) recursions over every candidate group boundary, and this
// is the axis whose state count they pay for.
public class MinimumPartitionScoreBenchmarks
{
    private const int Seed = 3826; // LC problem number
    private const int MaxValueExclusive = 10_000;

    private int[] _nums = [];

    private int _groupCount;
    [Params(12, 20)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = SeededDraws.Values(Length, 1, MaxValueExclusive, random);
        _groupCount = Math.Max(1, Length / 4);
    }

    [Benchmark(Baseline = true)]
    public long DictionaryMemo() =>
        MinimumPartitionScoreSolution.MinPartitionScoreByDictionaryMemo(_nums, _groupCount);

    [Benchmark]
    public long MemoizedPartition() =>
        MinimumPartitionScoreSolution.MinPartitionScoreByMemoizedPartition(_nums, _groupCount);
}
