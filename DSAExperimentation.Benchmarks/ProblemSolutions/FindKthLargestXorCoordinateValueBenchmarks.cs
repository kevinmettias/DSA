using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindKthLargestXorCoordinateValue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindKthLargestXorCoordinateValueSolution's, the same
// methods FindKthLargestXorCoordinateValueSolutionTests proves correct. They share the same
// 2D prefix-XOR pass and differ only in the selection that follows it - a full
// O(nm log nm) sort against an O(nm log k) size-k min-heap - so the matrix itself is
// built once in [GlobalSetup] rather than charged to either arm.
public class FindKthLargestXorCoordinateValueBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 1738; // LC problem number
    private const int MaxCoordinateValueExclusive = 1_000_000;

    private int[][] _matrix = [];

    [Params(50, 300)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = Enumerable.Range(0, Side)
            .Select(_ => SeededDraws.Values(Side, 1, MaxCoordinateValueExclusive, random))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullSort() =>
        FindKthLargestXorCoordinateValueSolution.KthLargestValueByFullSort(_matrix, K);

    [Benchmark]
    public int SizeKMinHeap() =>
        FindKthLargestXorCoordinateValueSolution.KthLargestValueBySizeKHeap(_matrix, K);
}
