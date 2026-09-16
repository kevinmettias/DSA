using DSAExperimentation.DataStructures.AhoCorasick;
using DSAExperimentation.LeetCode.ConstructStringWithMinimumCost;

namespace DSAExperimentation.LeetCode.Tests.ConstructStringWithMinimumCost;

// The seam between ConstructStringWithMinimumCostSolution's two DPs.
// MinCostByBruteForceDp tries every word at every position by direct character
// comparison. MinCostByAhoCorasickDp first folds the word list down through a
// DataStructures.HashMap<string,int> so a word spelled twice keeps only its cheapest
// cost, then feeds the distinct words to this repo's own DataStructures.AhoCorasick
// automaton and runs the same DP over the matches FindAll reports.
//
// The DP reads dp[match.Start] as final the moment it sees the match, which is only
// true because FindAll returns its matches in ascending Start order - a match can
// only ever feed a later dp slot than the one it reads, so once the matches arrive
// sorted there is nothing left to propagate. That ordering is the seam: it is a
// documented contract of the DSA structure that this solution's correctness silently
// rests on and never checks, so it is asserted here directly.
public sealed partial class AhoCorasickDpSeamTests
{
    [Fact]
    public void MinCost_LeetCodeExample_MatchesBruteForceDp()
    {
        string[] words = ["abdef", "abc", "d", "def", "ef"];
        int[] costs = [100, 1, 1, 10, 5];

        Assert.Equal(7, MinCostByAutomatonArm("abcdef", words, costs));
        AssertSameCost("abcdef", words, costs);
    }

    [Fact]
    public void MinCost_TargetNoWordMatches_ReturnsNone()
    {
        string[] words = ["z", "zz", "zzz"];
        int[] costs = [1, 10, 100];

        Assert.Equal(-1, MinCostByAutomatonArm("aaaa", words, costs));
        AssertSameCost("aaaa", words, costs);
    }

    // The contract the DP above is written against: FindAll's matches arrive in
    // ascending Start order. The reported order cannot be read off the scan position
    // alone once patterns of different lengths overlap, so this is the automaton's own
    // sort guarantee and not a property of when each match was discovered - and a DP
    // that relaxed a slot after it had already been read would silently under-report.
    [Fact]
    public void FindAll_OverlappingPatternsOfDifferentLengths_ReportsAscendingStartOrder()
    {
        string[] patterns = ["a", "aa", "aaa", "b"];
        var automaton = new AhoCorasick(patterns);

        var matches = automaton.FindAll("aaaab");

        Assert.Equal([0, 0, 0, 1, 1, 1, 2, 2, 3, 4], matches.Select(match => match.Start));
    }

    // Every match's PatternIndex has to address the pattern list the automaton was
    // built from, because the DP reads the matched word's own length through it and
    // the two must agree for the end position to land where the match ends.
    [Fact]
    public void FindAll_ReportsPatternIndexWhoseWordMatchesTheTextAtStart()
    {
        string[] patterns = ["xyz", "y", "z", "ab"];
        var automaton = new AhoCorasick(patterns);

        foreach (var match in automaton.FindAll("xyzab"))
        {
            Assert.Equal(
                patterns[match.PatternIndex],
                "xyzab".Substring(match.Start, patterns[match.PatternIndex].Length));
        }
    }

    // The same word spelled twice with different costs: only the cheaper spelling may
    // reach the automaton, and the cheap one is listed second, so a fold that kept the
    // first spelling seen would answer with the expensive cost.
    [Fact]
    public void MinCost_DuplicateWordCheaperWhenListedSecond_MatchesBruteForceDp()
    {
        string[] words = ["ab", "ab"];
        int[] costs = [9, 2];

        Assert.Equal(2, MinCostByAutomatonArm("ab", words, costs));
        AssertSameCost("ab", words, costs);
    }

    // Two distinct words and a duplicate sharing one spelling: the HashMap fold has to
    // keep both distinct keys while collapsing the repeat.
    [Fact]
    public void MinCost_DuplicateWordInTheMiddleOfADistinctList_MatchesBruteForceDp()
    {
        string[] words = ["a", "b", "ab", "a", "ba"];
        int[] costs = [4, 2, 6, 1, 2];

        Assert.Equal(3, MinCostByAutomatonArm("ab", words, costs));
        AssertSameCost("ab", words, costs);
    }

    // Every word is one character and the cheapest spelling of each, so the answer is
    // the sum of the cheapest single-character costs - the shape where the HashMap
    // fold decides the whole answer.
    [Fact]
    public void MinCost_RepeatedSingleCharacterWords_SumsTheCheapestSpelling()
    {
        string[] words = ["a", "b", "a", "b", "c"];
        int[] costs = [3, 4, 1, 2, 7];

        Assert.Equal(10, MinCostByAutomatonArm("abc", words, costs));
        AssertSameCost("abc", words, costs);
    }

    // A word longer than the target can never match, and the automaton still has to
    // build with it in the pattern set: the scan of a shorter text never reaches the
    // node, so it contributes no match rather than an out-of-range start.
    [Fact]
    public void MinCost_WordLongerThanTarget_MatchesBruteForceDp()
    {
        string[] words = ["abcdefgh", "abc", "de"];
        int[] costs = [1, 4, 8];

        Assert.Equal(12, MinCostByAutomatonArm("abcde", words, costs));
        AssertSameCost("abcde", words, costs);
    }

    // Overlapping matches that only a shortest-path over match positions resolves: the
    // cheapest cover of the target is not the cheapest first match.
    [Fact]
    public void MinCost_OverlappingMatchesWhereTheCheapestPrefixIsNotOptimal_MatchesBruteForceDp()
    {
        string[] words = ["ab", "bc", "abc", "c"];
        int[] costs = [1, 1, 5, 1];

        Assert.Equal(2, MinCostByAutomatonArm("abc", words, costs));
        AssertSameCost("abc", words, costs);
    }

    // One character of target, one word that is exactly it: the smallest input the DP
    // has a dp slot to relax at all.
    [Fact]
    public void MinCost_TargetOfOneMatchingCharacter_MatchesBruteForceDp()
    {
        string[] words = ["a", "b"];

        Assert.Equal(6, MinCostByAutomatonArm("a", words, [6, 1]));
        AssertSameCost("a", words, [6, 1]);
    }

    private static void AssertSameCost(string target, string[] words, int[] costs)
        => Assert.Equal(
            ConstructStringWithMinimumCostSolution.MinCostByBruteForceDp(target, words, costs),
            MinCostByAutomatonArm(target, words, costs));

    private static int MinCostByAutomatonArm(string target, string[] words, int[] costs)
        => ConstructStringWithMinimumCostSolution.MinCostByAhoCorasickDp(target, words, costs);
}
