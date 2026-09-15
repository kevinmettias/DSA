using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.WordLadderII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are WordLadderIISolution's. Unlike the pre-refactor
// version, which counted sequences rather than building them so the two arms could
// skip materializing a potentially exponential result, both arms now return
// LeetCode's actual answer - the same methods WordLadderIITests proves correct.
// On a chain-shaped workload the shortest-path DAG is narrow, so building the
// sequences costs little and the comparison is still about search cost.
[MemoryDiagnoser]
public class WordLadderIIBenchmarks
{
    private const int WordLength = 6;
    private const int RandomSeed = 126; private Set<string> _wordSet = new();

    private HammingGraph _graph = null!;
    private string _beginWord = "";
    private string _endWord = "";
    // LC problem number

    [Params(50, 300)]
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
    public int MutationLayeredBfsBacktrack() =>
        WordLadderIISolution.FindLaddersByLayeredMutation(
            new WordLadderIISolution.BeginWord(_beginWord),
            new WordLadderIISolution.EndWord(_endWord),
            _wordSet).Count;

    [Benchmark]
    public int ReduceGraphBfsBacktrack() =>
        WordLadderIISolution.FindLaddersByReduceGraph(
            _graph, new WordLadderIISolution.EndWord(_endWord)).Count;
}
