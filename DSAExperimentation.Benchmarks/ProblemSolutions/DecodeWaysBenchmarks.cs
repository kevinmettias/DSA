using DSAExperimentation.LeetCode.DecodeWays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DecodeWaysSolution's, the same methods
// DecodeWaysSolutionTests proves correct.
//
// A run of n '1's decodes Fib(n + 1) ways, one digit or a pair at every step, and LC 91
// promises an answer that fits in an int. So the run stops at 44 digits and the string
// goes on to LC 91's 100-digit cap in '7's: "17" still pairs, which makes the count
// Fib(46), the largest that fits, and a '7' pairs with nothing after it, so the count
// stays there however long the string grows.
public class DecodeWaysBenchmarks
{
    // The longest run of '1's the '7' after it keeps inside an int.
    private const int OnesRunCap = 44;

    private const char PairingDigit = '1';

    private const char LoneDigit = '7';

    private string _value = "";

    [Params(20, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var runLength = Math.Min(Length, OnesRunCap);
        _value = new string(PairingDigit, runLength).PadRight(Length, LoneDigit);
    }

    [Benchmark(Baseline = true)]
    public int Tabulation() => DecodeWaysSolution.CountDecodingsByTabulation(_value);

    [Benchmark]
    public int Memoized() => DecodeWaysSolution.CountDecodingsByMemoization(_value);
}
