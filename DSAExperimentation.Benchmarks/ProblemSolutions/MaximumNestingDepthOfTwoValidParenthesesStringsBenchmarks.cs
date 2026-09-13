using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNestingDepthOfTwoValidParenthesesStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumNestingDepthOfTwoValidParenthesesStringsSolution's - the O(n^2) rescan
// baseline against the single-pass Stack<char> walk.
[MemoryDiagnoser]
public class MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarks
{
    // LC problem number, reused as the deterministic random seed.
    private const int RandomSeed = 1111;

    // The generated sequence is split evenly between '(' and ')'.
    private const int HalfLengthDivisor = 2;

    // Exclusive upper bound for the open/close coin flip (0 or 1).
    private const int CoinFlipBound = 2;

    [Params(200, 5_000)]
    public int Length;

    private string _sequence = null!;

    [GlobalSetup]
    public void Setup() => _sequence = Generate(new Random(RandomSeed), Length);

    [Benchmark(Baseline = true)]
    public int[] RecomputeDepthPerPosition() =>
        MaximumNestingDepthOfTwoValidParenthesesStringsSolution.MaxDepthAfterSplitByDepthRescan(_sequence);

    [Benchmark]
    public int[] StackTrackedSinglePass() =>
        MaximumNestingDepthOfTwoValidParenthesesStringsSolution.MaxDepthAfterSplitByOpenerStack(_sequence);

    // Workload sizing only: a random but always-valid parentheses sequence of the
    // requested length.
    private static string Generate(Random random, int length)
    {
        var builder = new System.Text.StringBuilder(length);
        var openRemaining = length / HalfLengthDivisor;
        var closeRemaining = length / HalfLengthDivisor;

        while (openRemaining > 0 || closeRemaining > 0)
        {
            if (openRemaining > 0 && (closeRemaining == openRemaining || random.Next(CoinFlipBound) == 0))
            {
                builder.Append('(');
                openRemaining--;
            }
            else
            {
                builder.Append(')');
                closeRemaining--;
            }
        }

        return builder.ToString();
    }
}
