namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a word-ladder scenario shaped like LeetCode's Word Ladder family (LC
// 126/127): a chain of wordCount single-letter mutations starting from a fixed
// beginWord, so a real shortest transformation sequence to the chain's last word
// always exists and every benchmark below has to do genuine BFS work instead of
// failing fast. The full pairwise one-letter-apart scan run afterward (matching
// WordLadder(II)Tests' own BuildGraph) then finds every edge, not just the ones the
// chain happened to walk - the resulting graph is a random cluster in the Hamming
// graph over {a..z}^wordLength, not just one bare path.
internal static class WordLadderGraphs
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

    public static (List<string> Words, string BeginWord, string EndWord) BuildChain(
        int wordCount, int wordLength, int seed)
    {
        var random = new Random(seed);
        var current = new string('a', wordLength).ToCharArray();
        var words = new List<string> { new(current) };

        for (var i = 1; i < wordCount; i++)
        {
            var position = random.Next(wordLength);
            current[position] = Alphabet[random.Next(Alphabet.Length)];
            words.Add(new string(current));
        }

        return (words, words[0], words[^1]);
    }

    public static (Dictionary<string, WordLadderNode> NodesByWord, WordLadderNode BeginNode) BuildGraph(
        IReadOnlyList<string> words, string beginWord)
    {
        var nodesByWord = new Dictionary<string, WordLadderNode>();

        foreach (var word in words)
        {
            nodesByWord.TryAdd(word, new WordLadderNode(word));
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

        return (nodesByWord, nodesByWord[beginWord]);
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
