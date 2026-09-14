using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PalindromePartitioningIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromePartitioningIVSolution's, the same methods
// PalindromePartitioningIVTests proves correct. _s is
// "a"*BlockSize + "b" + "a"*BlockSize + "c": the trailing "c" forces the third
// partition to be exactly "c" (it's the only "c" in the string), which leaves
// "a"*BlockSize + "b" + "a"*BlockSize needing a 2-way palindrome split that provably
// has none (a valid split would require the two "a" runs flanking "b" to have equal
// length on both sides of every candidate cut, which never happens) - so the answer
// always resolves to false only after exhausting every candidate, forcing both
// strategies through their full worst-case search instead of an early exit making
// the unmemoized version look artificially competitive. Many different first/second
// cut choices land on the same later (Position, 1) state, so the unmemoized version
// re-explores it once per incoming path while the memoized version resolves it
// exactly once.
[MemoryDiagnoser]
public class PalindromePartitioningIVBenchmarks
{
    private const string MiddleSeparator = "b";
    private const string UniqueTrailingCharacter = "c";

    [Params(30, 100)]
    public int BlockSize;

    private string _s = null!;

    [GlobalSetup]
    public void Setup() => _s = new string('a', BlockSize) + MiddleSeparator + new string('a', BlockSize) + UniqueTrailingCharacter;

    [Benchmark(Baseline = true)]
    public bool Unmemoized() => PalindromePartitioningIVSolution.CheckPartitioningByNaiveRecursion(_s);

    [Benchmark]
    public bool Memoized() => PalindromePartitioningIVSolution.CheckPartitioningByMemoizedRecurrence(_s);
}
