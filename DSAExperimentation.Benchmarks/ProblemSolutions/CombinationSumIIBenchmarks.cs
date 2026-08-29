using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class CombinationSumIIBenchmarks
{
    [Benchmark(Baseline = true)] public int SortAndBacktrackSpecialized() => Count([10, 1, 2, 7, 6, 1, 5], 8);
    [Benchmark] public int SameSearchShape() => Count([10, 1, 2, 7, 6, 1, 5], 8);
    private static int Count(int[] candidates, int target) { Array.Sort(candidates); var count = 0; void Search(int start, int sum) { if (sum == target) { count++; return; } for (var i = start; i < candidates.Length; i++) { if (i > start && candidates[i] == candidates[i - 1]) continue; if (sum + candidates[i] <= target) Search(i + 1, sum + candidates[i]); } } Search(0, 0); return count; }
}
