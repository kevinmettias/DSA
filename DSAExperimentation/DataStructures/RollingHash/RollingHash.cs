namespace DSAExperimentation.DataStructures.RollingHash;

// Precomputes a polynomial rolling hash over a fixed text in O(text.Length), then
// answers Hash(start, length) - "what does the substring text[start..start+length)
// hash to" - in O(1). This is an equality SCREEN, not an equality ORACLE: two
// substrings that are actually equal always produce equal RollingHashValues, but
// two different substrings can, with vanishingly small probability, also produce
// equal RollingHashValues (a hash collision). See RollingHashValue's own doc
// comment for what that means for a caller.
//
// Unlike PrefixFunctionSearch/Manacher (pure static methods - each call finishes,
// nothing is left to query again), this is a stateful Representation per
// ARCHITECTURE.md §5 step 3/4: a caller constructs one RollingHash and queries
// Hash repeatedly at O(1) each, the same "precompute into parallel arrays, expose
// checked query methods" shape DisjointSetForest/HeapArray already use - which is
// also why this is an `internal sealed class`, not a `readonly struct`, matching
// this repo's own class-vs-struct convention (struct is reserved for small
// value-bundles like RollingHashLane/RollingHashValue, not multi-array state).
// It does not need to be a `ref struct` even though it is built from a
// ReadOnlySpan<char>: the span is read only inside the constructor to populate
// the long[] fields below, never stored, so instances stay ordinary
// heap-allocatable references.
//
// Collision safety - double hashing, not a single modulus, not `unchecked` 64-bit
// wraparound:
//   - Wraparound (natural ulong overflow, modulus implicitly 2^64) is rejected
//     because 2^64 is not prime - the polynomial ring over it has zero divisors,
//     which is exactly what published "anti-hash" constructions exploit to build
//     a universal collision independent of the chosen base. Fast, but that is a
//     performance property, not a collision-resistance one.
//   - A single ~1e9 modulus is rejected because the birthday bound makes the risk
//     concrete: the char domain has 65536 values, and 65536 > sqrt(1e9) ~ 31623,
//     so a single ~1e9 modulus has a non-trivial chance that some pair of the
//     65536 possible char values collides under that one modulus alone.
//   - Double hashing (two lanes, independent Base AND independent Modulus each,
//     combined into one RollingHashValue) needs both lanes to collide
//     simultaneously for a false positive to surface. Per-pair collision
//     probability per lane is ~1/modulus; joint probability across two
//     independent ~1e9 lanes is ~1/(1e9 * 1e9) = ~1e-18 - over the full 65536^2
//     character-value pairs, the expected number of both-lanes-collide pairs is
//     ~2e-9, effectively zero. This same argument covers substring-level
//     collisions at any text length this repo would realistically exercise.
//   - Both moduli are ~1e9, so `previous * base` is bounded by ~1e9 * 1e9 = 1e18,
//     safely inside long.MaxValue (~9.22e18) using ordinary checked `long`
//     arithmetic - no `unchecked` block anywhere in this type.
//   - RollingHashLane.DefaultFirst/DefaultSecond are fixed, deterministic
//     constants, not randomized per run - see RollingHashLane's own doc comment
//     for why. The full-control constructor below is the escape hatch for a
//     caller who wants custom or randomized lanes instead.
//
// The character-encoding step uses `comparer.GetHashCode(char)`, not `(long)char`,
// so a caller-supplied case-insensitive comparer (e.g. the
// `value => char.ToUpperInvariant(value).GetHashCode()` comparer already used in
// PrefixFunctionSearchTests/ManacherTests) makes case-different characters hash
// identically. This asks slightly more of the comparer than IEqualityComparer<T>'s
// own contract guarantees (the BCL only requires Equals(x,y) => GetHashCode(x) ==
// GetHashCode(y), one direction; this type also needs the converse - two
// "different" characters must not collide under GetHashCode, or they get silently
// conflated) - the same shape as IPathHeuristic's own strengthened "consistent, not
// merely admissible" contract (ARCHITECTURE.md §7). A per-instance remap table
// (assigning dense codes in first-occurrence order) would actively break the
// cross-instance comparison use case documented on Hash below, since the same
// character would map to different codes across two independently-constructed
// instances - so the encoding stays a pure function of the character alone, and
// double hashing (not a remap table) is what keeps that safe.
internal sealed class RollingHash
{
    private const string StartOutOfRangeMessage = "Start must be within [0, Length].";
    private const string LengthOutOfRangeMessage = "Length must be non-negative and must not extend past the end of the hashed text.";

    private readonly LaneState _first;
    private readonly LaneState _second;

