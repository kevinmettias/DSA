using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TypeOfTriangle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TypeOfTriangleSolution's, the same methods
// TypeOfTriangleTests proves correct. LC 3024 fixes nums.Length == 3, so there is
// no input size to scale with [Params] here - both arms are O(1) regardless, and
// the comparison is purely the constant-factor cost of a few pairwise comparisons
// against copying three elements into MergeSort's own indexed sequence.
[MemoryDiagnoser]
public class TypeOfTriangleBenchmarks
{
    private static readonly int[] Sides = [3, 4, 5];

    [Benchmark(Baseline = true)]
    public string DirectComparison() => TypeOfTriangleSolution.ClassifyByDirectComparison(Sides);

    [Benchmark]
    public string MergeSort() => TypeOfTriangleSolution.ClassifyByMergeSort(Sides);
}
