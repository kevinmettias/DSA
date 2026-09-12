using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.DataStructures.Trie;
using DSAExperimentation.LeetCode.ConcatenatedWords;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ConcatenatedWordsSolution's, the same methods
// ConcatenatedWordsTests proves correct. _words tiles a single short dictionary
// word into one long candidate plus the dictionary word itself, so both
// strategies reach the identical classification, isolating the segmentation-scan
// cost. Each arm is handed the prepared lookup structure its hoisted overload
// takes - a Set for the DP scan, a Trie for the pruned walk - so dictionary
// construction is charged to [GlobalSetup] rather than to the scan being
// measured.
[MemoryDiagnoser]
public class ConcatenatedWordsBenchmarks
{
    private const string DictionaryWord = "cat";
    private const int DictionaryWordLength = 3;

    [Params(600, 3000)]
    public int Length;

    private string[] _words = null!;
    private Set<string> _dictionary = null!;
    private Trie<bool> _trie = null!;

    [GlobalSetup]
    public void Setup()
    {
        var tiles = Enumerable.Repeat(DictionaryWord, Length / DictionaryWordLength);
        var candidate = string.Concat(tiles);
        _words = [candidate, DictionaryWord];

        _dictionary = new Set<string>(_words);

        _trie = new Trie<bool>();
        foreach (var word in _words)
        {
            _trie.Set(word, true);
        }
    }

    [Benchmark(Baseline = true)]
    public List<string> HashSetUnboundedScan() =>
        ConcatenatedWordsSolution.FindAllByHashSetScan(_words, _dictionary);

    [Benchmark]
    public List<string> TriePrunedMemoized() =>
        ConcatenatedWordsSolution.FindAllByTriePrunedMemo(_words, _trie);
}
