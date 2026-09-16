using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

// LeetCode 1467. Probability of a Two Boxes Having The Same Number of Distinct
// Balls: 2n balls of k colours are shuffled and dealt n to each of two boxes;
// report the probability that both boxes end up holding the same number of
// distinct colours.
//
// Both strategies walk the identical search tree - one step per colour, offering
// "send k of this colour's copies to box 1" for every legal k - carrying a
// running product of C(balls[i], k) as the number of deals that produce the split
// explored so far. A leaf contributes its weight when box 1 holds exactly half
// the balls AND both boxes hold equally many distinct colours; dividing the total
// by C(total, total/2), the number of equally likely half/half deals, is the
// probability asked for.
internal static class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsSolution
{
    // The balls are dealt into exactly two boxes.
    private const int BoxCount = 2;

    // The textbook answer, and the arm the Backtrack composition has to justify
    // itself against: the same choose-a-split-per-colour recursion written by
    // hand with no engine underneath, passing its running totals down as
    // arguments rather than mutating shared state (ARCHITECTURE.md 17.5).
    public static double GetProbabilityByHandRolledRecursion(int[] balls)
    {
        var total = balls.Sum();
        var half = total / BoxCount;
        var deal = (Balls: balls, Half: half);

        return Recurse(deal, 0, default, 1.0) / BinomialCoefficient(total, half);
    }

    // Bundles the recursion's per-branch running totals so Recurse stays at four
    // parameters, and puts the "what does dealing toBox1 of this colour do to
    // them" rule in one place - the same rule Choose/Unchoose apply and undo
    // below.
    private readonly record struct SplitCounts(int Box1Total, int Box1Distinct, int Box2Distinct)
    {
        public SplitCounts Deal(int colorCount, int toBox1) =>
            new(Box1Total + toBox1,
                Box1Distinct + (toBox1 > 0 ? 1 : 0),
                Box2Distinct + (HasBallsForBox2(colorCount, toBox1) ? 1 : 0));

        private static bool HasBallsForBox2(int colorCount, int toBox1) => colorCount - toBox1 > 0;
    }

    // This repo's Backtrack.Search primitive drives the identical walk
    // declaratively: one Candidates step per colour, with Choose/Unchoose
    // maintaining box 1's running ball count, both boxes' distinct-colour counts
    // and the combinatorial weight, and undoing all four exactly on backtrack.
    // Same search tree, same arithmetic per node - the comparison isolates
    // Backtrack's own call/delegate overhead rather than a difference in what is
    // computed.
    public static double GetProbabilityByBacktracking(int[] balls)
    {
        var total = balls.Sum();
        var half = total / BoxCount;
        var matchingWays = 0.0;
        var state = new SplitState();

        Backtrack.Search<SplitState, int>(
            state,
            isSolution: s => s.ColorIndex == balls.Length,
            // Search's onSolution never signals "stop" (see Backtrack.cs), so
            // TryEachCandidate still runs even at a leaf where IsSolution was
            // already true - Candidates must return empty there rather than
            // index balls out of bounds.
            candidates: s => s.ColorIndex == balls.Length
                ? NoCandidates()
                : Enumerable.Range(0, balls[s.ColorIndex] + 1),
            choose: (s, toBox1) => Choose(s, balls, toBox1),
            unchoose: (s, toBox1) => Unchoose(s, balls, toBox1),
            onSolution: s => matchingWays += SolutionWays(s, half));

        return matchingWays / BinomialCoefficient(total, half);
    }

    private static IEnumerable<int> NoCandidates() => [];

    // Mutable per-search scratch state Choose/Unchoose thread through - the
    // TState : class Backtrack.Search requires (ARCHITECTURE.md: mutations must
    // alias back to the caller, not copy).
    private sealed class SplitState
    {
        public int ColorIndex { get; set; }

        public int Box1Total { get; set; }

        public int Box1DistinctCount { get; set; }

        public int Box2DistinctCount { get; set; }

        public double Ways { get; set; } = 1.0;
    }

    private static void Choose(SplitState state, int[] balls, int toBox1)
    {
        var colorCount = balls[state.ColorIndex];
        state.Ways *= BinomialCoefficient(colorCount, toBox1);
        state.Box1Total += toBox1;

        if (toBox1 > 0)
        {
            state.Box1DistinctCount++;
        }

        if (colorCount - toBox1 > 0)
        {
            state.Box2DistinctCount++;
        }

        state.ColorIndex++;
    }

    private static void Unchoose(SplitState state, int[] balls, int toBox1)
    {
        state.ColorIndex--;
        var colorCount = balls[state.ColorIndex];

        if (colorCount - toBox1 > 0)
        {
            state.Box2DistinctCount--;
        }

        if (toBox1 > 0)
        {
            state.Box1DistinctCount--;
        }

        state.Box1Total -= toBox1;
        state.Ways /= BinomialCoefficient(colorCount, toBox1);
    }

    private static double SolutionWays(SplitState state, int half) =>
        state.Box1Total == half && state.Box1DistinctCount == state.Box2DistinctCount
            ? state.Ways
            : 0.0;

    // One node of the hand-rolled walk: offer "send toBox1 of this colour's copies
    // to box 1" for every legal toBox1, and score a completed deal by the leaf
    // test. `deal` bundles the two inputs that never change during the walk, so the
    // recursion stays at four parameters.
    private static double Recurse(
        (int[] Balls, int Half) deal,
        int colorIndex,
        SplitCounts counts,
        double ways)
    {
        if (colorIndex == deal.Balls.Length)
        {
            return counts.Box1Total == deal.Half && counts.Box1Distinct == counts.Box2Distinct
                ? ways
                : 0.0;
        }

        var colorCount = deal.Balls[colorIndex];
        var matching = 0.0;

        for (var toBox1 = 0; toBox1 <= colorCount; toBox1++)
        {
            var nextCounts = counts.Deal(colorCount, toBox1);
            matching += Recurse(
                deal,
                colorIndex + 1,
                nextCounts,
                ways * BinomialCoefficient(colorCount, toBox1));
        }

        return matching;
    }

    // C(n, r) accumulated in double rather than exactly: LC 1467 caps a colour at
    // 6 copies and the total at 48, and the answer it wants is a probability, so
    // the running product never needs more range than a double gives.
    private static double BinomialCoefficient(int totalCount, int chosenCount)
    {
        var result = 1.0;

        for (var i = 0; i < chosenCount; i++)
        {
            result = result * (totalCount - i) / (i + 1);
        }

        return result;
    }
}
