using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.Tests.LeetCodeCoverage.SlidingPuzzle.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SlidingPuzzle;

// LeetCode 773. Sliding Puzzle: board permutations are nodes of an implicit graph,
// with an edge between any two boards exactly one blank-tile slide apart. The fewest
// slides to reach "123450" is exactly Reduce.Graph's own BFS distance from the start
// board - the identical DistanceMapReduceAlgebra/BreadthFirstReduceOrder composition
// OpenTheLockTests already uses for LC 752's wheel-turn graph, just over a 6-cell
// adjacency table instead of 4-wheel arithmetic.
public sealed partial class SlidingPuzzleTests
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

    [Fact]
    public void MinMoves_OneSlideFromSolved_ReturnsOne()
    {
        var moves = SlidingPuzzle([[1, 2, 3], [4, 0, 5]]);

        Assert.Equal(1, moves);
    }

    [Fact]
    public void MinMoves_OppositeParityFromSolved_ReturnsMinusOne()
    {
        var moves = SlidingPuzzle([[1, 2, 3], [5, 4, 0]]);

        Assert.Equal(-1, moves);
    }

    [Fact]
    public void MinMoves_FiveSlidesFromSolved_ReturnsFive()
    {
        var moves = SlidingPuzzle([[4, 1, 2], [5, 0, 3]]);

        Assert.Equal(5, moves);
    }

    private static int SlidingPuzzle(int[][] board)
    {
        const string target = "123450";
        var start = Flatten(board);
        var nodesByState = BuildGraph();

        if (!nodesByState.TryGetValue(start, out var startNode) ||
            !nodesByState.TryGetValue(target, out var targetNode))
        {
            return -1;
        }

        var distances = Reduce.Graph<
            PuzzleNode, PuzzleTopology, ListChildren<PuzzleNode>,
            NaturalChildOrder<PuzzleNode, ListChildren<PuzzleNode>>, ListChildren<PuzzleNode>,
            BreadthFirstReduceOrder<PuzzleNode>,
            DistanceMapReduceAlgebra<PuzzleNode>, Dictionary<PuzzleNode, int>>(startNode);

        return distances.TryGetValue(targetNode, out var distance) ? distance : -1;
    }

    // Every permutation of "012345" becomes a node ('0' stands in for the blank).
    private static HashMap<string, PuzzleNode> BuildGraph()
    {
        var nodesByState = CreateNodes();
        WireNeighbors(nodesByState);
        return nodesByState;
    }

    private static HashMap<string, PuzzleNode> CreateNodes()
    {
        var nodesByState = new HashMap<string, PuzzleNode>();

        foreach (var state in GenerateAllPermutations())
        {
            nodesByState.Set(state, new PuzzleNode(state));
        }

        return nodesByState;
    }

    private static void WireNeighbors(HashMap<string, PuzzleNode> nodesByState)
    {
        foreach (var state in nodesByState.Keys)
        {
            nodesByState.TryGetValue(state, out var node);
            var blank = state.IndexOf('0');

            foreach (var neighborIndex in Adjacency[blank])
            {
                var neighborState = Swap(state, blank, neighborIndex);

                if (nodesByState.TryGetValue(neighborState, out var neighborNode))
                {
                    node.Neighbors.Add(neighborNode);
                }
            }
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

    private static string Flatten(int[][] board)
    {
        var chars = new char[6];
        var index = 0;

        foreach (var row in board)
        {
            foreach (var cell in row)
            {
                chars[index++] = (char)('0' + cell);
            }
        }

        return new string(chars);
    }
}
