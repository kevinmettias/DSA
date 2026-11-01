namespace DSAExperimentation.DataStructures.RollingHash;

// One (Base, Modulus) tuning pair for a single hash lane of RollingHash. Grouping
// these - rather than four bare `long` parameters on RollingHash's full-control
// constructor - keeps that constructor within this repo's parameter-count limit
// and rules out a caller positionally transposing one lane's base with the
// other lane's modulus, a real risk with four same-typed `long`s in a row.
//
// DefaultFirst/DefaultSecond are large, independent primes (131/1_000_000_007
// and 137/998_244_353 - both well-vetted competitive-programming constants, the
// second NTT-friendly and independently sourced from the first) chosen so
// RollingHash's two lanes need to collide simultaneously for a false positive:
// see RollingHash.cs's own doc comment for the full collision-probability
// argument. Fixed and deterministic rather than randomized-per-run, matching
// every other algorithm in this repo (nothing here uses System.Random) and this
// repo's own hand-verified-example test convention, which a randomized base
// would break.
/// <summary>One base and modulus pair that tunes a single hash lane of a <see cref="RollingHash"/>.</summary>
/// <param name="Base">The polynomial's base, multiplying the running hash before each character is added.</param>
/// <param name="Modulus">The modulus every hash in the lane is reduced by.</param>
public readonly record struct RollingHashLane(long Base, long Modulus)
{
    /// <summary>The default first lane, base 131 and modulus 1,000,000,007.</summary>
    public static RollingHashLane DefaultFirst => new(131, 1_000_000_007);

    /// <summary>The default second lane, base 137 and modulus 998,244,353.</summary>
    public static RollingHashLane DefaultSecond => new(137, 998_244_353);
}
