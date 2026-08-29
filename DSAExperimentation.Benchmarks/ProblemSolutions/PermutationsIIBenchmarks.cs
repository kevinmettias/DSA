using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class PermutationsIIBenchmarks
{
    [Benchmark(Baseline = true)] public int SpecializedUnique() => Count([1, 1, 2, 2, 3, 3]);
    [Benchmark] public int SameDedupSearchShape() => Count([1, 1, 2, 2, 3, 3]);
    private static int Count(int[] nums) { Array.Sort(nums); var used = new bool[nums.Length]; var count = 0; void Search(int depth) { if (depth == nums.Length) { count++; return; } for (var i = 0; i < nums.Length; i++) if (!used[i] && (i == 0 || nums[i] != nums[i - 1] || used[i - 1])) { used[i] = true; Search(depth + 1); used[i] = false; } } Search(0); return count; }
}
