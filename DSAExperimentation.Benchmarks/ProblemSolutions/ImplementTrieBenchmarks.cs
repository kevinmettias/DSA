using DSAExperimentation.LeetCode.ImplementTrie;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ImplementTrieSolution's, composing this repo's own
// DataStructures.Trie.Trie<bool> - the same primitive TrieReturnedBySolutionSeamTests pins the
// solution to returning. There is no second arm because there is no second strategy: the repo
// deliberately composes its own trie rather than inventing a representation, so a rival node-chain
// trie written here would time a structure the architecture does not have. BinarySearchTreeIterator
// is the same case. [Benchmark] inserts the whole word set, then replays search and startsWith over
// a shuffled copy of it, so insert cost is charged to both halves rather than just the first, and
// returns every verdict, each query's search then its startsWith.
public class ImplementTrieBenchmarks
{
    private const int RandomSeed = 208; // LC problem number
    private const int WordLength = 8;
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

    // Each query asks search and then startsWith.
    private const int VerdictsPerQuery = 2;

    private string[] _words = [];
    private string[] _queries = [];
    private int _prefixLength;

    // Every search and startsWith verdict, in call order; sized in setup so the replay allocates nothing.
    private bool[] _verdicts = [];

    [Params(1_000, 10_000)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _words = Enumerable.Range(0, WordCount).Select(_ => BuildWord(random)).ToArray();
        _queries = [.. _words.OrderBy(_ => random.Next())];
        _prefixLength = WordLength / 2;
        _verdicts = new bool[_queries.Length * VerdictsPerQuery];
    }

    [Benchmark(Baseline = true)]
    public bool[] InsertThenSearchAndStartsWith()
    {
        var trie = ImplementTrieSolution.CreateByTriePrimitive();
        var next = 0;

        foreach (var word in _words)
        {
            trie.Set(word, true);
        }

        foreach (var word in _queries)
        {
            _verdicts[next++] = trie.HasKey(word);
            _verdicts[next++] = trie.HasPrefix(word[.._prefixLength]);
        }

        return _verdicts;
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
