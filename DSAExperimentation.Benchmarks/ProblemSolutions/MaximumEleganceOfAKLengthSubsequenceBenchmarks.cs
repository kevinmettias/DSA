using DSAExperimentation.LeetCode.MaximumEleganceOfAKLengthSubsequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumEleganceOfAKLengthSubsequenceSolution's, the
// same methods MaximumEleganceOfAKLengthSubsequenceSolutionTests proves correct against
// LeetCode's own examples. Categories are drawn from a pool much smaller than the
// item count so the scan past subsequenceLength actually walks the duplicates stack
// down instead of running out of duplicates after the very first swap. Categories are
// numbered from 1, as LC 2813's 1 <= categoryi <= n requires.
public class MaximumEleganceOfAKLengthSubsequenceBenchmarks
{
    private const int MaxProfitExclusive = 100_000;
    private const int CategoryPoolSize = 20;
    private const int FirstCategory = 1;
    private const int Seed = 1;

    private int[][] _items = [];

    private int _subsequenceLength;
    [Params(500, 5_000)]
    public int ItemCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _items = Enumerable.Range(0, ItemCount)
            .Select(_ => new[] { random.Next(1, MaxProfitExclusive), random.Next(FirstCategory, FirstCategory + CategoryPoolSize) })
            .ToArray();
        _subsequenceLength = ItemCount / 2;
    }

    [Benchmark(Baseline = true)]
    public long Bcl() =>
        MaximumEleganceOfAKLengthSubsequenceSolution.MaximumEleganceByBcl(_items, _subsequenceLength);

    [Benchmark]
    public long RepoPrimitives() =>
        MaximumEleganceOfAKLengthSubsequenceSolution.MaximumEleganceByRepoPrimitives(_items, _subsequenceLength);
}
