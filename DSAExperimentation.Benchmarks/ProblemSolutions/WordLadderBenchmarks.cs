using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.WordLadder;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WordLadderSolution's, the same methods
// WordLadderTests proves correct. The word set is a connected mutation chain, so
// endWord is always genuinely reachable and both strategies run a full BFS instead
// of failing fast. Each arm gets the prepared input its hoisted overload takes, so
// dictionary/graph construction is charged to [GlobalSetup].
[MemoryDiagnoser]
public class WordLadderBenchmarks
{
    private const int WordLength = 6;
    private const int RandomSeed = 127; private Set<string> _wordSet = new();

    private HammingGraph _graph = null!;
    private string _beginWord = "";
    private string _endWord = "";
    // LC problem number

    [Params(200, 2_000)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (words, beginWord, endWord) =
            HammingWorkloads.BuildChain(WordCount, WordLength, StandardAlphabets.LowercaseLatin, seed: RandomSeed);

        _beginWord = beginWord;
        _endWord = endWord;
        _wordSet = new Set<string>(words);
        _graph = HammingGraph.Build(beginWord, words);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() =>
        WordLadderSolution.LadderLengthByMutationQueue(
            new BeginWord(_beginWord), new EndWord(_endWord), _wordSet);

    [Benchmark]
    public int ReduceGraphBfs() => WordLadderSolution.LadderLengthByReduceGraph(_graph, _endWord);
}
