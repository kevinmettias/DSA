namespace DSAExperimentation.LeetCode.ShortestDistanceToTargetStringInACircularArray;

// LC 2515's words[] reshaped once into the node form the BFS strategy walks: index
// i wired to its two circular steps. Being a named type rather than a bare
// List<CircularArrayNode> is what keeps the hoisted overload unambiguous against
// LeetCode's own string[] shape (ARCHITECTURE.md #17.4) - it is not an
// IEnumerable, so the two overloads can never both bind - and it lets a benchmark
// charge node construction to [GlobalSetup] instead of to the search being
// measured. Same role FunctionalGraph plays for LC 2360.
internal readonly record struct CircularArrayGraph(List<CircularArrayNode> Nodes)
{
    public static CircularArrayGraph Build(string[] words)
    {
        var length = words.Length;
        var nodes = new List<CircularArrayNode>(length);

        for (var i = 0; i < length; i++)
        {
            nodes.Add(new CircularArrayNode(i, words[i]));
        }

        for (var i = 0; i < length; i++)
        {
            nodes[i].Neighbors.Add(nodes[(i + 1) % length]);
            nodes[i].Neighbors.Add(nodes[((i - 1) + length) % length]);
        }

        return new CircularArrayGraph(nodes);
    }
}
