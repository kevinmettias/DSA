using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountAnagrams;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Anagrams (LC 2514): brute-force permutation enumeration (HashSet-deduped,
// factorial-time per word) vs. this repo's modular factorial/inverse-factorial
// table (Domain.Modular.ModularArithmetic), which computes the exact same
// multinomial-coefficient product without ever materializing a permutation. A
// small 5-letter alphabet forces repeated letters within each word, exercising the
// inverse-factorial division path instead of degenerating to a plain n!.
// WordLength stays small (brute force would not finish otherwise); the modular
// strategy scales to any length because it never depends on word length
// exponentially.
[MemoryDiagnoser]
public class CountAnagramsBenchmarks
{
    private const int RandomSeed = 2514; // LC problem number
    private const int WordCount = 20;
    private const string Alphabet = "abcde";

    private string _sentence = "";

    [Params(4, 7)]
    public int WordLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var words = Enumerable.Range(0, WordCount).Select(_ => BuildWord(random, WordLength));
        _sentence = string.Join(' ', words);
    }

    private static string BuildWord(Random random, int length)
        => new(Enumerable.Range(0, length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());

    [Benchmark(Baseline = true)]
    public long BruteForcePermutations() => CountAnagramsSolution.CountAnagramsByBruteForce(_sentence);

    [Benchmark]
    public long ModularFactorial() => CountAnagramsSolution.CountAnagramsByModularFactorial(_sentence);
}
