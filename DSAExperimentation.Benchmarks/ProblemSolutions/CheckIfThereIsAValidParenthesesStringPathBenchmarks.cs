using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CheckIfThereIsAValidParenthesesStringPath;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfThereIsAValidParenthesesStringPathSolution's,
// the same methods CheckIfThereIsAValidParenthesesStringPathTests proves agree.
//
// The grid is all '(' so nothing short-circuits the un-memoized arm's full
// right/down branching early - the balance only ever grows, never goes negative, so
// it really does visit every one of the C(2n-2, n-1) paths. Size is kept modest for
// exactly that reason. Grid construction is the LeetCode input shape itself, so
// building it in [GlobalSetup] already keeps it off the measured methods.
[MemoryDiagnoser]
public class CheckIfThereIsAValidParenthesesStringPathBenchmarks
{
    private const char Open = '(';

    [Params(8, 12)]
    public int Size;

    private char[,] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        _grid = new char[Size, Size];

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                _grid[row, col] = Open;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public bool UnmemoizedRecursion() =>
        CheckIfThereIsAValidParenthesesStringPathSolution.HasValidPathByUnmemoizedRecursion(_grid);

    [Benchmark]
    public bool MemoizedRecursion() =>
        CheckIfThereIsAValidParenthesesStringPathSolution.HasValidPathByMemoizedRecursion(_grid);
}
