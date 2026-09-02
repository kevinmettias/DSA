using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheRepetitions;

// LeetCode 466. Count The Repetitions: greedily walks s2 across n1 copies of s1, using
// this repo's own HashMap<int,(int,int)> to remember, for every distinct "position
// within s2" the walk has previously reached, how many s1-copies and s2-completions it
// took to get there. Because that position only ever takes one of s2.Length values, a
// repeat is guaranteed within s2.Length + 1 copies of s1 (pigeonhole) - once the
// HashMap reports one, the remaining copies of s1 are fast-forwarded by whole cycles
// instead of being walked one character at a time, which is what makes n1 up to 10^6
// tractable.
public sealed class CountTheRepetitionsTests
{
    [Fact]
    public void GetMaxRepetitions_ClassicExample_ReturnsMaxRepeatCount()
    {
        var actual = GetMaxRepetitions("acb", 4, "ab", 2);
        Assert.Equal(2, actual);
    }

    [Fact]
    public void GetMaxRepetitions_S1EqualsS2_ReturnsN1()
    {
        var actual = GetMaxRepetitions("acb", 1, "acb", 1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void GetMaxRepetitions_S2NeverAppearsInS1_ReturnsZero()
    {
        var actual = GetMaxRepetitions("a", 3, "b", 1);
        Assert.Equal(0, actual);
    }

    [Fact]
    public void GetMaxRepetitions_LargeN1_CompletesQuicklyViaCycleDetection()
    {
        var actual = GetMaxRepetitions("acb", 1_000_000, "ab", 100);
        Assert.Equal(10_000, actual);
    }

    private static int GetMaxRepetitions(string s1, int n1, string s2, int n2)
    {
        if (n1 == 0)
        {
            return 0;
        }

        var s2Count = CountS2CompletionsAcrossN1Copies(s1, s2, n1);

        return s2Count / n2;
    }

    private static int CountS2CompletionsAcrossN1Copies(string s1, string s2, int n1)
    {
        var state = new RepetitionScanState();

        while (state.S1Count < n1)
        {
            state.Advance(s1, s2, n1);
        }

        return state.S2Count;
    }

    // Bundles the mutable counters the scan carries between s1 copies (s1Count,
    // s2Count, s2Index) alongside the "position within s2 -> counts seen at that
    // position" map that pigeonhole-detects a repeating cycle.
    private sealed class RepetitionScanState
    {
        private readonly HashMap<int, (int S1Count, int S2Count)> _seen = new();

        public int S1Count { get; private set; }

        public int S2Count { get; private set; }

        public int S2Index { get; private set; }

        public void Advance(string s1, string s2, int n1)
        {
            (S2Index, S2Count) = AdvanceThroughS1Copy(s1, s2, S2Index, S2Count);

            S1Count++;

            if (_seen.TryGetValue(S2Index, out var prior))
            {
                (S1Count, S2Count) = FastForwardCycles(S1Count, S2Count, n1, prior);
            }
            else
            {
                _seen.Set(S2Index, (S1Count, S2Count));
            }
        }
    }

    private static (int S2Index, int S2Count) AdvanceThroughS1Copy(string s1, string s2, int s2Index, int s2Count)
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

        return (s2Index, s2Count);
    }

    private static (int S1Count, int S2Count) FastForwardCycles(int s1Count, int s2Count, int n1, (int S1Count, int S2Count) prior)
    {
        var cycleS1Count = s1Count - prior.S1Count;
        var cycleS2Count = s2Count - prior.S2Count;
        var cycles = (n1 - s1Count) / cycleS1Count;

        return (s1Count + (cycles * cycleS1Count), s2Count + (cycles * cycleS2Count));
    }
}
