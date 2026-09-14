using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CatAndMouseII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CatAndMouseIISolution's, the same methods
// CatAndMouseIITests proves correct - the textbook unmemoized minimax recursion,
// re-exploring every repeated board position from scratch, vs. the identical
// recurrence routed through this repo's own Memoizer. The workload is a single-row
// corridor (Food at one end, Cat in the middle, Mouse at the far end) so both
// players' positions - and therefore the distinct state count - stay small while
// CorridorLength still grows the unmemoized search tree. Grid construction is
// charged to [GlobalSetup]; the measured methods take LeetCode's own input shape,
// so no hoisted overload is needed.
[MemoryDiagnoser]
public class CatAndMouseIIBenchmarks
{
    private const int CatJump = 1;
    private const int MouseJump = 1;

    private const char Empty = '.';
    private const char Food = 'F';
    private const char Cat = 'C';
    private const char Mouse = 'M';

    // Cat starts halfway along the corridor.
    private const int CorridorMidpointDivisor = 2;

    [Params(4, 6)]
    public int CorridorLength;

    private string[] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var corridor = new char[CorridorLength];
        Array.Fill(corridor, Empty);
        corridor[0] = Food;
        corridor[CorridorLength / CorridorMidpointDivisor] = Cat;
        corridor[CorridorLength - 1] = Mouse;
        _grid = [new string(corridor)];
    }

    [Benchmark(Baseline = true)]
    public bool UnmemoizedRecursion() =>
        CatAndMouseIISolution.CanMouseWinByExhaustiveRecursion(_grid, CatJump, MouseJump);

    [Benchmark]
    public bool MemoizedRecursion() =>
        CatAndMouseIISolution.CanMouseWinByMemoizedRecursion(_grid, CatJump, MouseJump);
}
