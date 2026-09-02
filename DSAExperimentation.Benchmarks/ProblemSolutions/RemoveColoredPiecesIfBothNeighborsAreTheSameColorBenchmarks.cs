using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Buffers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Colored Pieces if Both Neighbors are the Same Color (LC 2038):
// NaiveGameSimulation actually plays the game out move by move - scanning for the
// next removable piece and removing it from a List<char> on every turn, O(n^2)
// overall - against RunLengthCounting, which recognizes that a run of length L
// always yields exactly max(L-2, 0) total moves regardless of removal order, so
// this repo's own ContiguousGroupBuffer (grouping the string into maximal
// same-color runs, the same primitive ContiguousGroupBufferTests exercises
// directly) computes both players' full move budgets in one O(n) pass.
[MemoryDiagnoser]
public class RemoveColoredPiecesIfBothNeighborsAreTheSameColorBenchmarks
{
    private const int ColorGroupCount = 2; // two color runs: 'A' then 'B'
    private const int RunEndpointCount = 2; // a run's two endpoints never have both same-colored neighbors, so they contribute no moves

    [Params(200, 5_000)]
    public int Length;

    private string _colors = null!;

    [GlobalSetup]
    public void Setup()
    {
        // One long run of 'A' followed by one long run of 'B' maximizes the total
        // number of moves available to both players, forcing NaiveGameSimulation
        // through its full O(n^2) worst case instead of stopping after a handful
        // of turns.
        var half = Length / ColorGroupCount;
        _colors = new string('A', half) + new string('B', Length - half);
    }

    [Benchmark(Baseline = true)]
    public bool NaiveGameSimulation()
    {
        var pieces = _colors.ToList();
        var aliceTurn = true;

        while (true)
        {
            var target = aliceTurn ? 'A' : 'B';
            var moveIndex = FindRemovableIndex(pieces, target);

            if (moveIndex < 0)
            {
                return !aliceTurn;
            }

            pieces.RemoveAt(moveIndex);
            aliceTurn = !aliceTurn;
        }
    }

    private static int FindRemovableIndex(List<char> pieces, char target)
    {
        for (var i = 1; i < pieces.Count - 1; i++)
        {
            if (pieces[i] == target && pieces[i - 1] == target && pieces[i + 1] == target)
            {
                return i;
            }
        }

        return -1;
    }

    [Benchmark]
    public bool RunLengthCounting()
    {
        var buffer = new ContiguousGroupBuffer<char, char>();
        var budgets = new Dictionary<char, int> { ['A'] = 0, ['B'] = 0 };

        void Accumulate(IReadOnlyList<char> items, char key) => budgets[key] += Math.Max(0, items.Count - RunEndpointCount);

        foreach (var color in _colors)
        {
            buffer.Add(color, color, Accumulate);
        }

        buffer.Flush(Accumulate);

        return budgets['A'] > budgets['B'];
    }
}
