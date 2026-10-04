namespace DSAExperimentation.LeetCode.ImplementRand10UsingRand7;

// LeetCode 470. Implement Rand10() Using Rand7(): given a black-box Rand7() that
// returns a uniform 1..7 integer, build Rand10() returning a uniform 1..10 integer,
// calling Rand7() only. Rand7 is injected as a collaborator rather than owned
// internally (LinkedListRandomNodeSolution's convention) so a harness controls
// seeding and call count instead of the strategy hiding its own randomness.
//
// Both strategies are rejection sampling, because 7^k is never a multiple of 10: no
// fixed number of Rand7() calls splits evenly ten ways, so a uniform Rand10() must
// sometimes draw again. Rand10ByRejectionSampling is the textbook answer: two
// Rand7() calls address a uniform 1..49 grid, retrying from scratch on the 9 cells
// beyond 40, and the surviving 1..40 folds down to a uniform 1..10.
// Rand10ByRecycledRejectionSampling answers LeetCode's follow-up - "could you
// minimize the number of calls to rand7()?" - by keeping what a rejected draw
// still knows instead of throwing it away.
internal static class ImplementRand10UsingRand7Solution
{
    private const int Rand7RangeSize = 7;
    private const int RejectionThreshold = 40;
    private const int Rand10Range = 10;

    public static int Rand10ByRejectionSampling(IRand7 rand7)
    {
        int index;

        do
        {
            var row = rand7.Draw();
            var col = rand7.Draw();
            index = ((row - 1) * Rand7RangeSize) + col;
        } while (index > RejectionThreshold);

        return 1 + (index - 1) % Rand10Range;
    }

    // Each round extends the value carried out of the last one by one more Rand7()
    // call: a value uniform over 1..r and a draw make a value uniform over 1..7r, of
    // which the largest multiple of ten folds to a uniform 1..10. A rejected value is
    // still uniform over the cells past that multiple, so it is carried into the next
    // round rather than discarded. From nothing carried (r = 1) the rounds are 7, 49,
    // 63 and 21 cells wide and accept 0, 40, 60 and 20 of them; the 21st cell carries
    // r = 1 again, a fresh start, once in 7^4 = 2401 four-draw sequences. That
    // averages 2.19 Rand7() calls per Rand10() against the plain sampler's
    // 2 * 49/40 = 2.45.
    public static int Rand10ByRecycledRejectionSampling(IRand7 rand7)
    {
        var carried = 1;
        var carriedRange = 1;
        int index;
        int accepted;

        do
        {
            var range = carriedRange * Rand7RangeSize;
            index = ((carried - 1) * Rand7RangeSize) + rand7.Draw();
            accepted = range - (range % Rand10Range);
            carried = index - accepted;
            carriedRange = range - accepted;
        } while (index > accepted);

        return 1 + (index - 1) % Rand10Range;
    }
}
