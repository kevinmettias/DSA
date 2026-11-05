using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.LeetCode.Solutions.ImplementTrie;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: every arm is ImplementTrieSolution's. This is LC 208's strategy benchmark: the
// textbook node chain over plain arrays against the one trie as a set under each store LC 208's words
// allow - slots packed in letter order behind a mask, a slot for every letter, and a hash map per
// node - so it shows where the hand-written chain wins and by how much. The textbook arm is the
// baseline (ARCHITECTURE 17.5): what a LeetCode answer writes without this repo, not a rival
// representation of the repo's own. Each arm inserts the whole word set, then replays search and
// startsWith over a shuffled copy of it, so insert cost is charged to both halves rather than just
// the first, and returns every verdict, each query's search then its startsWith.
public class ImplementTrieBenchmarks
{
    private const int RandomSeed = 208; // LC problem number
    private const int WordLength = 8;

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

    private static string BuildWord(Random random)
    {
        var letters = new char[WordLength];

        for (var index = 0; index < WordLength; index++)
        {
            letters[index] = EnglishLowercase.Letters[random.Next(EnglishLetters.Size)];
        }

        return new string(letters);
    }

    // Written against the textbook trie's own type rather than through Replay: a generic method over a
    // class shares one body across classes and calls through the interface, a dispatch the one trie's
    // struct views do not pay.
    [Benchmark(Baseline = true)]
    public bool[] ArrayNodes()
    {
        var trie = ImplementTrieSolution.CreateByArrayNodes();
        var next = 0;

        foreach (var word in _words)
        {
            trie.Add(word);
        }

        foreach (var word in _queries)
        {
            _verdicts[next++] = trie.HasKey(word);
            _verdicts[next++] = trie.HasPrefix(word[.._prefixLength]);
        }

        return _verdicts;
    }

    [Benchmark]
    public bool[] PackedSlots() => Replay(ImplementTrieSolution.CreateByPackedSlots());

    [Benchmark]
    public bool[] DenseSlots() => Replay(ImplementTrieSolution.CreateByDenseSlots());

    [Benchmark]
    public bool[] HashedChildren() => Replay(ImplementTrieSolution.CreateByHashedChildren());

    // The one workload, compiled once for each set: every set passed here is a struct, so each call is
    // direct.
    private bool[] Replay<TSet>(TSet words)
        where TSet : IPrefixSet
    {
        var next = 0;

        foreach (var word in _words)
        {
            words.Add(word);
        }

        foreach (var word in _queries)
        {
            _verdicts[next++] = words.HasKey(word);
            _verdicts[next++] = words.HasPrefix(word[.._prefixLength]);
        }

        return _verdicts;
    }
}
