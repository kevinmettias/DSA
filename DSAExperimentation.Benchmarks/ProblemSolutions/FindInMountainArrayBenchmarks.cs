using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.FindInMountainArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindInMountainArraySolution's, the same methods
// FindInMountainArrayTests proves correct. The ascending slope holds only even
// values and the descending slope only odd ones, so _target (deep in the
// descending slope) can never resolve early via the ascending half - the linear
// scan is forced through nearly the whole array on every invocation, instead of an
// early exit making it look artificially competitive.
[MemoryDiagnoser]
public class FindInMountainArrayBenchmarks
{
    private const int AscendingStep = 2;
    private const int DescendingStep = 2;
    private const int TargetOffsetFromEnd = 2;

    private int[] _mountain = [];

    private int _target;
    [Params(1_000, 100_000)]
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

        for (var i = peakIndex + 1; i < Length; i++)
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
