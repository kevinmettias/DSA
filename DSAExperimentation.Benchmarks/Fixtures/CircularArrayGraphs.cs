namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a Shortest Distance to Target String in a Circular Array scenario (LC
// 2515): length filler words arranged in a circle, with exactly one word replaced
// by the target string at the index diametrically opposite startIndex (0) - the
// worst case for either search direction, forcing both benchmarked approaches
// through real work instead of an immediate first-neighbor match.
internal static class CircularArrayGraphs
{
    public const string Target = "target-word";
    private const string Filler = "filler-word";

    public static (CircularArrayNode[] Nodes, string[] Words, int StartIndex) BuildGraph(int length)
    {
        var words = new string[length];

        for (var i = 0; i < length; i++)
        {
            words[i] = Filler;
        }

        words[length / 2] = Target;

        var nodes = new CircularArrayNode[length];

        for (var i = 0; i < length; i++)
        {
            nodes[i] = new CircularArrayNode(i, words[i]);
        }

        for (var i = 0; i < length; i++)
        {
            nodes[i].Neighbors.Add(nodes[(i + 1) % length]);
            nodes[i].Neighbors.Add(nodes[((i - 1) + length) % length]);
        }

        return (nodes, words, 0);
    }
}
