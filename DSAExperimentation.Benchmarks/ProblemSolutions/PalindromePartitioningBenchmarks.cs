using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PalindromePartitioning;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromePartitioningSolution's, the same methods
// PalindromePartitioningSolutionTests proves correct. The original benchmark's two [Benchmark]
// arms (Baseline, PrimitiveComposed) were both compile-smoke placeholders (`=> 1`) -
// one real strategy, not two - so the pair here is the backtracking walk against the
// precomputed-table walk. The string's letters are drawn from a seeded Random over
// "ab": two letters rather than LC 131's 26 leave short palindromes everywhere, so
// the answer grows with the length as it does on LeetCode's own examples. Length
// stops at LC 131's 16-letter cap, past which the answer itself is exponential.
public class PalindromePartitioningBenchmarks
{
    private const int RandomSeed = 131; // LC problem number
    private const string Alphabet = "ab";

    private string _text = "";

    [Params(4, 8, 16)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _text = new string([.. SeededDraws.Values(Length, 0, Alphabet.Length, new Random(RandomSeed)).Select(index => Alphabet[index])]);

    [Benchmark(Baseline = true)]
    public List<List<string>> Backtracking() => PalindromePartitioningSolution.PartitionByBacktracking(_text);

    [Benchmark]
    public List<List<string>> PrecomputedPalindromeTable() =>
        PalindromePartitioningSolution.PartitionByPrecomputedPalindromeTable(_text);
}
