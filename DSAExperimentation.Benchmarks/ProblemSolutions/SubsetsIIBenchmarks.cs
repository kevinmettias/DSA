using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubsetsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the arm is SubsetsIISolution's, the same method SubsetsIITests
// proves correct. The [Benchmark(Baseline = true)] "IterativeDedup" arm this
// migrated out of never implemented an independent algorithm - its body built an
// unused HashSet<string>, looped over a discarded HashSet<int> doing nothing, and
// then returned Backtracking()'s own value directly. It was not a second strategy,
// just dead code wrapping the one real arm, so it is not preserved here.
[MemoryDiagnoser]
public class SubsetsIIBenchmarks
{
    private const int DuplicateGroupSize = 2;

    private int[] _values = [];

    [Params(10, 14)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i / DuplicateGroupSize).ToArray();

    [Benchmark(Baseline = true)]
    public List<List<int>> BacktrackSkipDuplicates() =>
        SubsetsIISolution.FindAllSubsetsByBacktrackSkipDuplicates(_values);
}
