using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Word Ladder II (LC 126): every shortest transformation sequence, not just the
// shortest length WordLadderBenchmarks answers. The textbook baseline mutates every
// position per BFS layer to build a parents-by-word map (Dictionary<string,
// List<string>>, no repo primitives), keeping every layer's parents until the whole
// layer finishes (so a word discovered from two same-level parents keeps both). The
// primitive-composed side reuses this repo's own BFS - Reduce.Graph +
// DistanceMapReduceAlgebra over a precomputed WordLadderNode graph, the same
// composition WordLadderBenchmarks/GridShortestPath.Distance already use - for the
// distance labels, then backtracks from endWord following only edges that strictly
// decrease that label (WordLadderIITests' own CollectShortestPaths). Both count
// every sequence found rather than materializing them, so the comparison is about
// search cost, not allocating the (potentially exponential) result set.
[MemoryDiagnoser]
public class WordLadderIIBenchmarks
{
    private const int WordLength = 6;
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

    [Params(50, 300)]
    public int WordCount;

    private HashSet<string> _wordSet = null!;
    private string _beginWord = null!;
    private string _endWord = null!;
    private Dictionary<string, WordLadderNode> _nodesByWord = null!;
    private WordLadderNode _beginNode = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (words, beginWord, endWord) = WordLadderGraphs.BuildChain(WordCount, WordLength, seed: 126);
        _wordSet = [.. words];
        _beginWord = beginWord;
        _endWord = endWord;

        (_nodesByWord, _beginNode) = WordLadderGraphs.BuildGraph(words, beginWord);
    }

    [Benchmark(Baseline = true)]
    public int MutationLayeredBfsBacktrack()
    {
        var parents = new Dictionary<string, List<string>>();
        var knownDistance = new Dictionary<string, int> { [_beginWord] = 0 };
        var currentLevel = new List<string> { _beginWord };
        var buffer = new char[WordLength];

        while (currentLevel.Count > 0 && !parents.ContainsKey(_endWord))
        {
            var nextLevel = new Dictionary<string, List<string>>();

            foreach (var word in currentLevel)
            {
                word.CopyTo(buffer);

                for (var i = 0; i < WordLength; i++)
                {
                    var original = buffer[i];

                    foreach (var letter in Alphabet)
                    {
                        if (letter == original)
                        {
                            continue;
                        }

                        buffer[i] = letter;
                        var candidate = new string(buffer);

                        if (_wordSet.Contains(candidate) && !knownDistance.ContainsKey(candidate))
                        {
                            if (!nextLevel.TryGetValue(candidate, out var candidateParents))
                            {
                                nextLevel[candidate] = candidateParents = [];
                            }

                            candidateParents.Add(word);
                        }
                    }

                    buffer[i] = original;
                }
            }

            var nextDistance = knownDistance[currentLevel[0]] + 1;

            foreach (var (word, wordParents) in nextLevel)
            {
                knownDistance[word] = nextDistance;
                parents[word] = wordParents;
            }

            currentLevel = [.. nextLevel.Keys];
        }

        if (!parents.ContainsKey(_endWord))
        {
            return 0;
        }

        var count = 0;
        CountPaths(_endWord, _beginWord, parents, ref count);
        return count;
    }

    private static void CountPaths(string word, string beginWord, Dictionary<string, List<string>> parents, ref int count)
    {
        if (word == beginWord)
        {
            count++;
            return;
        }

        if (!parents.TryGetValue(word, out var wordParents))
        {
            return;
        }

        foreach (var parent in wordParents)
        {
            CountPaths(parent, beginWord, parents, ref count);
        }
    }

    [Benchmark]
    public int ReduceGraphBfsBacktrack()
    {
        var distances = Reduce.Graph<
            WordLadderNode, WordLadderTopology, ListChildren<WordLadderNode>,
            NaturalChildOrder<WordLadderNode, ListChildren<WordLadderNode>>, ListChildren<WordLadderNode>,
            BreadthFirstReduceOrder<WordLadderNode>,
            DistanceMapReduceAlgebra<WordLadderNode>, Dictionary<WordLadderNode, int>>(_beginNode);

        var endNode = _nodesByWord[_endWord];

        if (!distances.ContainsKey(endNode))
        {
            return 0;
        }

        var count = 0;
        CountShortestPaths(endNode, _beginNode, distances, ref count);
        return count;
    }

    private static void CountShortestPaths(
        WordLadderNode node, WordLadderNode beginNode, Dictionary<WordLadderNode, int> distances, ref int count)
    {
        if (node == beginNode)
        {
            count++;
            return;
        }

        var target = distances[node] - 1;

        foreach (var neighbor in node.Neighbors)
        {
            if (distances.TryGetValue(neighbor, out var neighborDistance) && neighborDistance == target)
            {
                CountShortestPaths(neighbor, beginNode, distances, ref count);
            }
        }
    }
}
