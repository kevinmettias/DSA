using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Domain.SlidingPuzzle;

namespace DSAExperimentation.LeetCode.SlidingPuzzle;

// LeetCode 773. Sliding Puzzle: fewest blank-tile slides from a given 2x3 board
// to "123450" ("board solved"), or -1 if the board's permutation parity makes it
// unreachable.
//
// Board permutations are nodes of Domain.SlidingPuzzle's implicit state graph, so
// the puzzle reduces to a shortest-path query on it - the same
// Reduce.Graph/DistanceMapReduceAlgebra/BreadthFirstReduceOrder composition
// OpenTheLockSolution uses for LC 752's wheel-turn graph, just over a 720-node
// board-permutation graph instead of a 10,000-combination lock. MinMovesByMutationQueue
// is the textbook alternative: BFS over boards mutated on the fly, never
// materializing the graph - the arm the composed solution has to justify itself
// against.
internal static class SlidingPuzzleSolution
{
    // LeetCode's own definition of "solved".
    private const string Target = "123450";

    // Deliberately written without this repo's primitives - BCL Queue + HashSet,
    // generating each candidate slide on the fly via PuzzleGraph's pure board
    // arithmetic (never the materialized graph) is what you would write without
    // this repo.
    public static int MinMovesByMutationQueue(int[][] board) => MinMovesByMutationQueue(Flatten(board));

    public static int MinMovesByMutationQueue(string start)
    {
        var visited = new HashSet<string> { start };
        var queue = new Queue<(string State, int Moves)>();
        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            var (state, moves) = queue.Dequeue();

            if (state == Target)
            {
                return moves;
            }

            ExpandNeighbors(state, moves, visited, queue);
        }

        return LeetCodeAnswer.None;
    }

    // Queues every slide of this board that the search has not reached yet.
    private static void ExpandNeighbors(
        string state,
        int moves,
        HashSet<string> visited,
        Queue<(string State, int Moves)> queue)
    {
        foreach (var neighbor in PuzzleGraph.BlankSlideNeighbors(state))
        {
            if (visited.Add(neighbor))
            {
                queue.Enqueue((neighbor, moves + 1));
            }
        }
    }

    // This repo's own BFS: Reduce.Graph in BreadthFirstReduceOrder with
    // DistanceMapReduceAlgebra is already exactly "distance from a root to every
    // node", so the puzzle reduces to one lookup in the result - the same
    // composition OpenTheLockSolution.MinTurnsByReduceGraph uses for LC 752.
    public static int MinMovesByReduceGraph(int[][] board)
    {
        var graph = PuzzleGraph.Build();

        return MinMovesByReduceGraph(graph, Flatten(board));
    }

    public static int MinMovesByReduceGraph(PuzzleGraph graph, string start)
    {
        if (!graph.TryGetNode(start, out var startNode) || !graph.TryGetNode(Target, out var targetNode))
        {
            return LeetCodeAnswer.None;
        }

        var distances = Reduce.Graph<
            PuzzleNode, PuzzleTopology, ListChildren<PuzzleNode>,
            NaturalChildOrder<PuzzleNode, ListChildren<PuzzleNode>>, ListChildren<PuzzleNode>,
            BreadthFirstReduceOrder<PuzzleNode>,
            DistanceMapReduceAlgebra<PuzzleNode>, Dictionary<PuzzleNode, int>>(startNode);

        return distances.TryGetValue(targetNode, out var distance) ? distance : LeetCodeAnswer.None;
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
