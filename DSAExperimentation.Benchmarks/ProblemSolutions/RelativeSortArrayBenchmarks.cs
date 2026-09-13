using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RelativeSortArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RelativeSortArraySolution's, the same methods
// RelativeSortArrayTests proves correct. _arr1 is half values drawn from _arr2
// (exercises the ranked branch) and half values guaranteed outside _arr2's range
// (exercises the unranked, sort-by-value-ascending branch and forces every
// LinearScanComparerSort miss through a full m-length scan). With ReferenceLength
// (m) fixed at 2,000, a BenchmarkDotNet --job Dry run shows the expected crossover:
// LinearScanComparerSort ahead 2.63x at Length=200 (n log n * m hasn't yet outgrown
// Array.Sort's much lower per-comparison constant factor), then HashMapMergeSort
// ahead ~2.5x at Length=5,000, as the O(n log n * m) scan cost overtakes HashMap's
// O(1)-lookup advantage - the same small-n-overhead-then-crossover shape several
// other composed-vs-naive benchmarks in this repo show.
[MemoryDiagnoser]
public class RelativeSortArrayBenchmarks
{
    private const int ReferenceLength = 2_000;
    private const int RandomSeed = 1122; // LC problem number
    private const int Arr2ValueStep = 2; // _arr2 holds every other integer
    private const int CoinFlipBound = 2; // random.Next(0, CoinFlipBound) == 0 is a 50/50 draw
    private const int OutOfReferenceRangeMin = 100_000;
    private const int OutOfReferenceRangeMax = 200_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _arr1 = null!;
    private int[] _arr2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr2 = Enumerable.Range(0, ReferenceLength).Select(i => i * Arr2ValueStep).ToArray();
        _arr1 = Enumerable.Range(0, Length)
            .Select(_ => random.Next(0, CoinFlipBound) == 0
                ? _arr2[random.Next(_arr2.Length)]
                : random.Next(OutOfReferenceRangeMin, OutOfReferenceRangeMax))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] LinearScanComparerSort() =>
        RelativeSortArraySolution.RelativeSortByLinearScanComparer(_arr1, _arr2);

    [Benchmark]
    public int[] HashMapMergeSort() =>
        RelativeSortArraySolution.RelativeSortByHashMapMergeSort(_arr1, _arr2);
}
