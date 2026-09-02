using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

// LeetCode 1467. Probability of a Two Boxes Having The Same Number of Distinct
// Balls: Backtrack.Search (Algorithms/Backtracking/Backtrack.cs) walks every way
// to split each ball type's copies between the two boxes - one Candidates step
// per type, offering "send k of this type's copies to box 1" for every legal k.
// Choose/Unchoose maintain box 1's running ball count, each box's running
// distinct-type count, and the combinatorial weight (running product of
// C(balls[i], k) over the types decided so far) of the split explored so far,
// undoing all four exactly on backtrack. onSolution - reached once every type has
// been assigned - adds a leaf split's weight into the matching total whenever
// both boxes end up with half the balls AND an equal number of distinct types.
// Dividing by C(total, total/2) (every equally likely half/half split) is the
// probability LeetCode asks for - this is the same backtracking-with-numeric-
// payload shape as Combination Sum/N-Queens' solution count, just counting a
// weighted total instead of solutions.
public sealed partial class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsTests
{
    [Fact]
    public void GetProbability_TwoSingletonTypes_AlwaysMatches()
    {
        Assert.Equal(1.0, GetProbability([1, 1]), precision: 9);
    }

    [Fact]
    public void GetProbability_ClassicThreeTypeExample_ReturnsTwoThirds()
    {
        Assert.Equal(2.0 / 3.0, GetProbability([2, 1, 1]), precision: 9);
    }

    [Fact]
    public void GetProbability_FourTypeExample_ReturnsThreeFifths()
    {
        Assert.Equal(0.6, GetProbability([1, 2, 1, 2]), precision: 9);
    }

    [Fact]
    public void GetProbability_ThreeTypeDescendingCounts_ReturnsZeroPointThree()
    {
        Assert.Equal(0.3, GetProbability([3, 2, 1]), precision: 9);
    }

    // Mutable per-search scratch state Choose/Unchoose thread through - the
    // TState : class Backtrack.Search requires (ARCHITECTURE.md: mutations
    // must alias back to the caller, not copy).
    private sealed class SplitState
    {
        public int TypeIndex;
        public int Box1Total;
        public int Box1DistinctCount;
        public int Box2DistinctCount;
        public double Ways = 1.0;
    }

    private static double GetProbability(int[] balls)
    {
        var total = balls.Sum();
        var half = total / 2;
        var totalWays = BinomialCoefficient(total, half);
        var matchingWays = 0.0;
        var state = new SplitState();

        Backtrack.Search<SplitState, int>(
            state,
            isSolution: s => IsSolution(s, balls),
            // Search's onSolution never signals "stop" (see Backtrack.cs), so
            // TryEachCandidate still runs even at a leaf where IsSolution was
            // already true - Candidates must return empty there rather than
            // index balls out of bounds.
            candidates: s => Candidates(s, balls),
            choose: (s, toBox1) => Choose(s, toBox1, balls),
            unchoose: (s, toBox1) => Unchoose(s, toBox1, balls),
            onSolution: s => matchingWays += OnSolutionWeight(s, half));

        return matchingWays / totalWays;
    }

    private static bool IsSolution(SplitState s, int[] balls) => s.TypeIndex == balls.Length;

    private static IEnumerable<int> Candidates(SplitState s, int[] balls)
        => s.TypeIndex == balls.Length ? [] : Enumerable.Range(0, balls[s.TypeIndex] + 1);

    private static void Choose(SplitState s, int toBox1, int[] balls)
    {
        var typeCount = balls[s.TypeIndex];
        s.Ways *= BinomialCoefficient(typeCount, toBox1);
        s.Box1Total += toBox1;

        if (toBox1 > 0)
        {
            s.Box1DistinctCount++;
        }

        if (typeCount - toBox1 > 0)
        {
            s.Box2DistinctCount++;
        }

        s.TypeIndex++;
    }

    private static void Unchoose(SplitState s, int toBox1, int[] balls)
    {
        s.TypeIndex--;
        var typeCount = balls[s.TypeIndex];

        if (typeCount - toBox1 > 0)
        {
            s.Box2DistinctCount--;
        }

        if (toBox1 > 0)
        {
            s.Box1DistinctCount--;
        }

        s.Box1Total -= toBox1;
        s.Ways /= BinomialCoefficient(typeCount, toBox1);
    }

    private static double OnSolutionWeight(SplitState s, int half)
        => s.Box1Total == half && s.Box1DistinctCount == s.Box2DistinctCount ? s.Ways : 0.0;

    private static double BinomialCoefficient(int n, int r)
    {
        var result = 1.0;

        for (var i = 0; i < r; i++)
        {
            result = result * (n - i) / (i + 1);
        }

        return result;
    }
}
