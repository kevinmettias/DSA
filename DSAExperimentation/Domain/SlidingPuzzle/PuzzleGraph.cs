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
/// <summary>Every board of the 2x3 sliding puzzle, each joined to the boards one blank-tile slide away.</summary>
public sealed class PuzzleGraph
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
    /// <summary>Builds the graph of all 720 boards.</summary>
    /// <returns>The graph, with each board joined to its blank-slide neighbors.</returns>
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

    /// <summary>Looks up the node for a board.</summary>
    /// <param name="state">
    /// The board read row by row, <c>'0'</c> standing for the blank, such as <c>"123450"</c>.
    /// </param>
    /// <param name="node">The board's node when it is one of the puzzle's boards; otherwise unspecified.</param>
    /// <returns>True when the board is a permutation of <c>"012345"</c>; otherwise false.</returns>
    public bool TryGetNode(string state, out PuzzleNode node) => _nodesByState.TryGetValue(state, out node);

    // The 2-3 boards one blank-tile slide away - the pure board arithmetic Build
    // wires its edges with.
    /// <summary>Lists the boards one blank-tile slide away.</summary>
    /// <param name="state">A board read row by row, <c>'0'</c> standing for the blank.</param>
    /// <returns>The two or three boards reached by sliding a tile next to the blank into it.</returns>
    public static IEnumerable<string> BlankSlideNeighbors(string state)
    {
        var blank = state.IndexOf('0');

        foreach (var neighborIndex in Adjacency[blank])
        {
            yield return Swap(state, blank, neighborIndex);
        }
    }

    private static string Swap(string state, int firstIndex, int secondIndex)
    {
        var chars = state.ToCharArray();
        (chars[firstIndex], chars[secondIndex]) = (chars[secondIndex], chars[firstIndex]);
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
