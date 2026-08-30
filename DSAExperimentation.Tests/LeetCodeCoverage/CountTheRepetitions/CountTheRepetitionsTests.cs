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
        => Assert.Equal(2, GetMaxRepetitions("acb", 4, "ab", 2));

    [Fact]
    public void GetMaxRepetitions_S1EqualsS2_ReturnsN1()
        => Assert.Equal(1, GetMaxRepetitions("acb", 1, "acb", 1));

    [Fact]
    public void GetMaxRepetitions_S2NeverAppearsInS1_ReturnsZero()
        => Assert.Equal(0, GetMaxRepetitions("a", 3, "b", 1));

    [Fact]
    public void GetMaxRepetitions_LargeN1_CompletesQuicklyViaCycleDetection()
        => Assert.Equal(10_000, GetMaxRepetitions("acb", 1_000_000, "ab", 100));

    private static int GetMaxRepetitions(string s1, int n1, string s2, int n2)
    {
        if (n1 == 0)
        {
            return 0;
        }

        var s2Index = 0;
        var s2Count = 0;
        var s1Count = 0;

        // s2Index at the end of an s1 copy -> (s1Count, s2Count) at that same moment.
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
