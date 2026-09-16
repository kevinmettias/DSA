namespace DSAExperimentation.LeetCode.UglyNumberIII;

// LC 1201's counting rule on its own: how many positive integers at or below a
// candidate are divisible by the first factor, by the second, or by the third is
// three-set inclusion-exclusion over the multiples of each factor,
//
//   x/a + x/b + x/c - x/lcm(a,b) - x/lcm(a,c) - x/lcm(b,c) + x/lcm(a,b,c)
//
// and it is non-decreasing in that candidate, which is the one property
// UglyCountSequence's binary search leans on. The three factors are its whole input, and
// everything else it needs is derived from them here rather than asked of a caller: the
// four LCMs depend on no argument but the factors, so they are computed once into the
// single value they are, and the cap they are computed against is the end of the window
// the caller searches, so no caller can hand in an LCM the counting rule does not
// actually hold over.
//
// lcm(a, b, c) is computed with that cap so that combining two already-huge pairwise
// LCMs never overflows long - once an operand already exceeds the cap every further
// multiple of it does too, so its exact value stops mattering and it can be clamped
// past the end of the window instead of multiplied.
internal readonly struct MultiplesOfThreeFactors
{
    private readonly int _firstFactor;
    private readonly int _secondFactor;
    private readonly int _thirdFactor;
    private readonly SubsetCommonMultiples _commonMultiples;

    public MultiplesOfThreeFactors(int firstFactor, int secondFactor, int thirdFactor, long cap)
    {
        _firstFactor = firstFactor;
        _secondFactor = secondFactor;
        _thirdFactor = thirdFactor;
        var lcmAb = Lcm(firstFactor, secondFactor);
        _commonMultiples = new SubsetCommonMultiples(
            lcmAb, Lcm(firstFactor, thirdFactor), Lcm(secondFactor, thirdFactor),
            LcmCapped(lcmAb, thirdFactor, cap));
    }

    // How many positive integers at or below upperLimit are divisible by the first
    // factor, by the second, or by the third.
    public long CountUpTo(long upperLimit) =>
        (upperLimit / _firstFactor) + (upperLimit / _secondFactor) + (upperLimit / _thirdFactor)
        - (upperLimit / _commonMultiples.LcmAb) - (upperLimit / _commonMultiples.LcmAc)
        - (upperLimit / _commonMultiples.LcmBc) + (upperLimit / _commonMultiples.LcmAbc);

    // first is only ever multiplied once it is already within cap, so the product never
    // exceeds cap * second - safely inside long's range for this problem's 10^9-bounded
    // factors.
    private static long LcmCapped(long first, long second, long cap) =>
        first > cap ? OnePastCap(cap) : Lcm(first, second);

    // The first value past the window, which is all a clamped LCM has to say: its exact
    // value stopped mattering the moment it exceeded the cap.
    private static long OnePastCap(long cap) => cap + 1;

    private static long Lcm(long first, long second) => first / Gcd(first, second) * second;

    private static long Gcd(long first, long second) => second == 0 ? first : Gcd(second, first % second);
}
