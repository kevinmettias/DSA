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

    // The letters a ladder word is spelled in, restated rather than read from
    // EnglishLowercase: a baseline reaches no library type, not even a constant
    // (ARCHITECTURE 17.5).
    private const string LadderLetters = "abcdefghijklmnopqrstuvwxyz";

    // The textbook answer: BCL Queue + HashSet, generating each of the 25 * L
    // candidate mutations on the fly and keeping the ones the dictionary contains,
    // so the graph is never materialized.
    public static int LadderLengthByMutationQueue(BeginWord beginWord, EndWord endWord, IEnumerable<string> wordList)
    {
        var wordSet = new HashSet<string>(wordList);

        return LadderLength(beginWord, endWord, new HashSetWords(wordSet));
    }

    // The same search over a dictionary its caller prepared once, read only through Has.
    public static int LadderLengthByMutationQueue(BeginWord beginWord, EndWord endWord, Set<string> wordSet) =>
        LadderLength(beginWord, endWord, new PreparedWords(wordSet));

    private static int LadderLength<TWords>(BeginWord beginWord, EndWord endWord, TWords words)
        where TWords : struct, IWordSet
    {
        if (!words.Has(endWord.Text))
        {
            return NoLadder;
        }

        var distance = MutationDistance(beginWord.Text, endWord.Text, words);

        if (distance is null)
        {
            return NoLadder;
        }

        return WordCount(distance.Value);
    }

    // Mutations walked from start to target, or null when no path stays inside the
    // dictionary. start itself need not be in it, matching LeetCode's beginWord convention.
    private static int? MutationDistance<TWords>(string start, string target, TWords words)
        where TWords : struct, IWordSet
    {
        var walk = new LadderWalk<TWords>(words, [start], new Queue<(string Word, int Distance)>());
        walk.Queue.Enqueue((start, 0));

        while (walk.Queue.Count > 0)
        {
            var (word, distance) = walk.Queue.Dequeue();

            if (word == target)
            {
                return distance;
            }

            EnqueueDictionaryMutations(word, distance, walk);
        }

        return null;
    }

    // Queues every dictionary word one letter away from word, the first time the walk
    // meets it.
    private static void EnqueueDictionaryMutations<TWords>(string word, int distance, LadderWalk<TWords> walk)
        where TWords : struct, IWordSet
    {
        foreach (var candidate in OneLetterMutations(word))
        {
            if (walk.Words.Has(candidate) && walk.Visited.Add(candidate))
            {
                walk.Queue.Enqueue((candidate, distance + 1));
            }
        }
    }

    // Every word that differs from word in exactly one position, spelled over LadderLetters.
    private static IEnumerable<string> OneLetterMutations(string word)
    {
        var buffer = word.ToCharArray();

        for (var position = 0; position < buffer.Length; position++)
        {
            var original = buffer[position];

            foreach (var replacement in LadderLetters)
            {
                if (replacement != original)
                {
                    buffer[position] = replacement;
                    yield return new string(buffer);
                }
            }

            buffer[position] = original;
        }
    }

    // Membership in the dictionary, the one thing the search asks of it - answered by
    // the LeetCode-shaped overload's own HashSet or by the Set its caller prepared.
    private interface IWordSet
    {
        bool Has(string word);
    }

    private readonly struct HashSetWords(HashSet<string> words) : IWordSet
    {
        public bool Has(string word) => words.Contains(word);
    }

    private readonly struct PreparedWords(Set<string> words) : IWordSet
    {
        public bool Has(string word) => words.Has(word);
    }

    // The dictionary one breadth-first walk tests candidates against, and the visited
    // set and queue it threads through every step.
    private readonly record struct LadderWalk<TWords>(
        TWords Words,
        HashSet<string> Visited,
        Queue<(string Word, int Distance)> Queue)
        where TWords : struct, IWordSet;

    // This repo's own BFS over DataStructures.Graph.Hamming's materialized graph: Reduce.Graph in
    // BreadthFirstReduceOrder with DistanceMapReduceAlgebra is already "distance
    // from a root to every node", so the puzzle is one lookup in the result.
    public static int LadderLengthByReduceGraph(BeginWord beginWord, EndWord endWord, IEnumerable<string> wordList)
    {
        var graph = HammingGraph.Build(beginWord.Text, wordList);

        return LadderLengthByReduceGraph(graph, endWord.Text);
    }

    public static int LadderLengthByReduceGraph(HammingGraph graph, string endWord)
    {
        if (!graph.TryGetNode(endWord, out var endNode))
        {
            return NoLadder;
        }

        var distances = HammingDistances.From(graph.Root);

        return distances.TryGetValue(endNode, out var distance) ? WordCount(distance) : NoLadder;
    }

    // LeetCode counts the words in the chain, not the edges walked between them.
    private static int WordCount(int edges) => edges + 1;
}
