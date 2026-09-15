using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Trie;
using DSAExperimentation.LeetCode.LongestCommonSuffixQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestCommonSuffixQueriesSolution's, the same
// methods LongestCommonSuffixQueriesTests proves correct. The trie arm is handed an
// already-built Trie<int>, so container insertion is charged to [GlobalSetup] and
// only query answering is measured.
[MemoryDiagnoser]
public class LongestCommonSuffixQueriesBenchmarks
{
    private const int Seed = 3093;
    private const int MaxWordLength = 10;
    private const string Alphabet = "ab";

    private string[] _wordsContainer = [];

    private string[] _wordsQuery = [];
    private Trie<int> _trie = new();
    [Params(30, 200)]
    public int ContainerSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _wordsContainer = BuildWords(random, ContainerSize);
        _wordsQuery = BuildWords(random, ContainerSize);
        _trie = LongestCommonSuffixQueriesSolution.BuildSuffixTrie(_wordsContainer);
    }

    private static string[] BuildWords(Random random, int count)
    {
        var words = new string[count];

        for (var i = 0; i < count; i++)
        {
            var length = random.Next(1, MaxWordLength + 1);
            var chars = new char[length];

            for (var j = 0; j < length; j++)
            {
                chars[j] = Alphabet[random.Next(Alphabet.Length)];
            }

            words[i] = new string(chars);
        }

        return words;
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        LongestCommonSuffixQueriesSolution.FindIndicesByBruteForce(_wordsContainer, _wordsQuery);

    [Benchmark]
    public int[] Trie() =>
        LongestCommonSuffixQueriesSolution.FindIndicesByTrie(_trie, _wordsQuery);
}
