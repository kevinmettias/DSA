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
internal readonly record struct RollingHashLane(long Base, long Modulus)
{
    public static RollingHashLane DefaultFirst => new(131, 1_000_000_007);

    public static RollingHashLane DefaultSecond => new(137, 998_244_353);
}
