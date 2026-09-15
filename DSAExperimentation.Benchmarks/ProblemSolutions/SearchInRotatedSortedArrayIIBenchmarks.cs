using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SearchInRotatedSortedArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SearchInRotatedSortedArrayIISolution's, the same
// methods SearchInRotatedSortedArrayIITests proves correct. A bounded band of
// duplicate values is stamped across both ends so the trim loop does real, but
// small, work relative to Length - large enough to exercise duplicate handling,
// small enough that the O(log n) win over LinearScan still shows.
[MemoryDiagnoser]
public class SearchInRotatedSortedArrayIIBenchmarks
{
    private const int PivotDivisor = 3; // rotation pivot is one third of the way into the array
    private const int DuplicateSpanDivisor = 8; // fraction of Length stamped with duplicate boundary values
    private const int MaxDuplicateSpan = 40; // upper bound on how many boundary elements are duplicated
    private const int TargetDivisor = 2; // target sits halfway into the pre-rotation segment

    private int[] _values = [];
    private int _target;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var sorted = Enumerable.Range(0, Length).ToArray();
        var pivot = Length / PivotDivisor;
        var rotated = sorted[pivot..].Concat(sorted[..pivot]).ToArray();

        var duplicateSpan = Math.Min(Length / DuplicateSpanDivisor, MaxDuplicateSpan);
        for (var i = 0; i < duplicateSpan; i++)
        {
            rotated[i] = rotated[0];
            rotated[^(i + 1)] = rotated[0];
        }

        _values = rotated;
        _target = pivot / TargetDivisor;
    }

    [Benchmark(Baseline = true)]
    public bool LinearScan() => SearchInRotatedSortedArrayIISolution.SearchByLinearScan(_values, _target);

    [Benchmark]
    public bool TrimDuplicatesThenBinarySearch() =>
        SearchInRotatedSortedArrayIISolution.SearchByTrimDuplicatesThenBinarySearch(_values, _target);
}
