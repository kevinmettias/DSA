using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stream of Characters (LC 1032): re-testing every suffix of the whole stream so
// far against a HashSet<string> on every query (O(stream length * max word length)
// per query, since each candidate substring itself costs O(length) to materialize
// and hash) vs. this repo's own LowercaseTrie<TValue> built from reversed words,
// walked backward one already-streamed character at a time through
// DynamicArray<char>'s O(1) indexed Get (O(max word length) per query, the standard
// LC1032 solution). Words are chosen with no shared suffixes so neither approach
// gets an early exit from an early match, forcing both through their full per-query
// cost on nearly every character.
[MemoryDiagnoser]
public class StreamOfCharactersBenchmarks
{
    private static readonly string[] Words =
    [
        "characters", "algorithm", "benchmark", "primitive", "structure",
        "traversal", "reference", "composed", "sequence", "children",
    ];

    private static readonly int MaxWordLength = Words.Max(w => w.Length);

    [Params(200, 3_000)]
    public int StreamLength;

    private char[] _stream = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1032);
        _stream = Enumerable.Range(0, StreamLength).Select(_ => (char)('a' + random.Next(0, 26))).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanEverySuffixAgainstHashSet()
    {
        var words = new HashSet<string>(Words);
        var buffer = new List<char>();
        var matches = 0;

        foreach (var letter in _stream)
        {
            buffer.Add(letter);

            for (var length = 1; length <= Math.Min(buffer.Count, MaxWordLength); length++)
            {
                var suffix = new string(buffer.GetRange(buffer.Count - length, length).ToArray());

                if (words.Contains(suffix))
                {
                    matches++;
                    break;
                }
            }
        }

        return matches;
    }

    [Benchmark]
    public int ReversedTrieBackwardWalk()
    {
        var reversedWords = new LowercaseTrie<bool>();

        foreach (var word in Words)
        {
            reversedWords.Set(new string(word.Reverse().ToArray()), true);
        }

        var buffer = new DynamicArray<char>();
        var matches = 0;

        foreach (var letter in _stream)
        {
            buffer.Add(letter);

            var node = reversedWords.Root;

            for (var i = buffer.Count - 1; i >= 0 && node is not null; i--)
            {
                node = node.Children[buffer.Get(i) - 'a'];

                if (node is not null && node.HasValue)
                {
                    matches++;
                    break;
                }
            }
        }

        return matches;
    }
}
