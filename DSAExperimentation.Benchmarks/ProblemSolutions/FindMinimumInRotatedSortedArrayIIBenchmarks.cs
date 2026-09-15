using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindMinimumInRotatedSortedArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMinimumInRotatedSortedArrayIISolution's, the
// same methods FindMinimumInRotatedSortedArrayIITests proves correct. A bounded
// band of duplicate values is stamped across both ends so the shrink loop's
// tie-breaking does real, but small, work relative to Length - the same
// construction SearchInRotatedSortedArrayIIBenchmarks uses for LC 81's sibling
// duplicate-tolerant problem.
[MemoryDiagnoser]
public class FindMinimumInRotatedSortedArrayIIBenchmarks
{
    private const int PivotDivisor = 3; // rotation pivot is one third of the way into the array
    private const int DuplicateSpanDivisor = 8; // fraction of Length stamped with duplicate boundary values
    private const int MaxDuplicateSpan = 40; // upper bound on how many boundary elements are duplicated

    private int[] _values = [];

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
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => FindMinimumInRotatedSortedArrayIISolution.FindMinByLinearScan(_values);

    [Benchmark]
    public int DuplicateTolerantBinaryShrink() =>
        FindMinimumInRotatedSortedArrayIISolution.FindMinByDuplicateTolerantBinaryShrink(_values);
}
