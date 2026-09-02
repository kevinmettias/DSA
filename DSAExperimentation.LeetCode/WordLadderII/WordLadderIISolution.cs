using DSAExperimentation.Algorithms.ShortestPaths.Hamming;
using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.WordLadderII;

// LeetCode 126. Word Ladder II: every shortest transformation sequence, not just
// the shortest length (#127's question).
//
// Both strategies share the same insight - a word sits on SOME shortest sequence
// exactly when a predecessor one step closer to beginWord exists - and differ only
// in how they establish "one step closer": a parents map accumulated per BFS layer,
// or a distance label from a single Reduce.Graph pass over DataStructures.Graph.Hamming's graph.
// The strictly-decreasing filter is what keeps every backtrack finite despite the
// graph itself being cyclic.
internal static class WordLadderIISolution
{
    // The textbook answer: layered BFS building a parents-by-word map, keeping
    // every layer's parents until the whole layer finishes, so a word discovered
    // from two same-level parents keeps both.
    public static List<string[]> FindLaddersByLayeredMutation(
        string beginWord, string endWord, IEnumerable<string> wordList)
    {
        var wordSet = new Set<string>(wordList);

        return FindLaddersByLayeredMutation(beginWord, endWord, wordSet);
    }

    public static List<string[]> FindLaddersByLayeredMutation(
        string beginWord, string endWord, Set<string> wordSet)
    {
        if (beginWord == endWord)
        {
            return [[beginWord]];
        }

        if (!wordSet.Has(endWord))
        {
            return [];
        }

        var parents = BuildParentsByLayer(beginWord, endWord, wordSet);

        if (!parents.ContainsKey(endWord))
        {
            return [];
        }

        var sequences = new List<string[]>();
        CollectByParents(endWord, new ParentWalk(beginWord, parents, [endWord], sequences));
        return sequences;
    }

    // Expands one BFS layer at a time until endWord is first reached, recording
    // every predecessor a word was discovered from within its own layer.
    private static Dictionary<string, List<string>> BuildParentsByLayer(
        string beginWord, string endWord, Set<string> wordSet)
    {
        var parents = new Dictionary<string, List<string>>();
        var knownDistance = new Dictionary<string, int> { [beginWord] = 0 };
        var currentLevel = new List<string> { beginWord };

        while (currentLevel.Count > 0 && !parents.ContainsKey(endWord))
        {
            currentLevel = ExpandLayer(currentLevel, wordSet, knownDistance, parents);
        }

        return parents;
    }

    // This repo's own BFS labels every reachable word with its distance from
    // beginWord in one pass; backtracking then follows only edges that strictly
    // decrease that label.
    public static List<string[]> FindLaddersByReduceGraph(
        string beginWord, string endWord, IEnumerable<string> wordList)
    {
        var graph = HammingGraph.Build(beginWord, wordList);

        return FindLaddersByReduceGraph(graph, endWord);
    }

    public static List<string[]> FindLaddersByReduceGraph(HammingGraph graph, string endWord)
    {
        if (!graph.TryGetNode(endWord, out var endNode))
        {
            return [];
        }

        var distances = HammingDistances.From(graph.Root);

        if (!distances.ContainsKey(endNode))
        {
            return [];
        }

        var sequences = new List<string[]>();
        CollectByDistance(endNode, new DistanceWalk(graph.Root, distances, [endNode.Value], sequences));
        return sequences;
    }

    private static List<string> ExpandLayer(
        List<string> currentLevel,
        Set<string> wordSet,
        Dictionary<string, int> knownDistance,
        Dictionary<string, List<string>> parents)
    {
        var nextLevel = new Dictionary<string, List<string>>();

        foreach (var word in currentLevel)
        {
            RecordUnseenNeighbors(word, wordSet, knownDistance, nextLevel);
        }

        CommitLayer(nextLevel, knownDistance[currentLevel[0]] + 1, knownDistance, parents);

        return [.. nextLevel.Keys];
    }

    private static void RecordUnseenNeighbors(
        string word,
        Set<string> wordSet,
        Dictionary<string, int> knownDistance,
        Dictionary<string, List<string>> nextLevel)
    {
        foreach (var candidate in HammingGraph.OneCharacterMutations(word, StandardAlphabets.LowercaseLatin))
        {
            if (!wordSet.Has(candidate) || knownDistance.ContainsKey(candidate))
            {
                continue;
            }

            if (!nextLevel.TryGetValue(candidate, out var candidateParents))
            {
                nextLevel[candidate] = candidateParents = [];
            }

            candidateParents.Add(word);
        }
    }

    // A whole layer is committed at once, so a word discovered from two parents in
    // the SAME layer keeps both - which is what makes every shortest sequence
    // reachable during backtracking rather than just one of them.
    private static void CommitLayer(
        Dictionary<string, List<string>> nextLevel,
        int nextDistance,
        Dictionary<string, int> knownDistance,
        Dictionary<string, List<string>> parents)
    {
        foreach (var (word, wordParents) in nextLevel)
        {
            knownDistance[word] = nextDistance;
            parents[word] = wordParents;
        }
    }

    private static void CollectByParents(string word, ParentWalk walk)
    {
        if (word == walk.BeginWord)
        {
            walk.Sequences.Add(Reversed(walk.PathFromEnd));
            return;
        }

        if (!walk.Parents.TryGetValue(word, out var wordParents))
        {
            return;
        }

        foreach (var parent in wordParents)
        {
            walk.PathFromEnd.Add(parent);
            CollectByParents(parent, walk);
            walk.PathFromEnd.RemoveAt(walk.PathFromEnd.Count - 1);
        }
    }

    private static void CollectByDistance(HammingNode node, DistanceWalk walk)
    {
        if (node == walk.BeginNode)
        {
            walk.Sequences.Add(Reversed(walk.PathFromEnd));
            return;
        }

        var target = walk.Distances[node] - 1;

        foreach (var neighbor in node.Neighbors)
        {
            if (walk.Distances.TryGetValue(neighbor, out var neighborDistance) && neighborDistance == target)
            {
                walk.PathFromEnd.Add(neighbor.Value);
                CollectByDistance(neighbor, walk);
                walk.PathFromEnd.RemoveAt(walk.PathFromEnd.Count - 1);
            }
        }
    }

    // The state a single backtrack threads through every recursive step.
    // PathFromEnd accumulates backward from endWord and is unwound on the way out.
    private readonly record struct ParentWalk(
        string BeginWord,
        Dictionary<string, List<string>> Parents,
        List<string> PathFromEnd,
        List<string[]> Sequences);

    private readonly record struct DistanceWalk(
        HammingNode BeginNode,
        Dictionary<HammingNode, int> Distances,
        List<string> PathFromEnd,
        List<string[]> Sequences);

    // pathFromEnd accumulates backward from endWord to beginWord; sequences are
    // reported in forward order.
    private static string[] Reversed(List<string> pathFromEnd)
    {
        var sequence = new string[pathFromEnd.Count];

        for (var i = 0; i < pathFromEnd.Count; i++)
        {
            sequence[i] = pathFromEnd[pathFromEnd.Count - 1 - i];
        }

        return sequence;
    }
}
