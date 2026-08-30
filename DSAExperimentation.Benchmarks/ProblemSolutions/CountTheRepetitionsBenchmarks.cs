using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count The Repetitions (LC 466): the naive full O(N1 * |S1|) simulation - walk every
// copy of s1 one character at a time, the shape that times out on LeetCode's real
// n1 <= 10^6 constraint - vs. the intended greedy simulation with this repo's own
// HashMap<int,(int,int)> detecting a repeated "position within s2" state. A repeat is
// guaranteed within |S2| + 1 copies of s1 by pigeonhole, so HashMapCycleDetection's
// cost stays flat as N1 grows while NaiveFullSimulation scales linearly with it.
[MemoryDiagnoser]
public class CountTheRepetitionsBenchmarks
{
    private static readonly string S1 = string.Concat(Enumerable.Repeat("ab", 25)); // 50 chars
    private static readonly string S2Value = "ba";
    private const int N2 = 1;

    [Params(5_000, 100_000)]
    public int N1;

    [Benchmark(Baseline = true)]
    public int NaiveFullSimulation()
    {
        var s2Index = 0;
        var s2Count = 0;

        for (var copy = 0; copy < N1; copy++)
        {
            foreach (var c in S1)
            {
                if (c == S2Value[s2Index])
                {
                    s2Index++;
                    if (s2Index == S2Value.Length)
                    {
                        s2Index = 0;
                        s2Count++;
                    }
                }
            }
        }

        return s2Count / N2;
    }

    [Benchmark]
    public int HashMapCycleDetection() => GetMaxRepetitions(S1, N1, S2Value, N2);

    private static int GetMaxRepetitions(string s1, int n1, string s2, int n2)
    {
        if (n1 == 0)
        {
            return 0;
        }

        var s2Index = 0;
        var s2Count = 0;
        var s1Count = 0;
        var seen = new HashMap<int, (int S1Count, int S2Count)>();

        while (s1Count < n1)
        {
            foreach (var c in s1)
            {
                if (c == s2[s2Index])
                {
                    s2Index++;
                    if (s2Index == s2.Length)
                    {
                        s2Index = 0;
                        s2Count++;
                    }
                }
            }

            s1Count++;

            if (seen.TryGetValue(s2Index, out var prior))
            {
                var cycleS1Count = s1Count - prior.S1Count;
                var cycleS2Count = s2Count - prior.S2Count;
                var cycles = (n1 - s1Count) / cycleS1Count;

                s1Count += cycles * cycleS1Count;
                s2Count += cycles * cycleS2Count;
            }
            else
            {
                seen.Set(s2Index, (s1Count, s2Count));
            }
        }

        return s2Count / n2;
    }
}
