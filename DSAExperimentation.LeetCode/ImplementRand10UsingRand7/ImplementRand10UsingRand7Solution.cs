namespace DSAExperimentation.LeetCode.ImplementRand10UsingRand7;

// LeetCode 470. Implement Rand10() Using Rand7(): given a black-box Rand7() that
// returns a uniform 1..7 integer, build Rand10() returning a uniform 1..10 integer,
// calling Rand7() only. Rand7 is injected as a delegate rather than owned
// internally (LinkedListRandomNodeSolution's convention) so a harness controls
// seeding and call count instead of the strategy hiding its own randomness.
//
// Rand10ByNaiveModuloFold is the common first instinct - a single Rand7() call
// folded via modulo - included as the "fast but wrong" contrast: 7 does not divide
// 10 evenly, so its output is statistically biased (it can never produce 8, 9 or
// 10), not a correctness baseline the other strategy has to beat. There is only one
// LeetCode-accepted algorithm for this problem, and it is
// Rand10ByRejectionSampling: two Rand7() calls address a uniform 1..49 grid,
// retrying on the 9 cells beyond 40, and the surviving 1..40 folds down to a
// uniform 1..10.
internal static class ImplementRand10UsingRand7Solution
{
    private const int Rand7RangeSize = 7;
    private const int RejectionThreshold = 40;
    private const int Rand10Range = 10;

    public static int Rand10ByNaiveModuloFold(Func<int> rand7) => 1 + (rand7() - 1) % Rand10Range;

    public static int Rand10ByRejectionSampling(Func<int> rand7)
    {
        int index;

        do
        {
            var row = rand7();
            var col = rand7();
            index = ((row - 1) * Rand7RangeSize) + col;
        } while (index > RejectionThreshold);

        return 1 + (index - 1) % Rand10Range;
    }
}
