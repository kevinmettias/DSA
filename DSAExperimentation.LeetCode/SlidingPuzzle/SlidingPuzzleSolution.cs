using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.States;
using DSAExperimentation.Domain.Games.SlidingPuzzle;

namespace DSAExperimentation.LeetCode.SlidingPuzzle;

// LeetCode 773. Sliding Puzzle: fewest blank-tile slides from a given 2x3 board
// to "123450" ("board solved"), or -1 if the board's permutation parity makes it
// unreachable.
//
// Board arrangements are the states of the general sliding puzzle
// (Domain/Games/SlidingPuzzle) on LC 773's own 2x3 board, so the puzzle reduces to a
// shortest-path query on their state graph - the same BreadthFirstDistances
// composition OpenTheLockSolution uses for LC 752's wheel-turn graph, just over a
// 720-node board-permutation graph instead of a 10,000-combination lock.
// MinMovesByMutationQueue is the textbook alternative: BFS over boards mutated on
// the fly, never materializing the graph - the arm the composed solution has to
// justify itself against.
internal static class SlidingPuzzleSolution
{
    // LeetCode's own definition of "solved".
    private const string Target = "123450";

    // The empty cell, the only tile that moves.
    private const char Blank = '0';

    // The cells a blank at each position of the row-major 2x3 board can slide into.
    private static readonly int[][] SlideTargets =
    [
        [1, 3],
        [0, 2, 4],
        [1, 5],
        [0, 4],
        [1, 3, 5],
        [2, 4],
    ];

    // Deliberately written without this repo's primitives - BCL Queue + HashSet,
    // generating each candidate slide on the fly with its own board arithmetic rather
    // than SlidingPuzzle's, whose model a baseline may not read (ARCHITECTURE 17.5), is
    // what you would write without this repo.
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
        foreach (var neighbor in OneSlideAway(state))
        {
            if (visited.Add(neighbor))
            {
                queue.Enqueue((neighbor, moves + 1));
            }
        }
    }

    // Every board one slide of the blank away, in the order SlidingPuzzle writes them.
    private static IEnumerable<string> OneSlideAway(string state)
    {
        var blank = state.IndexOf(Blank);

        foreach (var target in SlideTargets[blank])
        {
            var cells = state.ToCharArray();
            (cells[blank], cells[target]) = (cells[target], cells[blank]);

            yield return new string(cells);
        }
    }

    // This repo's own BFS: BreadthFirstDistances is already exactly "distance from
    // a root to every node", so the puzzle reduces to one lookup in the result -
    // the same composition OpenTheLockSolution.MinTurnsByReduceGraph uses for LC 752.
    public static int MinMovesByReduceGraph(int[][] board)
    {
        var graph = BuildPuzzleGraph();

        return MinMovesByReduceGraph(graph, Flatten(board));
    }

    public static int MinMovesByReduceGraph(StateGraph<string> graph, string start)
    {
        if (!graph.TryGetNode(start, out var startNode)
            || !graph.TryGetNode(SlidingPuzzle<TwoByThreeBoard>.Solved, out var targetNode))
        {
            return LeetCodeAnswer.None;
        }

        return BreadthFirstDistances.To<StateNode<string>, StateTopology<string>, ListChildren<StateNode<string>>>(startNode, targetNode)
            ?? LeetCodeAnswer.None;
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

    // The composed arm's input: all 720 arrangements of LC 773's board, each joined to those one
    // slide away. Public so a benchmark can build it outside the timed region (17.4).
    public static StateGraph<string> BuildPuzzleGraph()
        => StateGraph<string>.Build(new PuzzleSlides<TwoByThreeBoard>(), new EveryState<string>());
}
