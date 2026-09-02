using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Algorithms.ShortestPaths.Hamming;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.WordLadder;

// LeetCode 127. Word Ladder: the length of a shortest beginWord-to-endWord chain
// in which consecutive words differ by one letter and every word after the first
// is in wordList.
//
// LeetCode counts words in the sequence rather than edges walked, so every
// strategy here answers distance + 1, and 0 when endWord is unreachable.
internal static class WordLadderSolution
{
    private const int NoLadder = 0;

    // The textbook answer: BCL Queue + HashSet, generating each of the 25 * L
    // candidate mutations on the fly and keeping the ones the dictionary contains,
    // so the graph is never materialized.
    public static int LadderLengthByMutationQueue(string beginWord, string endWord, IEnumerable<string> wordList)
    {
        var wordSet = new Set<string>(wordList);

        return LadderLengthByMutationQueue(beginWord, endWord, wordSet);
    }

    public static int LadderLengthByMutationQueue(string beginWord, string endWord, Set<string> wordSet)
    {
        if (!wordSet.Has(endWord))
        {
            return NoLadder;
        }

        var distance = HammingSearch.MutationDistance(beginWord, endWord, wordSet, StandardAlphabets.LowercaseLatin);

        return distance is null ? NoLadder : distance.Value + 1;
    }

    // This repo's own BFS over DataStructures.Graph.Hamming's materialized graph: Reduce.Graph in
    // BreadthFirstReduceOrder with DistanceMapReduceAlgebra is already "distance
    // from a root to every node", so the puzzle is one lookup in the result.
    public static int LadderLengthByReduceGraph(string beginWord, string endWord, IEnumerable<string> wordList)
    {
        var graph = HammingGraph.Build(beginWord, wordList);

        return LadderLengthByReduceGraph(graph, endWord);
    }

    public static int LadderLengthByReduceGraph(HammingGraph graph, string endWord)
    {
        if (!graph.TryGetNode(endWord, out var endNode))
        {
            return NoLadder;
        }

        var distances = HammingDistances.From(graph.Root);

        return distances.TryGetValue(endNode, out var distance) ? distance + 1 : NoLadder;
    }
}
