using DSAExperimentation.LeetCode.CountAnagrams;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Anagrams (LC 2514): brute-force permutation enumeration (HashSet-deduped,
// factorial-time per word) vs. this repo's modular factorial/inverse-factorial
// table (Domain.Modular.ModularArithmetic), which computes the exact same
// multinomial-coefficient product without ever materializing a permutation. A
// small 5-letter alphabet forces repeated letters within each word, exercising the
// inverse-factorial division path instead of degenerating to a plain n!.
//
// Sizes are per arm. Brute force would not finish past a word length of 7, so it
// stops there; the modular strategy never depends on word length exponentially and
// runs on to 4,000 letters a word, whose 20-word sentence (80,019 characters) stays
// inside LC 2514's bound of 10^5. The two are compared at the lengths both run.
public class CountAnagramsBenchmarks
{
    private const int RandomSeed = 2514; // LC problem number
    private const int WordCount = 20;
    private const string Alphabet = "abcde";

    private Dictionary<int, string> _sentenceByWordLength = [];

    public static IEnumerable<int> BruteForceSizes => [4, 7];

    public static IEnumerable<int> ModularSizes => [.. BruteForceSizes, 100, 4_000];

    // Every word length any arm runs is built here, outside the timed region, each from its
    // own generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _sentenceByWordLength = ModularSizes.ToDictionary(length => length, BuildSentence);

    private static string BuildSentence(int wordLength)
    {
        var random = new Random(RandomSeed);
        var words = Enumerable.Range(0, WordCount).Select(_ => BuildWord(random, wordLength));

        return string.Join(' ', words);
    }

    private static string BuildWord(Random random, int length)
        => new(Enumerable.Range(0, length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForcePermutations(int wordLength) =>
        CountAnagramsSolution.CountAnagramsByBruteForce(_sentenceByWordLength[wordLength]);

    [Benchmark]
    [ArgumentsSource(nameof(ModularSizes))]
    public long ModularFactorial(int wordLength) =>
        CountAnagramsSolution.CountAnagramsByModularFactorial(_sentenceByWordLength[wordLength]);
}
