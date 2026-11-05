namespace DSAExperimentation.LeetCode.Solutions.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

// LC 1467's promise about its input: "sum(balls) is even", so the balls split into two boxes of n each.
// LeetCode's judge never poses an input that breaks it. With an odd total no split fills both boxes
// equally, and the strategies' counts no longer stand for the statement's probability. Generated cases
// do not always keep the promise, so a harness that runs them asks this first.
internal static class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsPromise
{
    // Two boxes of equal size.
    private const int Boxes = 2;

    public static bool IsKeptBy(int[] balls) => balls.Sum() % Boxes == 0;
}
