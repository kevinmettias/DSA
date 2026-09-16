using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindMinimumInRotatedSortedArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMinimumInRotatedSortedArrayIISolution's, the
// same methods FindMinimumInRotatedSortedArrayIITests proves correct. The array is
// built by RotatedSortedArrayWorkloads - a rotated sequence with a bounded band of
// duplicate values stamped across both ends - so the shrink loop's tie-breaking does
// real, but small, work relative to Length. LC 81's sibling harness shares it.
[MemoryDiagnoser]
public class FindMinimumInRotatedSortedArrayIIBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = RotatedSortedArrayWorkloads.RotatedWithDuplicateBoundaryBand(Length).Values;

    [Benchmark(Baseline = true)]
    public int LinearScan() => FindMinimumInRotatedSortedArrayIISolution.FindMinByLinearScan(_values);

    [Benchmark]
    public int DuplicateTolerantBinaryShrink() =>
        FindMinimumInRotatedSortedArrayIISolution.FindMinByDuplicateTolerantBinaryShrink(_values);
}
