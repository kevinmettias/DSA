using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordLadderIIBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for the same question - the shortest transformation sequences - so a harness whose arms
// disagree is timing two different problems. Each arm returns the sequences it built, and the
// workload's chain shape keeps them real rather than none, which is what the count assertion below
// pins.
//
// LC 126 also promises its shortest sequences total at most 10^5 words, so at the larger size they
// are counted here - shortest paths into each word, layer by layer, without either arm - and their
// total checked against that cap.
public sealed partial class WordLadderIIBenchmarksTests
{
    private const int SmallestWordCount = 50;

    // Mirrors WordLadderIIBenchmarks' own private WordLength and RandomSeed, and its larger size.
    private const int WordLength = 5;
    private const int Seed = 126;
    private const int LargestWordCount = 300;

    private const long MaxTotalSequenceWords = 100_000;
    private const int MinimumSequenceCount = 1;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().MutationLayeredBfsBacktrack()),
            AnswerGraphText.Of(BuildHarness().MutationLayeredBfsBacktrack()));

    [Fact]
    public void MutationLayeredBfsBacktrack_ConnectedChain_FindsAnActualSequence() =>
        Assert.True(BuildHarness().MutationLayeredBfsBacktrack().Count >= MinimumSequenceCount);

    [Fact]
    public void ReduceGraphBfsBacktrack_ConnectedChain_FindsAnActualSequence() =>
        Assert.True(BuildHarness().ReduceGraphBfsBacktrack().Count >= MinimumSequenceCount);

    [Fact]
    public void Setup_LargestWordCount_ShortestSequencesStayInsideTheOutputCap()
    {
        var (words, beginWord, endWord) =
            HammingWorkloads.BuildChain(LargestWordCount, WordLength, StandardAlphabets.LowercaseLatin, seed: Seed);
        var (sequenceCount, wordsPerSequence) = CountShortestSequences(words.ToHashSet(), beginWord, endWord);

        Assert.InRange(sequenceCount * wordsPerSequence, 1, MaxTotalSequenceWords);
    }

    // Breadth-first from beginWord, carrying how many shortest paths reach each word of the next layer;
    // the layer endWord first appears in gives the sequence count and every sequence's word count.
    private static (long Count, long WordsPerSequence) CountShortestSequences(
        HashSet<string> words, string beginWord, string endWord)
    {
        var layer = new Dictionary<string, long> { [beginWord] = 1 };
        var reached = new HashSet<string> { beginWord };
        var wordsPerSequence = 1L;

        while (layer.Count > 0 && !layer.ContainsKey(endWord))
        {
            layer = NextLayer(layer, words, reached);
            reached.UnionWith(layer.Keys);
            wordsPerSequence++;
        }

        return (layer.GetValueOrDefault(endWord), wordsPerSequence);
    }

    private static Dictionary<string, long> NextLayer(
        Dictionary<string, long> layer, HashSet<string> words, HashSet<string> reached)
    {
        var next = new Dictionary<string, long>();

        foreach (var (word, paths) in layer)
        {
            foreach (var neighbor in words.Where(candidate => !reached.Contains(candidate) && IsOneLetterApart(word, candidate)))
            {
                next[neighbor] = next.GetValueOrDefault(neighbor) + paths;
            }
        }

        return next;
    }

    private static bool IsOneLetterApart(string first, string second) =>
        first.Zip(second).Count(letters => letters.First != letters.Second) == 1;

    private static WordLadderIIBenchmarks BuildHarness()
    {
        var harness = new WordLadderIIBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
