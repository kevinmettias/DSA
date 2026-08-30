using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.Tests.LeetCodeCoverage.WordLadderII.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordLadderII;

// LeetCode 126. Word Ladder II: every shortest transformation sequence from
// beginWord to endWord, not just the shortest length (#127's question).
// Reduce.Graph's own BFS distance map (DistanceMapReduceAlgebra, the same
// composition WordLadderTests/GridShortestPath.Distance already use) still does
// the hard part in one pass - it labels every reachable word with its distance
// from beginWord. A word sits on SOME shortest sequence exactly when a neighbor
// one distance closer to beginWord exists, so backtracking from endWord only
// ever follows edges that strictly decrease that label - the filter that turns
// the flat distance map into the DAG of shortest paths LeetCode wants enumerated,
// and what keeps every walk finite despite the graph itself being cyclic.
public sealed partial class WordLadderIITests
{
    [Fact]
    public void FindLadders_ClassicExample_ReturnsEveryShortestSequence()
    {
        var sequences = FindLadders("hit", "cog", ["hot", "dot", "dog", "lot", "log", "cog"]);

        Assert.Equal(
            new[]
            {
                new[] { "hit", "hot", "dot", "dog", "cog" },
                new[] { "hit", "hot", "lot", "log", "cog" },
            },
            sequences.OrderBy(sequence => string.Join(',', sequence)).ToArray());
    }

    [Fact]
    public void FindLadders_EndWordMissingFromWordList_ReturnsNoSequences()
    {
        var sequences = FindLadders("hit", "cog", ["hot", "dot", "dog", "lot", "log"]);

        Assert.Empty(sequences);
    }

    private static List<string[]> FindLadders(string beginWord, string endWord, List<string> wordList)
    {
        var nodesByWord = BuildGraph(beginWord, wordList, out var beginNode);

        if (!nodesByWord.TryGetValue(endWord, out var endNode))
        {
            return [];
        }

        var distances = Reduce.Graph<
            WordNode, WordTopology, ListChildren<WordNode>,
            NaturalChildOrder<WordNode, ListChildren<WordNode>>, ListChildren<WordNode>,
            BreadthFirstReduceOrder<WordNode>,
            DistanceMapReduceAlgebra<WordNode>, Dictionary<WordNode, int>>(beginNode);

        if (!distances.ContainsKey(endNode))
        {
            return [];
        }

        var sequences = new List<string[]>();
        CollectShortestPaths(endNode, beginNode, distances, [endNode.Word], sequences);
        return sequences;
    }

    // Walks backward from `node` toward beginNode, only ever stepping to a neighbor
    // exactly one distance closer.
    private static void CollectShortestPaths(
        WordNode node, WordNode beginNode, Dictionary<WordNode, int> distances, List<string> pathFromEnd, List<string[]> sequences)
    {
        if (node == beginNode)
        {
            var sequence = new string[pathFromEnd.Count];

            for (var i = 0; i < pathFromEnd.Count; i++)
            {
                sequence[i] = pathFromEnd[pathFromEnd.Count - 1 - i];
            }

            sequences.Add(sequence);
            return;
        }

        var target = distances[node] - 1;

        foreach (var neighbor in node.Neighbors)
        {
            if (distances.TryGetValue(neighbor, out var neighborDistance) && neighborDistance == target)
            {
                pathFromEnd.Add(neighbor.Word);
                CollectShortestPaths(neighbor, beginNode, distances, pathFromEnd, sequences);
                pathFromEnd.RemoveAt(pathFromEnd.Count - 1);
            }
        }
    }

    // An edge joins any two words differing by exactly one letter - the classic
    // O(n^2 * L) pairwise scan, small enough here that no repo primitive earns its
    // place over it (see WordLadderTests, JumpGameBenchmarks/GasStationBenchmarks
    // for the same call on adjacency/reachability scans this size).
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
