using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SearchInRotatedSortedArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SearchInRotatedSortedArrayIISolution's, the same
// methods SearchInRotatedSortedArrayIITests proves correct. RotatedSortedArrayWorkloads
// builds the array - rotated, with a bounded band of duplicates stamped across both
// ends, shared with LC 154's sibling harness - so the trim loop does real, but small,
// work relative to Length: large enough to exercise duplicate handling, small enough
// that the O(log n) win over LinearScan still shows. The construction's own pivot
// places the target inside the pre-rotation segment.
[MemoryDiagnoser]
public class SearchInRotatedSortedArrayIIBenchmarks
{
    private const int TargetDivisor = 2; // target sits halfway into the pre-rotation segment

    private int[] _values = [];
    private int _target;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (values, pivot) = RotatedSortedArrayWorkloads.RotatedWithDuplicateBoundaryBand(Length);
        _values = values;
        _target = pivot / TargetDivisor;
    }

    [Benchmark(Baseline = true)]
    public bool HasTargetByLinearScan() => SearchInRotatedSortedArrayIISolution.HasTargetByLinearScan(_values, _target);

    [Benchmark]
    public bool HasTargetByTrimDuplicatesThenBinarySearch() =>
        SearchInRotatedSortedArrayIISolution.HasTargetByTrimDuplicatesThenBinarySearch(_values, _target);
}
