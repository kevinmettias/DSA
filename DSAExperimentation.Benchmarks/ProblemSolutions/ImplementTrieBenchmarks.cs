using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ImplementTrie;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ImplementTrieSolution's, composing this repo's own
// DataStructures.Trie.Trie<bool> - the same primitive TrieReturnedBySolutionSeamTests pins the
// solution to returning. There is no second arm because there is no second strategy: the repo
// deliberately composes its own trie rather than inventing a representation, so a rival node-chain
// trie written here would time a structure the architecture does not have. BinarySearchTreeIterator
// is the same case. [Benchmark] inserts the whole word set, then replays search and startsWith over
// a shuffled copy of it, so insert cost is charged to both halves rather than just the first.
[MemoryDiagnoser]
public class ImplementTrieBenchmarks
{
    private const int RandomSeed = 208; // LC problem number
    private const int WordLength = 8;
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

    private string[] _words = [];
    private string[] _queries = [];
    private int _prefixLength;

    [Params(1_000, 10_000)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _words = Enumerable.Range(0, WordCount).Select(_ => BuildWord(random)).ToArray();
        _queries = [.. _words.OrderBy(_ => random.Next())];
        _prefixLength = WordLength / 2;
    }

    [Benchmark(Baseline = true)]
    public int InsertThenSearchAndStartsWith()
    {
        var trie = ImplementTrieSolution.CreateByTriePrimitive();
        var hits = 0;

        foreach (var word in _words)
        {
            trie.Set(word, true);
        }

        foreach (var word in _queries)
        {
            hits += trie.HasKey(word) ? 1 : 0;
            hits += trie.HasPrefix(word[.._prefixLength]) ? 1 : 0;
        }

        return hits;
    }

    private static string BuildWord(Random random)
    {
        var letters = new char[WordLength];

        for (var index = 0; index < WordLength; index++)
        {
            letters[index] = Alphabet[random.Next(Alphabet.Length)];
        }

        return new string(letters);
    }
}
