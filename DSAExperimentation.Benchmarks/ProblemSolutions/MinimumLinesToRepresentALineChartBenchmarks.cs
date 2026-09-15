using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumLinesToRepresentALineChart;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumLinesToRepresentALineChartSolution's, the same
// methods MinimumLinesToRepresentALineChartTests proves correct - the BCL's Array.Sort
// plus floating-point slope division against this repo's own MergeSort over
// ArrayIndexedSequence plus the exact cross-product test. [GlobalSetup] generates the
// point set in LeetCode's own jagged int[][] shape, so both arms are handed the prepared
// input directly and only the sort-and-count is measured.
[MemoryDiagnoser]
public class MinimumLinesToRepresentALineChartBenchmarks
{
    private const int RandomSeed = 2280;
    private const int PriceBound = 100_000;

    private int[][] _points = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // A shuffled 0..Length-1 range guarantees exactly Length distinct days, so every
        // generated input actually has Length points instead of silently collapsing
        // duplicates.
        var days = Enumerable.Range(0, Length).ToArray();
        for (var i = days.Length - 1; i > 0; i--)
        {
            var swapIndex = random.Next(i + 1);
            (days[i], days[swapIndex]) = (days[swapIndex], days[i]);
        }

        _points = [.. days.Select(day => new[] { day, random.Next(0, PriceBound) })];
    }

    [Benchmark(Baseline = true)]
    public int ArraySortFloatingSlope() =>
        MinimumLinesToRepresentALineChartSolution.MinimumLinesByArraySortFloatingSlope(_points);

    [Benchmark]
    public int MergeSortIntegerSlope() =>
        MinimumLinesToRepresentALineChartSolution.MinimumLinesByMergeSortIntegerSlope(_points);
}
