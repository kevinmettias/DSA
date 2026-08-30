namespace DSAExperimentation.Benchmarks.Fixtures;

// Builds a Sliding Puzzle scenario (LC 773): all 720 permutations of "012345" ('0'
// stands in for the blank), 2-3 candidate blank-tile-slide edges per node - the same
// "materialize every state, then wire neighbors" construction LockGraphs.BuildGraph
// uses for LC 752's 10,000-combination graph, just over a fixed 6-cell board instead
// of a variable-size Cayley graph.
internal static class PuzzleGraphs
{
    public const string Target = "123450";

    private static readonly int[][] Adjacency =
    [
        [1, 3],
        [0, 2, 4],
        [1, 5],
        [0, 4],
        [1, 3, 5],
        [2, 4],
    ];

    public static (Dictionary<string, PuzzleNode> NodesByState, PuzzleNode StartNode) BuildGraph(string start)
    {
        var nodesByState = new Dictionary<string, PuzzleNode>();

        foreach (var state in GenerateAllPermutations())
        {
            nodesByState[state] = new PuzzleNode(state);
        }

        foreach (var node in nodesByState.Values)
        {
            foreach (var neighborState in BlankSlideNeighbors(node.State))
            {
                if (nodesByState.TryGetValue(neighborState, out var neighborNode))
                {
                    node.Neighbors.Add(neighborNode);
                }
            }
        }

        return (nodesByState, nodesByState[start]);
    }

    public static IEnumerable<string> BlankSlideNeighbors(string state)
    {
        var blank = state.IndexOf('0');

        foreach (var neighborIndex in Adjacency[blank])
        {
            yield return Swap(state, blank, neighborIndex);
        }
    }

    private static List<string> GenerateAllPermutations()
    {
        var results = new List<string>();
        Permute(['0', '1', '2', '3', '4', '5'], 0, results);
        return results;
    }

    private static void Permute(char[] chars, int start, List<string> results)
    {
        if (start == chars.Length)
        {
            results.Add(new string(chars));
            return;
        }

        for (var i = start; i < chars.Length; i++)
        {
            (chars[start], chars[i]) = (chars[i], chars[start]);
            Permute(chars, start + 1, results);
            (chars[start], chars[i]) = (chars[i], chars[start]);
        }
    }

    private static string Swap(string state, int i, int j)
    {
        var chars = state.ToCharArray();
        (chars[i], chars[j]) = (chars[j], chars[i]);
        return new string(chars);
    }
}
