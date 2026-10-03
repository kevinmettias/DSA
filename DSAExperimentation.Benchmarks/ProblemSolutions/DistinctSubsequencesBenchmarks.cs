using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DistinctSubsequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistinctSubsequencesSolution's, the same methods
// DistinctSubsequencesSolutionTests proves correct. The strings come from
// DistinctSubsequencesWorkloads: a seeded target of English letters, and a source
// that is the target with three letters doubled, which keeps the count inside LC
// 115's 32-bit promise at every length while both recurrences still do their
// O(source * target) work. SourceLength stops at LC 115's 1,000-character cap.
public class DistinctSubsequencesBenchmarks
{
    private const int RandomSeed = 115; // LC problem number

    private SourceText _source;
    private TargetPattern _target;

    [Params(100, 1_000)]
    public int SourceLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (source, target) = DistinctSubsequencesWorkloads.Build(SourceLength, new Random(RandomSeed));
        _source = new SourceText(source);
        _target = new TargetPattern(target);
    }

    [Benchmark(Baseline = true)]
    public int MemoizedRecursion() =>
        DistinctSubsequencesSolution.CountDistinctSubsequencesByMemoizedRecursion(_source, _target);

    [Benchmark]
    public int IterativeTable() =>
        DistinctSubsequencesSolution.CountDistinctSubsequencesByIterativeTable(_source, _target);
}
