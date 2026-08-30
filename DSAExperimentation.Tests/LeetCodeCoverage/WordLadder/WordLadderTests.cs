using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.Tests.LeetCodeCoverage.WordLadder.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordLadder;

// LeetCode 127. Word Ladder: words are nodes of an implicit graph, with an edge
// between any two words that differ by exactly one letter. The shortest
// transformation sequence's length is exactly Reduce.Graph's own BFS distance from
// beginWord to endWord - DistanceMapReduceAlgebra, the same "distance to some
// specific target" composition GridShortestPath.Distance already uses for LC's grid
// family - plus one, since LeetCode counts words in the sequence, not edges walked.
public sealed partial class WordLadderTests
{
    [Fact]
    public void LadderLength_ClassicExample_ReturnsShortestSequenceWordCount()
    {
        var length = LadderLength("hit", "cog", ["hot", "dot", "dog", "lot", "log", "cog"]);

        Assert.Equal(5, length);
    }

    [Fact]
    public void LadderLength_EndWordMissingFromWordList_ReturnsZero()
    {
        var length = LadderLength("hit", "cog", ["hot", "dot", "dog", "lot", "log"]);

        Assert.Equal(0, length);
    }

    private static int LadderLength(string beginWord, string endWord, List<string> wordList)
    {
        var nodesByWord = BuildGraph(beginWord, wordList, out var beginNode);

        if (!nodesByWord.TryGetValue(endWord, out var endNode))
        {
            return 0;
        }

        var distances = Reduce.Graph<
            WordNode, WordTopology, ListChildren<WordNode>,
            NaturalChildOrder<WordNode, ListChildren<WordNode>>, ListChildren<WordNode>,
            BreadthFirstReduceOrder<WordNode>,
            DistanceMapReduceAlgebra<WordNode>, Dictionary<WordNode, int>>(beginNode);

        return distances.TryGetValue(endNode, out var distance) ? distance + 1 : 0;
    }

    // An edge joins any two words differing by exactly one letter - the classic
    // O(n^2 * L) pairwise scan, small enough here that no repo primitive earns its
    // place over it (see JumpGameBenchmarks/GasStationBenchmarks for the same call
    // on adjacency/reachability scans this size).
    private static HashMap<string, WordNode> BuildGraph(string beginWord, List<string> wordList, out WordNode beginNode)
    {
        var nodesByWord = new HashMap<string, WordNode>();
        beginNode = new WordNode(beginWord);
        nodesByWord.Set(beginWord, beginNode);

        foreach (var word in wordList)
        {
            if (!nodesByWord.HasKey(word))
            {
                nodesByWord.Set(word, new WordNode(word));
            }
        }

        var allNodes = nodesByWord.Values.ToList();

        for (var i = 0; i < allNodes.Count; i++)
        {
            for (var j = i + 1; j < allNodes.Count; j++)
            {
                if (IsOneLetterApart(allNodes[i].Word, allNodes[j].Word))
                {
                    allNodes[i].Neighbors.Add(allNodes[j]);
                    allNodes[j].Neighbors.Add(allNodes[i]);
                }
            }
        }

        return nodesByWord;
    }

    private static bool IsOneLetterApart(string a, string b)
    {
        var differences = 0;

        for (var i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i] && ++differences > 1)
            {
                return false;
            }
        }

        return differences == 1;
    }
}
