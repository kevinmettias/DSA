namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 704 - a sorted array of even values, with the
// search target deliberately the last element so LinearScan is forced through its
// full worst-case pass instead of an early exit near the start making it look
// artificially competitive.
internal static class BinarySearchAlgorithmWorkloads
{
    // Spacing between consecutive generated values, so every value is even.
    private const int ValueStride = 2;

    public static int[] BuildSortedValues(int length) =>
        Enumerable.Range(0, length).Select(i => i * ValueStride).ToArray();

    public static int FarthestTarget(int length) => (length - 1) * ValueStride;
}
