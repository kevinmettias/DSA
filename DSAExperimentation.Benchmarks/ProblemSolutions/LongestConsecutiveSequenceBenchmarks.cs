using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LongestConsecutiveSequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestConsecutiveSequenceSolution's, the same
// methods LongestConsecutiveSequenceSolutionTests proves correct. The set arm hashes each
// value once as it walks the runs; the sorted arm instead pays an O(n log n) sort
// of a full copy before its single linear scan over the ordered values.
public class LongestConsecutiveSequenceBenchmarks
{
    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = LongestConsecutiveSequenceWorkloads.BuildArray(Length, seed: 128);

    [Benchmark(Baseline = true)]
    public int SetRunExpansion() => LongestConsecutiveSequenceSolution.LongestConsecutiveBySetRunExpansion(_nums);

    [Benchmark]
    public int SortedScan() => LongestConsecutiveSequenceSolution.LongestConsecutiveBySortedScan(_nums);
}
