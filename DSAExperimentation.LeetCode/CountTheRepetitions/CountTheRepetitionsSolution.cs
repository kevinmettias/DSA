using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CountTheRepetitions;

// LeetCode 466. Count The Repetitions: how many times str2 (n2 copies of s2) can be
// obtained as a subsequence of str1 (n1 copies of s1), i.e. the greatest m such that
// str2 repeated m times is a subsequence of str1 - computed as (completions of s2
// found while walking n1 copies of s1) / n2.
//
// GetMaxRepetitionsByNaiveSimulation is the textbook O(n1 * |s1|) full walk that
// times out on LeetCode's real n1 <= 10^6 constraint - deliberately written without
// this repo's primitives, the arm the composed solution below has to justify itself
// against. GetMaxRepetitionsByHashMapCycleDetection is the composed answer: this
// repo's own HashMap<int,(int,int)> remembers, for every distinct "position within
// s2" the walk has previously reached, how many s1-copies and s2-completions it took
// to get there. That position only ever takes one of |s2| values, so a repeat is
// guaranteed within |s2| + 1 copies of s1 (pigeonhole) - once the HashMap reports
// one, the remaining copies of s1 are fast-forwarded by whole cycles instead of
// being walked one character at a time.
internal static class CountTheRepetitionsSolution
{
    public static int GetMaxRepetitionsByNaiveSimulation(string s1, int n1, string s2, int n2)
    {
        if (n1 == 0)
        {
            return 0;
        }

        var s2Index = 0;
        var s2Count = 0;

        for (var copy = 0; copy < n1; copy++)
        {
            ScanOneS1Copy(s1, s2, ref s2Index, ref s2Count);
        }

        return s2Count / n2;
    }

    public static int GetMaxRepetitionsByHashMapCycleDetection(string s1, int n1, string s2, int n2)
    {
        if (n1 == 0)
        {
            return 0;
        }

        var seen = new HashMap<int, (int S1Count, int S2Count)>();
        var s2Index = 0;
        var s2Count = 0;
        var s1Count = 0;

        while (s1Count < n1)
        {
            ScanOneS1Copy(s1, s2, ref s2Index, ref s2Count);
            s1Count++;

            if (seen.TryGetValue(s2Index, out var prior))
            {
                (s1Count, s2Count) = FastForwardCycles(n1, s1Count, s2Count, prior);
            }
            else
            {
                seen.Set(s2Index, (s1Count, s2Count));
            }
        }

        return s2Count / n2;
    }

    // The scan step shared by both strategies: walk one copy of s1 forward through
    // s2's remaining characters, advancing s2Index and counting each full pass
    // through s2 as one completion.
    private static void ScanOneS1Copy(string s1, string s2, ref int s2Index, ref int s2Count)
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

    private static (int S1Count, int S2Count) FastForwardCycles(
        int n1, int s1Count, int s2Count, (int S1Count, int S2Count) prior)
    {
        var cycleS1Count = s1Count - prior.S1Count;
        var cycleS2Count = s2Count - prior.S2Count;
        var cycles = (n1 - s1Count) / cycleS1Count;

        return (s1Count + (cycles * cycleS1Count), s2Count + (cycles * cycleS2Count));
    }
}
