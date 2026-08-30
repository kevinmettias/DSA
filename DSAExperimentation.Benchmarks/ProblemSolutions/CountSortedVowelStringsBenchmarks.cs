using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Sorted Vowel Strings (LC 1641): BacktrackingEnumeration builds every
// actual non-decreasing vowel string and materializes it before counting -
// the textbook brute force, allocation-heavy in proportion to the final
// count - vs. MemoizedRecurrence, this repo's own Memoizer caching the
// (remaining length, smallest allowed vowel) recurrence down to O(n)
// distinct states with no string ever built, the same TopDownMemoized shape
// FibonacciBenchmarks already uses for its own recurrence. N is pushed past
// LeetCode's own n <= 33 constraint specifically so MemoryDiagnoser shows a
// real allocation gap between the two, not just a wall-clock one.
[MemoryDiagnoser]
public class CountSortedVowelStringsBenchmarks
{
    private static readonly char[] Vowels = ['a', 'e', 'i', 'o', 'u'];

    [Params(20, 35)]
    public int N;

    [Benchmark(Baseline = true)]
    public int BacktrackingEnumeration()
    {
        var results = new List<string>();
        var buffer = new char[N];
        Build(buffer, 0, 0, results);
        return results.Count;
    }

    private static void Build(char[] buffer, int position, int start, List<string> results)
    {
        if (position == buffer.Length)
        {
            results.Add(new string(buffer));
            return;
        }

        for (var vowel = start; vowel < Vowels.Length; vowel++)
        {
            buffer[position] = Vowels[vowel];
            Build(buffer, position + 1, vowel, results);
        }
    }

    [Benchmark]
    public int MemoizedRecurrence()
    {
        return Memoizer.Memoize<(int Remaining, int Start), int>((N, 0), Count);

        int Count((int Remaining, int Start) state, Func<(int Remaining, int Start), int> count)
        {
            var (remaining, start) = state;

            if (remaining == 0)
            {
                return 1;
            }

            var total = 0;

            for (var vowel = start; vowel < Vowels.Length; vowel++)
            {
                total += count((remaining - 1, vowel));
            }

            return total;
        }
    }
}
