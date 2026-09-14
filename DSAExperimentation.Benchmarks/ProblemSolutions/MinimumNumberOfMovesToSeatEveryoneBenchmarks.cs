using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfMovesToSeatEveryone;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfMovesToSeatEveryoneSolution's, the
// same methods MinimumNumberOfMovesToSeatEveryoneTests proves correct - an O(n^2)
// selection sort of both arrays against this repo's own O(n log n) MergeSort over
// ArrayIndexedSequence, each followed by the same index-wise distance sum.
[MemoryDiagnoser]
public class MinimumNumberOfMovesToSeatEveryoneBenchmarks
{
    // LC problem number, reused as the deterministic random seed.
    private const int RandomSeed = 2037;

    // Exclusive upper bound on a generated seat or student position.
    private const int MaxSeatPosition = 1_000_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _seats = null!;
    private int[] _students = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _seats = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxSeatPosition)).ToArray();
        _students = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxSeatPosition)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SelectionSortPairSum() =>
        MinimumNumberOfMovesToSeatEveryoneSolution.MinMovesToSeatBySelectionSort(_seats, _students);

    [Benchmark]
    public int MergeSortPairSum() =>
        MinimumNumberOfMovesToSeatEveryoneSolution.MinMovesToSeatByMergeSort(_seats, _students);
}
