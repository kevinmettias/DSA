namespace DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

// Every distinct string named by original/changed, built once - the domain
// model, not an answer to any one query about it, the same split
// LetterNetwork uses for LC 2976's fixed 26-letter alphabet, generalized
// here to whatever substrings the rules mention. original[i].length ==
// changed[i].length is a problem constraint, so every edge - and therefore
// every path Floyd-Warshall can build from it - only ever connects
// same-length strings.
internal sealed class SubstringNetwork
{
    private SubstringNetwork(Dictionary<string, StringNode> nodesByValue) => NodesByValue = nodesByValue;

    public Dictionary<string, StringNode> NodesByValue { get; }

    public static SubstringNetwork Build(string[] original, string[] changed, int[] cost)
    {
        var nodesByValue = new Dictionary<string, StringNode>();

        for (var i = 0; i < original.Length; i++)
        {
            var from = GetOrAdd(nodesByValue, original[i]);
            var to = GetOrAdd(nodesByValue, changed[i]);

            from.Edges.Add((cost[i], to));
        }

        return new SubstringNetwork(nodesByValue);
    }

    private static StringNode GetOrAdd(Dictionary<string, StringNode> nodesByValue, string value)
    {
        if (!nodesByValue.TryGetValue(value, out var node))
        {
            node = new StringNode(value);
            nodesByValue[value] = node;
        }

        return node;
    }
}
