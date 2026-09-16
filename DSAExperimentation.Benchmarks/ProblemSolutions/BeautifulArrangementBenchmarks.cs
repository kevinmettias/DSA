using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BeautifulArrangement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BeautifulArrangementSolution's, the same classes
// BeautifulArrangementTests proves correct - generate every full permutation of
// [1..n] and reject it afterward (O(n!) permutations, each fully materialized
// regardless of how early it violates the rule) vs. this repo's own
// Backtrack.Search with the v % position == 0 / position % v == 0 rule folded
// directly into Candidates, so an illegal value is never placed and the branch
// is pruned immediately instead of discovered n steps later.
[MemoryDiagnoser]
public class BeautifulArrangementBenchmarks
{
    [Params(6, 8)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public int GenerateThenFilter() => BeautifulArrangementSolution.CountByGenerateThenFilter(Size);

    [Benchmark]
    public int PrunedBacktracking() => BeautifulArrangementSolution.CountByPrunedBacktracking(Size);
}
