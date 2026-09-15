namespace DSAExperimentation.LeetCode.UglyNumberIII;

// LC 1201's counting rule on its own: how many positive integers at or below x are
// divisible by a, by b, or by c is three-set inclusion-exclusion over the multiples of
// each factor,
//
//   x/a + x/b + x/c - x/lcm(a,b) - x/lcm(a,c) - x/lcm(b,c) + x/lcm(a,b,c)
//
// and it is non-decreasing in x, which is the one property UglyCountSequence's binary
// search leans on. The three factors are its whole input, and everything else it needs
// is derived from them here rather than asked of a caller: the four LCMs depend on no
// argument but the factors, so they are computed once into the single value they are,
// and the cap they are computed against is the end of the window the caller searches,
// so no caller can hand in an LCM the counting rule does not actually hold over.
//
// lcm(a, b, c) is computed with that cap so that combining two already-huge pairwise
// LCMs never overflows long - once an operand already exceeds the cap every further
// multiple of it does too, so its exact value stops mattering and it can be clamped
// past the end of the window instead of multiplied.
internal readonly struct MultiplesOfThreeFactors
{
    private readonly int a;
    private readonly int b;
    private readonly int c;
    private readonly SubsetCommonMultiples commonMultiples;

    public MultiplesOfThreeFactors(int a, int b, int c, long cap)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        var lcmAb = Lcm(a, b);
        commonMultiples = new SubsetCommonMultiples(lcmAb, Lcm(a, c), Lcm(b, c), LcmCapped(lcmAb, c, cap));
    }

    // How many positive integers at or below x are divisible by a, by b, or by c.
    public long CountUpTo(long x) =>
        (x / a) + (x / b) + (x / c)
        - (x / commonMultiples.LcmAb) - (x / commonMultiples.LcmAc)
        - (x / commonMultiples.LcmBc) + (x / commonMultiples.LcmAbc);

    // x is only ever multiplied once it is already within cap, so the product never
    // exceeds cap * y - safely inside long's range for this problem's 10^9-bounded
    // factors.
    private static long LcmCapped(long x, long y, long cap) => x > cap ? OnePastCap(cap) : Lcm(x, y);

    // The first value past the window, which is all a clamped LCM has to say: its exact
    // value stopped mattering the moment it exceeded the cap.
    private static long OnePastCap(long cap) => cap + 1;

    private static long Lcm(long x, long y) => x / Gcd(x, y) * y;

    private static long Gcd(long x, long y) => y == 0 ? x : Gcd(y, x % y);
}
