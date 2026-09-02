namespace DSAExperimentation.LeetCode.FrequenciesOfShortestSupersequences;

// LC 3435's own parsed input: every word is exactly 2 characters, so each one IS a
// directed precedence edge (word[0] must appear before word[1] in any
// supersequence) - Letters is the distinct alphabet those edges range over, sorted
// so both strategies enumerate doubled-letter subsets in the same deterministic
// order. This is the hoisted overload's prepared type (§17.4), the same role
// LockGraph.Build(deadends) plays for OpenTheLock.
internal sealed class LetterGraph
{
    private LetterGraph(List<char> letters, List<(char From, char To)> edges)
    {
        Letters = letters;
        Edges = edges;
    }

    public IReadOnlyList<char> Letters { get; }

    public IReadOnlyList<(char From, char To)> Edges { get; }

    public static LetterGraph Build(IEnumerable<string> words)
    {
        var edges = words.Select(word => (From: word[0], To: word[1])).ToList();
        var letters = edges
            .SelectMany(edge => new[] { edge.From, edge.To })
            .Distinct()
            .Order()
            .ToList();

        return new LetterGraph(letters, edges);
    }
}
