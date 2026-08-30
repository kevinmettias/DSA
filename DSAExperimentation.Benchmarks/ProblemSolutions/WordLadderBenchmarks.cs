using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Word Ladder (LC 127): the textbook mutate-every-position BFS (Queue<string> plus
// a HashSet<string> dictionary, generating each candidate word on the fly) vs. this
// repo's own BFS - Reduce.Graph + DistanceMapReduceAlgebra over a precomputed
// WordLadderNode graph, the same "distance to some specific target" composition
// GridShortestPath.Distance already uses. _words is a connected mutation chain
// (WordLadderGraphs.BuildChain), so endWord is always genuinely reachable and both
// strategies do a real full BFS instead of failing fast.
[MemoryDiagnoser]
public class WordLadderBenchmarks
{
    private const int WordLength = 6;
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

    [Params(200, 2_000)]
    public int WordCount;

    private HashSet<string> _wordSet = null!;
    private string _beginWord = null!;
    private string _endWord = null!;
    private Dictionary<string, WordLadderNode> _nodesByWord = null!;
    private WordLadderNode _beginNode = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (words, beginWord, endWord) = WordLadderGraphs.BuildChain(WordCount, WordLength, seed: 127);
        _wordSet = [.. words];
        _beginWord = beginWord;
        _endWord = endWord;

        (_nodesByWord, _beginNode) = WordLadderGraphs.BuildGraph(words, beginWord);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs()
    {
        var visited = new HashSet<string> { _beginWord };
        var queue = new Queue<(string Word, int Distance)>();
        queue.Enqueue((_beginWord, 0));
        var buffer = new char[WordLength];

        while (queue.Count > 0)
        {
            var (word, distance) = queue.Dequeue();

            if (word == _endWord)
            {
                return distance;
            }

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

                    if (_wordSet.Contains(candidate) && visited.Add(candidate))
                    {
                        queue.Enqueue((candidate, distance + 1));
                    }
                }

                buffer[i] = original;
            }
        }

        return -1;
    }

    [Benchmark]
    public int ReduceGraphBfs()
    {
        var distances = Reduce.Graph<
            WordLadderNode, WordLadderTopology, ListChildren<WordLadderNode>,
            NaturalChildOrder<WordLadderNode, ListChildren<WordLadderNode>>, ListChildren<WordLadderNode>,
            BreadthFirstReduceOrder<WordLadderNode>,
            DistanceMapReduceAlgebra<WordLadderNode>, Dictionary<WordLadderNode, int>>(_beginNode);

        return distances.TryGetValue(_nodesByWord[_endWord], out var distance) ? distance : -1;
    }
}
