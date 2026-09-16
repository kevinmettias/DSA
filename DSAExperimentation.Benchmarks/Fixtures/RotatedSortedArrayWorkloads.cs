namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for the two duplicate-tolerant rotated-array siblings -
// LC 154 (find the minimum) and LC 81 (search a target). 0..length-1 is rotated left
// by a third and a bounded band of duplicate values is then stamped across both ends,
// so duplicate handling does real, but small, work relative to length: large enough
// that ties matter, small enough that the O(log n) win over a linear scan still shows.
internal static class RotatedSortedArrayWorkloads
{
    private const int PivotDivisor = 3; // rotation pivot is one third of the way into the array
    private const int DuplicateSpanDivisor = 8; // fraction of length stamped with duplicate boundary values
    private const int MaxDuplicateSpan = 40; // upper bound on how many boundary elements are duplicated

    // The pivot comes back with the array because LC 81's harness aims its target
    // inside the pre-rotation segment, which only this construction knows.
    public static (int[] Values, int Pivot) RotatedWithDuplicateBoundaryBand(int length)
    {
        var sorted = Enumerable.Range(0, length).ToArray();
        var pivot = length / PivotDivisor;
        var rotated = sorted[pivot..].Concat(sorted[..pivot]).ToArray();

        var duplicateSpan = Math.Min(length / DuplicateSpanDivisor, MaxDuplicateSpan);
        for (var i = 0; i < duplicateSpan; i++)
        {
            rotated[i] = rotated[0];
            rotated[^(i + 1)] = rotated[0];
        }

        return (rotated, pivot);
    }
}
