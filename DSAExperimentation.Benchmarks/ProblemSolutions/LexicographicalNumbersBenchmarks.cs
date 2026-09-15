using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LexicographicalNumbers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LexicographicalNumbersSolution's, the same methods
// LexicographicalNumbersTests proves correct.
[MemoryDiagnoser]
public class LexicographicalNumbersBenchmarks
{
    [Params(1_000, 500_000)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public List<int> StringSort() => LexicographicalNumbersSolution.LexicalOrderByStringSort(N);

    [Benchmark]
    public List<int> DepthFirstDigitTree() => LexicographicalNumbersSolution.LexicalOrderByDepthFirstDigitTree(N);
}
