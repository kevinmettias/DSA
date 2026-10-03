using DSAExperimentation.LeetCode.OrderlyQueue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OrderlyQueueSolution's, the same methods
// OrderlyQueueSolutionTests proves correct, measured on the movablePrefixLength == 1 half - comparing all n
// candidate rotations directly (O(n) rotations x O(n) comparison each) against
// building a SuffixArray over text + text once and reading off the first suffix start
// below text.Length (O(n log^2 n) per SuffixArray.cs's own doc comment). Length stops at
// LeetCode's own constraint (s.length <= 1000), which never leaves brute force's comfort
// zone: .NET's ordinal string compare is fast enough that the crossover only arrives
// around n in the tens of thousands, so at these sizes the two report numbers of the
// same order and the suffix array's asymptotic edge does not show. The
// movablePrefixLength > 1 case reduces to sorting text outright - already exercised
// against this repo's MergeSort by HIndex/ThreeSum/etc.'s own benchmarks, so it isn't
// repeated here.
public class OrderlyQueueBenchmarks
{
    private const int RandomSeed = 899; // LC problem number
    private const int AlphabetSize = 26;
    private const int RotationsOnly = 1;

    private string _text = "";

    [Params(100, 1_000)]
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
