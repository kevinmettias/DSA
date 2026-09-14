namespace DSAExperimentation.LeetCode.LongestBinarySubsequenceLessThanOrEqualToK;

// LeetCode 2311. Longest Binary Subsequence Less Than or Equal to K: the longest
// subsequence of a binary string whose value, read as a binary number, is at most k.
//
// No repo primitive applies - the answer is a single right-to-left greedy scan over
// the string itself, the same "no stronger reusable primitive over a bare sequence"
// category already established for GasStation/JumpGame/MaximumProductSubarray. The
// two strategies here are that scan and the exhaustive subset enumeration it has to
// justify itself against.
internal static class LongestBinarySubsequenceLessThanOrEqualToKSolution
{
    // 2^30 already exceeds 1e9, the largest value LeetCode's k can take, so no '1'
    // at or beyond this position can ever be affordable. Skipping those outright is not just an
    // optimization: C# masks a long's shift count to 6 bits, so an unguarded
    // `1L << power` past 63 would silently wrap to a small, wrong weight rather than
    // the huge one intended - the same "state the overflow law, don't assume it"
    // discipline IntegerReplacement's int-vs-long choice already documents.
    private const int MaxAffordablePower = 30;

    // The textbook answer: every one of the 2^n subsequences, each read as a binary
    // number and checked against the bound. Deliberately written with nothing from
    // this repo - it is the arm the greedy scan below has to justify itself against.
    //
    // Unchecked precondition: bits is short enough for its subset space to be walked
    // at all (the mask is an int, so fewer than 31 characters). That is a property of
    // the baseline's exhaustive character, not of the problem - the greedy strategy
    // below carries no such bound.
    public static int LongestSubsequenceBySubsetEnumeration(string bits, int maxValue)
    {
        var best = 0;

        // The empty subsequence is worth 0 characters, which best already holds, so
        // enumeration can start at the first non-empty mask.
        for (var mask = 1; mask < 1 << bits.Length; mask++)
        {
            var (value, length) = ValueAndLength(bits, mask);

            if (value <= maxValue && length > best)
            {
                best = length;
            }
        }

        return best;
    }

    // Reads the characters the mask selects, most-significant first, as one binary
    // number. The value is a long because a 30-character selection already outruns
    // an int, and the comparison against the bound has to see the real magnitude
    // rather than a wrapped one.
    private static (long Value, int Length) ValueAndLength(string bits, int mask)
    {
        long value = 0;
        var length = 0;

        for (var i = 0; i < bits.Length; i++)
        {
            if ((mask & (1 << i)) == 0)
            {
                continue;
            }

            value = (value << 1) + (bits[i] - '0');
            length++;
        }

        return (value, length);
    }

    // The O(n) answer, scanning right to left so each character's weight is known by
    // the time it is decided on. The decision itself lives in GreedyTake below, so
    // this is just the walk.
    public static int LongestSubsequenceByGreedyScan(string bits, int maxValue)
    {
        var taken = new GreedyTake(maxValue);

        for (var i = bits.Length - 1; i >= 0; i--)
        {
            taken.Consider(bits[i]);
        }

        return taken.Length;
    }

    // The greedy scan's running state - how many characters have been taken, what
    // they are worth read as a binary number, and how far left the next character's
    // weight has moved. It exists so the scan above stays a single loop rather than
    // four locals threaded through a branch by hand, the same job AddBinarySolution's
    // DigitWalk does for the carry walk; a class rather than a walk struct because
    // every one of these fields is assigned after construction.
    private sealed class GreedyTake(int budget)
    {
        private readonly int _budget = budget;
        private long _value;
        private int _power;

        public int Length { get; private set; }

        // A '0' is always worth taking - it adds a character and nothing to the
        // value, wherever it lands. A '1' costs 2^power, which doubles with every
        // character already taken to its right, so it is taken only while the running
        // value still fits the budget; scanning right to left is what makes that
        // cheapest-first and therefore optimal.
        public void Consider(char bit)
        {
            if (bit == '0')
            {
                Take(0);
                return;
            }

            if (_power >= MaxAffordablePower)
            {
                return;
            }

            var weight = 1L << _power;

            if (_value + weight <= _budget)
            {
                Take(weight);
            }
        }

        private void Take(long weight)
        {
            _value += weight;
            _power++;
            Length++;
        }
    }
}
