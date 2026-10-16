using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.EscapeALargeMaze;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are EscapeALargeMazeSolution's capped search - the accepted
// algorithm - on LC 1036's real 10^6 x 10^6 board, as a BCL breadth-first walk and on
// this repo's implicit-graph traversal. Its old
// baseline flood-filled a materialized board, which cannot run on 10^12 cells, so it
// timed shrunken boards LeetCode never poses and was dropped. The search stops after
// BlockedCount * (BlockedCount - 1) / 2 cells from each end, so the blocked-cell count
// is what sets its cost, and BlockedCount runs to LC's 200.
//
// The blocked cells are scattered over a square at the source's corner, twice
// BlockedCount on a side, but never on row 0 or column 0: the source's edge row stays
// open, so the source always escapes, the far corner is untouched, and the answer is
// true by construction - while the cells still shape the search near the source.
public class EscapeALargeMazeBenchmarks
{
    private const int RandomSeed = 1036; // LC problem number
    private const int WindowPerBlockedCell = 2;

    private HashSet<(int Row, int Col)> _blockedHashSet = [];

    private Set<(int Row, int Col)> _blocked = new();
    private (int Row, int Col) _source;
    private (int Row, int Col) _target;

    [Params(50, 200)]
    public int BlockedCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var window = WindowPerBlockedCell * BlockedCount;
        var blocked = new Set<(int Row, int Col)>();
        var blockedHashSet = new HashSet<(int Row, int Col)>();

        // Each draw lands in both sets, so the two arms search the identical board.
        while (blocked.Count < BlockedCount)
        {
            var cell = (Row: random.Next(1, window), Col: random.Next(1, window));
            blocked.TryAdd(cell);
            blockedHashSet.Add(cell);
        }

        _blocked = blocked;
        _blockedHashSet = blockedHashSet;
        _source = (Row: 0, Col: 0);
        _target = (Row: EscapeALargeMazeBoard.Size - 1, Col: EscapeALargeMazeBoard.Size - 1);
    }

    [Benchmark(Baseline = true)]
    public bool CanEscapeByCappedBreadthFirstSearch() =>
        EscapeALargeMazeSolution.CanEscapeByCappedBreadthFirstSearch(
            _blockedHashSet, _source, _target, EscapeALargeMazeBoard.Size);

    [Benchmark]
    public bool CanEscapeByCappedTraversal() =>
        EscapeALargeMazeSolution.CanEscapeByCappedTraversal(_blocked, _source, _target, EscapeALargeMazeBoard.Size);
}
