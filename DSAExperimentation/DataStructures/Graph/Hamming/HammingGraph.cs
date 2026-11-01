using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.Graph.Hamming;

// The subgraph of the Hamming graph over some alphabet induced by a given set of
// equal-length strings: an edge joins any two of them differing in exactly one
// position.
//
// This is the domain model behind LeetCode's whole "one-character mutation"
// family - Word Ladder (127), Word Ladder II (126), Minimum Genetic Mutation
// (433) - each of which previously carried its own copy of this node type, this
// pairwise wiring scan, and this distance predicate.
/// <summary>
/// A graph over equal-length strings, with an edge between every two that differ in exactly one position.
/// </summary>
public sealed class HammingGraph
{
    private readonly HashMap<string, HammingNode> _nodesByValue;

    // The node the caller seeded the graph from - LeetCode's beginWord/startGene,
    // which is not always a member of the supplied set.
    /// <summary>The node for the string the graph was seeded from, which the values may omit.</summary>
    public HammingNode Root { get; }

    private HammingGraph(HashMap<string, HammingNode> nodesByValue, HammingNode root)
    {
        _nodesByValue = nodesByValue;
        Root = root;
    }

    // The classic O(n^2 * L) pairwise scan. Small enough for every problem in this
    // family that no repo primitive earns its place over it; the interesting work
    // is the search that runs afterward, not the wiring.
    /// <summary>
    /// Builds the graph over a root and a set of values by comparing every pair, O(n^2 * L) for n strings of length L.
    /// </summary>
    /// <param name="root">The string that becomes <see cref="Root"/>, whether or not the values include it.</param>
    /// <param name="values">The strings to add, each as long as the root; a repeated string shares one node.</param>
    /// <returns>The graph, with one node per distinct string.</returns>
    public static HammingGraph Build(string root, IEnumerable<string> values)
    {
        var rootNode = new HammingNode(root);
        var nodesByValue = BuildNodes(rootNode, values);
        var allNodes = nodesByValue.Values.ToList();

        WireEdges(allNodes);

        return new HammingGraph(nodesByValue, rootNode);
    }

    private static HashMap<string, HammingNode> BuildNodes(HammingNode rootNode, IEnumerable<string> values)
    {
        var nodesByValue = new HashMap<string, HammingNode>();
        nodesByValue.Set(rootNode.Value, rootNode);

        foreach (var value in values)
        {
            if (!nodesByValue.HasKey(value))
            {
                nodesByValue.Set(value, new HammingNode(value));
            }
        }

        return nodesByValue;
    }

    private static void WireEdges(List<HammingNode> allNodes)
    {
        for (var i = 0; i < allNodes.Count; i++)
        {
            for (var j = i + 1; j < allNodes.Count; j++)
            {
                if (IsOneApart(allNodes[i].Value, allNodes[j].Value))
                {
                    allNodes[i].Neighbors.Add(allNodes[j]);
                    allNodes[j].Neighbors.Add(allNodes[i]);
                }
            }
        }
    }

    /// <summary>Looks up the node for a string.</summary>
    /// <param name="value">The string to look up.</param>
    /// <param name="node">The string's node when this returns true; meaningful only then.</param>
    /// <returns>True when the graph holds a node for <paramref name="value"/>; otherwise false.</returns>
    public bool TryGetNode(string value, out HammingNode node) =>
        _nodesByValue.TryGetValue(value, out node);

    /// <summary>Reports whether two equal-length strings differ in exactly one position.</summary>
    /// <param name="first">One string.</param>
    /// <param name="second">The other string, the same length as <paramref name="first"/>.</param>
    /// <returns>True when exactly one position differs; false when none or more than one do.</returns>
    public static bool IsOneApart(string first, string second)
    {
        var differences = 0;

        for (var i = 0; i < first.Length; i++)
        {
            if (first[i] != second[i] && ++differences > 1)
            {
                return false;
            }
        }

        return differences == 1;
    }

    // Every string one character away from `value` over `alphabet`, whether or not
    // it is a member of any graph - what a search that never materializes the
    // graph generates on the fly instead of following edges.
    /// <summary>Generates every string one character away from a value over an alphabet.</summary>
    /// <param name="value">The string to mutate.</param>
    /// <param name="alphabet">The characters each position may be changed to.</param>
    /// <returns>Each string with one position changed to a different alphabet character, generated lazily.</returns>
    public static IEnumerable<string> OneCharacterMutations(string value, Alphabet alphabet)
    {
        var buffer = value.ToCharArray();

        for (var position = 0; position < buffer.Length; position++)
        {
            var original = buffer[position];

            foreach (var replacement in alphabet.Characters)
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
}
