using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.PeakIndexInAMountainArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PeakIndexInAMountainArraySolution's, the same
// methods PeakIndexInAMountainArrayTests proves correct. The mountain is built
// once in [GlobalSetup] with its peak at the midpoint, the worst case for the
// linear scan and a neutral one for the binary search.
[MemoryDiagnoser]
public class PeakIndexInAMountainArrayBenchmarks
{

    private int[] _mountain = [];

    [Params(200, 100_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _mountain = new int[Length];
        var peak = Length / AlgorithmConstants.HalvingFactor;

        for (var i = 0; i < Length; i++)
        {
            _mountain[i] = i <= peak ? i : DescendingHeight(Length, i);
        }
    }

    // The mountain's descending side: the height a distance past the peak mirrors
    // the ascent's.
    private static int DescendingHeight(int length, int index) => length - index;

    [Benchmark(Baseline = true)]
    public int LinearScan() => PeakIndexInAMountainArraySolution.PeakIndexByLinearScan(_mountain);

    [Benchmark]
    public int BinarySearchLowerBound() =>
        PeakIndexInAMountainArraySolution.PeakIndexByBinarySearchLowerBound(_mountain);
}
