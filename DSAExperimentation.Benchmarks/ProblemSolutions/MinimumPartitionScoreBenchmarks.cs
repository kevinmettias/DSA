using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumPartitionScore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumPartitionScoreSolution's, the same
// methods MinimumPartitionScoreTests proves correct. K is fixed at a quarter
// of Length so the recursion always has real partitioning choices at every
// depth, not a nearly-forced split. Length stays modest: both arms are
// O(Length^2 * k) recursions over every candidate group boundary, and this
// is the axis whose state count they pay for.
[MemoryDiagnoser]
public class MinimumPartitionScoreBenchmarks
{
    private const int Seed = 3826; // LC problem number
    private const int MaxValueExclusive = 10_000;

    [Params(12, 20)]
    public int Length;

    private int[] _nums = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _k = Math.Max(1, Length / 4);
    }

    [Benchmark(Baseline = true)]
    public long DictionaryMemo() => MinimumPartitionScoreSolution.MinPartitionScoreByDictionaryMemo(_nums, _k);

    [Benchmark]
    public long MemoizedPartition() => MinimumPartitionScoreSolution.MinPartitionScoreByMemoizedPartition(_nums, _k);
}
