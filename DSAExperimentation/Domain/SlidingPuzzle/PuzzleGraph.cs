using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Domain.SlidingPuzzle;

// The full state graph of a 2x3 sliding puzzle: all 720 permutations of "012345"
// ('0' standing in for the blank), each adjacent to the 2-3 boards reachable by
// one blank-tile slide. The 2x3 adjacency table below is fixed by the puzzle's
// own geometry, not by any one query about it - the same role LockGraph plays
// for LC 752's wheel-turn Cayley graph, just over a fixed board shape instead of
// a variable-size combination space.
//
// This is the domain model, not an answer to any one puzzle about it - it knows
// which boards are one slide apart and nothing about what a caller wants to find
// out. Callers supply their own start and target.
internal sealed class PuzzleGraph
{
    private static readonly int[][] Adjacency =
    [
        [1, 3],
        [0, 2, 4],
        [1, 5],
        [0, 4],
        [1, 3, 5],
        [2, 4],
    ];

    private readonly HashMap<string, PuzzleNode> _nodesByState;

    private PuzzleGraph(HashMap<string, PuzzleNode> nodesByState) => _nodesByState = nodesByState;

    // Every permutation of "012345" becomes a node, wired to its blank-slide neighbors.
    public static PuzzleGraph Build()
    {
        var nodesByState = BuildNodes();

        WireEdges(nodesByState);

        return new PuzzleGraph(nodesByState);
    }

    private static HashMap<string, PuzzleNode> BuildNodes()
    {
        var nodesByState = new HashMap<string, PuzzleNode>();

        foreach (var state in GenerateAllPermutations())
        {
            nodesByState.Set(state, new PuzzleNode(state));
        }

        return nodesByState;
    }

    private static List<string> GenerateAllPermutations()
    {
        var results = new List<string>();
        Permute(['0', '1', '2', '3', '4', '5'], 0, results);
        return results;
    }

    private static void WireEdges(HashMap<string, PuzzleNode> nodesByState)
    {
        foreach (var state in nodesByState.Keys)
        {
            nodesByState.TryGetValue(state, out var node);

            foreach (var neighborState in BlankSlideNeighbors(state))
            {
                if (nodesByState.TryGetValue(neighborState, out var neighborNode))
                {
                    node.Neighbors.Add(neighborNode);
                }
            }
        }
    }

    public bool TryGetNode(string state, out PuzzleNode node) => _nodesByState.TryGetValue(state, out node);

    // The 2-3 boards one blank-tile slide away - pure board arithmetic, so
    // callers that never materialize the graph can use it too.
    public static IEnumerable<string> BlankSlideNeighbors(string state)
    {
        var blank = state.IndexOf('0');

        foreach (var neighborIndex in Adjacency[blank])
        {
            yield return Swap(state, blank, neighborIndex);
        }
    }

    private static string Swap(string state, int i, int j)
    {
        var chars = state.ToCharArray();
        (chars[i], chars[j]) = (chars[j], chars[i]);
        return new string(chars);
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
}
