using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Kth Bit in Nth Binary String (LC 1545): brute-force materializing the whole
// binary string Sn via the literal S(n) = S(n-1) + "1" + invert(reverse(S(n-1)))
// recurrence (baseline - O(2^n) time and space, since Sn's length is 2^n - 1) vs.
// the well-known O(n) recursive bisection that locates bit k without ever building
// the string. No repo container or algorithm primitive applies to either side -
// each call in the smart approach makes exactly one recursive call, never two, so
// there are no overlapping subproblems for this repo's own Memoizer to help with,
// the same "lighter repo-primitive fit" case as PowXnBenchmarks. K is fixed at the
// last bit of Sn (2^n - 1) so both approaches do a full-depth walk: a complete
// string build for the baseline, the deepest possible recursion chain for the
// bisection.
[MemoryDiagnoser]
public class FindKthBitInNthBinaryStringBenchmarks
{
    private const string BaseCaseBit = "0"; // S(1)
    private const string MiddleBitSeparator = "1"; // S(n) = S(n-1) + "1" + invert(reverse(S(n-1)))
    private const int MidpointDivisor = 2;

    [Params(10, 20)]
    public int N;

    [Benchmark(Baseline = true)]
    public char BruteForceConstruction() => BuildNthString(N)[^1];

    private static string BuildNthString(int n)
    {
        if (n == 1)
        {
            return BaseCaseBit;
        }

        var previous = BuildNthString(n - 1);
        var invertedReversed = new char[previous.Length];

        for (var i = 0; i < previous.Length; i++)
        {
            invertedReversed[previous.Length - 1 - i] = previous[i] == '0' ? '1' : '0';
        }

        return previous + MiddleBitSeparator + new string(invertedReversed);
    }

    [Benchmark]
    public char RecursiveBisection() => FindKthBit(N, (1 << N) - 1);

    private static char FindKthBit(int n, int k)
    {
        if (n == 1)
        {
            return '0';
        }

        var length = (1 << n) - 1;
        var mid = (length / MidpointDivisor) + 1;

        if (k == mid)
        {
            return '1';
        }

        if (k < mid)
        {
            return FindKthBit(n - 1, k);
        }

        var mirroredBit = FindKthBit(n - 1, length - k + 1);
        return mirroredBit == '0' ? '1' : '0';
    }
}
