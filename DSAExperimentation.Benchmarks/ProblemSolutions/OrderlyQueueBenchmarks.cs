using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.OrderlyQueue.OrderlyQueueSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OrderlyQueueSolution's, the same methods
// OrderlyQueueTests proves correct, measured on the k == 1 half - comparing all n
// candidate rotations directly (O(n) rotations x O(n) comparison each) against
// building a SuffixArray over s + s once and reading off the first suffix start
// below n (O(n log^2 n) per SuffixArray.cs's own doc comment). LeetCode's own
// constraint (s.length <= 1000) never leaves brute force's comfort zone - .NET's
// ordinal string compare is fast enough that the crossover only arrives around n
// in the tens of thousands, well past that bound - so Length is deliberately set
// past it (measured locally: brute force ~400ms vs. SuffixArray ~45ms at 50k,
// ~1.6s vs. ~0.1s at 100k) to actually exercise the O(n^2) vs. O(n log^2 n) gap
// this composition buys, rather than reporting a same-order-of-magnitude number at
// LeetCode's own input size. The k > 1 case reduces to sorting s outright -
// already exercised against this repo's MergeSort by HIndex/ThreeSum/etc.'s own
// benchmarks, so it isn't repeated here.
[MemoryDiagnoser]
public class OrderlyQueueBenchmarks
{
    private const int RandomSeed = 899; // LC problem number
    private const int AlphabetSize = 26;
    private const int RotationsOnly = 1;

    [Params(50_000, 100_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _s = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string BruteForceAllRotations() => SmallestStringByBruteForceRotations(_s, RotationsOnly);

    [Benchmark]
    public string SuffixArraySmallestRotation() => SmallestStringBySuffixArray(_s, RotationsOnly);
}
