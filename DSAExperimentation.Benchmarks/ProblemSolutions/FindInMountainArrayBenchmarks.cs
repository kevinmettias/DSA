using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.FindInMountainArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindInMountainArraySolution's, the same methods
// FindInMountainArraySolutionTests proves correct. The ascending slope holds only even
// values and the descending slope, which starts one below the peak, only odd ones, so
// _target (second from the end) appears nowhere on the ascending half and its only
// index is Length - 2: the linear scan is forced through nearly the whole array on
// every invocation, instead of an early exit making it look artificially competitive.
// The last value is 3, inside LC 1095's non-negative values, and LC caps the mountain
// at 10^4 values, so the larger Length is that cap.
public class FindInMountainArrayBenchmarks
{
    private const int AscendingStep = 2;
    private const int DescendingStep = 2;
    private const int TargetOffsetFromEnd = 2;

    private int[] _mountain = [];

    private int _target;

    [Params(1_000, 10_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var peakIndex = Length / AlgorithmConstants.HalvingFactor;
        _mountain = new int[Length];

        for (var i = 0; i <= peakIndex; i++)
        {
            _mountain[i] = AscendingStep * i;
        }

        _mountain[peakIndex + 1] = _mountain[peakIndex] - 1;

        for (var i = peakIndex + 2; i < Length; i++)
        {
            _mountain[i] = _mountain[i - 1] - DescendingStep;
        }

        _target = _mountain[Length - TargetOffsetFromEnd];
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => FindInMountainArraySolution.FindIndexByLinearScan(_mountain, _target);

    [Benchmark]
    public int PeakBisectionThenBinarySearch() =>
        FindInMountainArraySolution.FindIndexByPeakBisection(_mountain, _target);
}
