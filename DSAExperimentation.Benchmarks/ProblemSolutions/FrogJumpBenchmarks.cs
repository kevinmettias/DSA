using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FrogJump;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FrogJumpSolution's, the same methods
// FrogJumpTests proves correct. _stones is built as consecutive integers
// (every jump delta from every reachable stone lands on another real stone)
// with an unreachable final stone appended far away - the same "rig the
// input so both strategies are forced through their full worst case" trick
// TwoSumBenchmarks/JumpGameBenchmarks use, here forcing
// CanCrossByRecursiveBruteForce through its full exponential search instead
// of returning early on success.
[MemoryDiagnoser]
public class FrogJumpBenchmarks
{
    // Offset from StoneCount back to the value of the last consecutively-filled
    // stone (the loop fills indices [0, StoneCount - 2] with their own index).
    private const int LastConsecutiveStoneOffset = 2;

    // Pushed onto the last consecutive stone's value so the appended final stone
    // sits far enough away that no jump size can ever reach it.
    private const int UnreachableStoneGap = 1_000;

    [Params(10, 16)]
    public int StoneCount;

    private int[] _stones = null!;

    [GlobalSetup]
    public void Setup()
    {
        _stones = new int[StoneCount];

        for (var i = 0; i < StoneCount - 1; i++)
        {
            _stones[i] = i;
        }

        _stones[StoneCount - 1] = StoneCount - LastConsecutiveStoneOffset + UnreachableStoneGap;
    }

    [Benchmark(Baseline = true)]
    public bool RecursiveBruteForce() => FrogJumpSolution.CanCrossByRecursiveBruteForce(_stones);

    [Benchmark]
    public bool HashMapDynamicProgramming() => FrogJumpSolution.CanCrossByHashMapDynamicProgramming(_stones);
}
