using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.UglyNumberIII;

// LC 1201's binary search runs over a virtual monotone sequence rather than a
// materialized array: index i's "value" is whether at least n ugly numbers are at
// or below i, counted by three-set inclusion-exclusion over the multiples of a, of
// b, of c, and of their pairwise and three-way LCMs. This witness answers that one
// question for that one problem, so it lives beside the solution rather than in
// DataStructures/Sequence with IRandomAccessSequence's other implementations - the
// same placement NthMagicalNumber's MagicalCountSequence gets.
//
// The LCMs are derived here rather than passed in: they are part of the counting
// rule, not of the caller's question. lcm(a, b, c) is computed with a cap so that
// combining two already-huge pairwise LCMs never overflows long - once an operand
// already exceeds upperBound every further multiple of it does too, so its exact
// value stops mattering and it can be clamped past the end of the window instead
// of multiplied.
internal readonly struct UglyCountSequence : IRandomAccessSequence<int>
{
    private readonly int target;
    private readonly int a;
    private readonly int b;
    private readonly int c;
    private readonly int upperBound;
    private readonly long lcmAb;
    private readonly long lcmAc;
    private readonly long lcmBc;
    private readonly long lcmAbc;

    public UglyCountSequence(int n, int a, int b, int c, int upperBound)
    {
        target = n;
        this.a = a;
        this.b = b;
        this.c = c;
        this.upperBound = upperBound;
        lcmAb = Lcm(a, b);
        lcmAc = Lcm(a, c);
        lcmBc = Lcm(b, c);
        lcmAbc = LcmCapped(lcmAb, c, upperBound);
    }

    public int Length => upperBound + 1;

    // Get(index) treats index itself as the candidate ugly number x, the same
    // "value doubles as index" shape MagicalCountSequence uses.
    public int Get(int index)
    {
        long x = index;
        var count = (x / a) + (x / b) + (x / c) - (x / lcmAb) - (x / lcmAc) - (x / lcmBc) + (x / lcmAbc);

        return count >= target ? 1 : 0;
    }

    private static long Gcd(long x, long y) => y == 0 ? x : Gcd(y, x % y);

    private static long Lcm(long x, long y) => x / Gcd(x, y) * y;

    // x is only ever multiplied once it is already within cap, so the product never
    // exceeds cap * y - safely inside long's range for this problem's 10^9-bounded
    // factors.
    private static long LcmCapped(long x, long y, long cap) => x > cap ? cap + 1 : Lcm(x, y);
}
