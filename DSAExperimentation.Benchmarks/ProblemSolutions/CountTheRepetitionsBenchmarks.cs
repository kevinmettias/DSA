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
    private const string S1BuildingBlock = "ab";
    private const int S1BuildingBlockRepeatCount = 25;

    private static readonly string S1 = BuildS1(); // 50 chars
    private static readonly string S2Value = "ba";
    private const int N2 = 1;

    [Params(5_000, 100_000)]
    public int N1;

    private static string BuildS1()
    {
        var repeatedPairs = Enumerable.Repeat(S1BuildingBlock, S1BuildingBlockRepeatCount);
        return string.Concat(repeatedPairs);
    }

    [Benchmark(Baseline = true)]
    public int NaiveFullSimulation()
    {
        var s2Index = 0;
        var s2Count = 0;

        for (var copy = 0; copy < N1; copy++)
        {
            ScanCopy(S1, S2Value, ref s2Index, ref s2Count);
        }

        return s2Count / N2;
    }

    private static void ScanCopy(string s1, string s2, ref int s2Index, ref int s2Count)
    {
        foreach (var c in s1)
        {
            if (c != s2[s2Index])
            {
                continue;
            }

            s2Index++;
            if (s2Index == s2.Length)
            {
                s2Index = 0;
                s2Count++;
            }
        }
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
            ScanCopy(s1, s2, ref s2Index, ref s2Count);
            s1Count++;
            (s1Count, s2Count) = RecordOrApplyCycle(seen, s2Index, n1, (s1Count, s2Count));
        }

        return s2Count / n2;
    }

    private static (int S1Count, int S2Count) RecordOrApplyCycle(
        HashMap<int, (int S1Count, int S2Count)> seen, int s2Index, int n1, (int S1Count, int S2Count) counts)
    {
        if (seen.TryGetValue(s2Index, out var prior))
        {
            return ApplyDetectedCycle(n1, counts.S1Count, counts.S2Count, prior);
        }

        seen.Set(s2Index, counts);
        return counts;
    }

    private static (int S1Count, int S2Count) ApplyDetectedCycle(
        int n1, int s1Count, int s2Count, (int S1Count, int S2Count) prior)
    {
        var cycleS1Count = s1Count - prior.S1Count;
        var cycleS2Count = s2Count - prior.S2Count;
        var cycles = (n1 - s1Count) / cycleS1Count;

        return (s1Count + cycles * cycleS1Count, s2Count + cycles * cycleS2Count);
    }
}