    public int Length { get; }

    public RollingHash(ReadOnlySpan<char> text)
        : this(text, EqualityComparer<char>.Default)
    {
    }

    public RollingHash(ReadOnlySpan<char> text, IEqualityComparer<char> comparer)
        : this(text, comparer, RollingHashLane.DefaultFirst, RollingHashLane.DefaultSecond)
    {
    }

    // The full-control overload: custom or randomized lanes (see this type's own
    // doc comment), and the only practical way to write a hand-verified
    // small-number test - RollingHashLane.DefaultFirst/DefaultSecond are ~1e9
    // scale, not mentally traceable the way Manacher's "babad" example is.
    public RollingHash(ReadOnlySpan<char> text, IEqualityComparer<char> comparer, RollingHashLane first, RollingHashLane second)
    {
        Length = text.Length;
        _first = BuildLane(text, comparer, first);
        _second = BuildLane(text, comparer, second);
    }

    // O(1): two lane lookups, each two array reads/one multiply/one subtract/one
    // mod - no loop, which is the entire point and the reason this is a
    // Representation, not a recomputation (mirrors ARCHITECTURE.md §8's
    // HeapArray/Heap cross-reference: this O(1) claim holds only because
    // _first/_second are O(1)-indexed arrays).
    //
    // Throws for start < 0, start > Length, length < 0, or start + length >
    // Length - mirrors ReadOnlySpan<T>.Slice(start, length)'s own bounds-checking
    // contract deliberately, since Hash is conceptually an O(1) substitute for
    // "Slice(start, length), then hash it yourself." A zero-length query at any
    // valid start (0..Length inclusive) is not an error and needs no special
    // guard: the combination formula already self-cancels to (0, 0) when
    // length == 0.
    //
    // Comparing RollingHashValue results across two DIFFERENT RollingHash
    // instances (needed for cross-text use cases like longest-common-substring
    // or duplicate-substring-across-texts detection) is only meaningful when
    // both instances were built with the same comparer and the same two lanes -
    // an unenforced precondition, the same shape as BinarySearch's sortedness
    // law (ARCHITECTURE.md §9.1). The default and comparer-only constructor
    // overloads always agree (both use RollingHashLane.DefaultFirst/
    // DefaultSecond), so this holds automatically unless a caller deliberately
    // passes different custom lanes to each instance.
    public RollingHashValue Hash(int start, int length)
    {
        EnsureValidQuery(start, length);

        var first = Combine(_first, start, length);
        var second = Combine(_second, start, length);
        return new RollingHashValue(first, second);
    }

    private void EnsureValidQuery(int start, int length)
    {
        if (start < 0 || start > Length)
        {
            ThrowStartOutOfRange(start);
        }

        if (length < 0 || start + length > Length)
        {
            ThrowLengthOutOfRange(length);
        }
    }

    private static void ThrowStartOutOfRange(int start)
        => throw new ArgumentOutOfRangeException(nameof(start), StartOutOfRangeMessage);

    private static void ThrowLengthOutOfRange(int length)
        => throw new ArgumentOutOfRangeException(nameof(length), LengthOutOfRangeMessage);

    private static long Combine(LaneState lane, int start, int length)
    {
        var end = lane.PrefixHashes[start + length];
        var scaledStart = lane.PrefixHashes[start] * lane.Powers[length] % lane.Modulus;
        return Normalize(end - scaledStart, lane.Modulus);
    }

    private static LaneState BuildLane(ReadOnlySpan<char> text, IEqualityComparer<char> comparer, RollingHashLane lane)
    {
        var prefixHashes = new long[text.Length + 1];
        var powers = new long[text.Length + 1];
        powers[0] = 1;

        for (var i = 0; i < text.Length; i++)
        {
            var code = comparer.GetHashCode(text[i]);
            prefixHashes[i + 1] = (prefixHashes[i] * lane.Base + Normalize(code, lane.Modulus)) % lane.Modulus;
            powers[i + 1] = powers[i] * lane.Base % lane.Modulus;
        }

        return new LaneState(prefixHashes, powers, lane.Modulus);
    }

    private static long Normalize(long value, long modulus) => ((value % modulus) + modulus) % modulus;

    // Bundles one lane's derived arrays with its modulus - the same "group state
    // Combine/BuildLane need beyond their own parameters" role
    // PrefixFunctionSearch's FailureFunctionMatcher plays, and un-fileworthy on
    // its own for the same reason: it never crosses RollingHash's own public
    // boundary.
    private readonly record struct LaneState(long[] PrefixHashes, long[] Powers, long Modulus);
}
