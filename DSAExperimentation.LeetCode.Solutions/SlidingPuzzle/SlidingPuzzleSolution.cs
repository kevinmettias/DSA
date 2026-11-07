using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.States;
using DSAExperimentation.Domain.Games.SlidingPuzzle;

namespace DSAExperimentation.LeetCode.Solutions.SlidingPuzzle;

// LeetCode 773. Sliding Puzzle: fewest blank-tile slides from a given 2x3 board
// to "123450" ("board solved"), or -1 if the board's permutation parity makes it
// unreachable.
//
// Board arrangements are the states of the general sliding puzzle
// (Domain/Games/SlidingPuzzle) on LC 773's own 2x3 board, so the puzzle reduces to a
// shortest-path query on their state graph - the same BreadthFirstDistances
// composition OpenTheLockSolution uses for LC 752's wheel-turn graph, just over a
// 720-node board-permutation graph instead of a 10,000-combination lock.
// MinMovesByMutationQueue is the textbook alternative: BFS over boards slid on the
// fly, never materializing the graph - the arm the composed solution has to justify
// itself against. Both arms read the puzzle's rules from SlidingPuzzle: how a board
// is spelled, which boards one slide reaches, and what "solved" means (ARCHITECTURE
// §17.5).
[LeetCodeQuestion("sliding-puzzle", LeetCodeAnswerOrder.AsWritten)]
internal static class SlidingPuzzleSolution
{
    // Deliberately written without this repo's primitives - BCL Queue + HashSet,
    // asking the puzzle for each board's slides as the search reaches it, is what you
    // would write without this repo.
    public static int MinMovesByMutationQueue(int[][] board) => MinMovesByMutationQueue(Flatten(board));

    public static int MinMovesByMutationQueue(string start)
    {
        var visited = new HashSet<string> { start };
        var queue = new Queue<(string State, int Moves)>();
        var slides = new string[SlidingPuzzle<TwoByThreeBoard>.MostSlides];
        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            var (state, moves) = queue.Dequeue();

            if (state == SlidingPuzzle<TwoByThreeBoard>.Solved)
            {
                return moves;
            }

            var count = SlidingPuzzle<TwoByThreeBoard>.WriteOneSlideAway(state, slides);
            var neighbors = slides.AsSpan(0, count);
            QueueUnreached(neighbors, moves + 1, visited, queue);
        }

        return LeetCodeAnswer.None;
    }

    // Queues every board one slide away that the search has not reached yet.
    private static void QueueUnreached(
        ReadOnlySpan<string> neighbors,
        int neighborMoves,
        HashSet<string> visited,
        Queue<(string State, int Moves)> queue)
    {
        foreach (var neighbor in neighbors)
        {
            if (visited.Add(neighbor))
            {
                queue.Enqueue((neighbor, neighborMoves));
            }
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
        var chars = new char[SlidingPuzzle<TwoByThreeBoard>.CellCount];
        var index = 0;

        foreach (var row in board)
        {
            foreach (var cell in row)
            {
                chars[index++] = SlidingPuzzle<TwoByThreeBoard>.CellSymbols[cell];
            }
        }

        return new string(chars);
    }

    // The composed arm's input: all 720 arrangements of LC 773's board, each joined to those one
    // slide away. Public so a benchmark can build it outside the timed region (17.4).
    public static StateGraph<string> BuildPuzzleGraph()
        => StateGraph<string>.Build(new PuzzleSlides<TwoByThreeBoard>(), new EveryState<string>());
}
