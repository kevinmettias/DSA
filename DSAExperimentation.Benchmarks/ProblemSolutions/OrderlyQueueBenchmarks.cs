using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.OrderlyQueue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OrderlyQueueSolution's, the same methods
// OrderlyQueueTests proves correct, measured on the movablePrefixLength == 1 half - comparing all n
// candidate rotations directly (O(n) rotations x O(n) comparison each) against
// building a SuffixArray over text + text once and reading off the first suffix start
// below text.Length (O(n log^2 n) per SuffixArray.cs's own doc comment). LeetCode's own
// constraint (s.length <= 1000) never leaves brute force's comfort zone - .NET's
// ordinal string compare is fast enough that the crossover only arrives around n
// in the tens of thousands, well past that bound - so Length is deliberately set
// past it (measured locally: brute force ~400ms vs. SuffixArray ~45ms at 50k,
// ~1.6s vs. ~0.1s at 100k) to actually exercise the O(n^2) vs. O(n log^2 n) gap
// this composition buys, rather than reporting a same-order-of-magnitude number at
// LeetCode's own input size. The movablePrefixLength > 1 case reduces to sorting text outright -
// already exercised against this repo's MergeSort by HIndex/ThreeSum/etc.'s own
// benchmarks, so it isn't repeated here.
[MemoryDiagnoser]
public class OrderlyQueueBenchmarks
{
    private const int RandomSeed = 899; // LC problem number
    private const int AlphabetSize = 26;
    private const int RotationsOnly = 1;

    private string _text = "";

    [Params(50_000, 100_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _text = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string BruteForceAllRotations() => OrderlyQueueSolution.SmallestStringByBruteForceRotations(_text, RotationsOnly);

    [Benchmark]
    public string SuffixArraySmallestRotation() => OrderlyQueueSolution.SmallestStringBySuffixArray(_text, RotationsOnly);
}
