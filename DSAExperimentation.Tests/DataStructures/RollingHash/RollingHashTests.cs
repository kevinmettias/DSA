using RollingHashOperations = DSAExperimentation.DataStructures.RollingHash.RollingHash;
using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Tests.DataStructures.RollingHash;

public sealed partial class RollingHashTests
{
    // Reused from ManacherTests' own brute-force cross-check set, plus one longer
    // repeat-heavy string - repeat-heavy text is exactly where a weak rolling
    // hash would be most likely to expose a real collision.
    private static readonly string[] BruteForceCrossCheckStrings =
    [
        "aaaa", "abcba", "aabbaa", "racecar", "abacabad", "mississippi", "abababab", "x", "",
        "abababababababababab",
    ];

    // Every (start, length) window pair across a string, compared against real
    // substring equality. A failure here at production-scale constants is a
    // ~1e-9-probability event per RollingHash's own collision-probability
    // argument, not flakiness - see RollingHash.cs's doc comment.
    [Fact]
    public void Hash_AllSubstringPairsAcrossVariedRepeatHeavyStrings_AgreesWithReferenceEqualityBruteForce()
    {
        foreach (var text in BruteForceCrossCheckStrings)
        {
            var hash = new RollingHashOperations(text);

            foreach (var (start1, length1) in AllWindows(text))
            {
                foreach (var (start2, length2) in AllWindows(text))
                {
                    var window1 = text.AsSpan(start1, length1);
                    var window2 = text.AsSpan(start2, length2);
                    var expected = window1.SequenceEqual(window2);
                    var actual = hash.Hash(start1, length1) == hash.Hash(start2, length2);
                    Assert.Equal(expected, actual);
                }
            }
        }
    }

    private static IEnumerable<(int Start, int Length)> AllWindows(string text)
    {
        for (var start = 0; start <= text.Length; start++)
        {
            for (var length = 0; start + length <= text.Length; length++)
            {
                yield return (start, length);
            }
        }
    }

    // Hand-traceable custom lanes - RollingHashLane.DefaultFirst/DefaultSecond are
    // ~1e9 scale, not mentally verifiable, so this exercises the full-control
    // overload with small constants instead. "ab" under base 10, modulus 97:
    // prefix[1] = 'a'=97 -> normalize(97,97)=0 -> prefix[1]=0
    // prefix[2] = (0*10 + normalize('b'=98,97)=1) % 97 = 1
    // Hash(0,2) = prefix[2] - prefix[0]*10^2 = 1 - 0 = 1.
    [Fact]
    public void Hash_KnownStringWithSmallCustomLane_MatchesHandComputedValue()
    {
        var lane = new RollingHashLane(10, 97);
        var comparer = EqualityComparer<char>.Create((left, right) => left == right, value => value);
        var hash = new RollingHashOperations("ab", comparer, lane, lane);

        var result = hash.Hash(0, 2);

        Assert.Equal(1, result.First);
        Assert.Equal(1, result.Second);
    }

    [Fact]
    public void Hash_ZeroLengthQuery_ReturnsSameValueRegardlessOfStart()
    {
        var hash = new RollingHashOperations("abcdef");

        var atStart = hash.Hash(0, 0);
        var inMiddle = hash.Hash(3, 0);
        var atEnd = hash.Hash(6, 0);

        Assert.Equal(atStart, inMiddle);
        Assert.Equal(atStart, atEnd);
        Assert.Equal(default, atStart);
    }

    [Fact]
    public void Hash_EmptyText_ZeroLengthQuery_DoesNotThrow()
    {
        var hash = new RollingHashOperations("");

        var result = hash.Hash(0, 0);

        Assert.Equal(default, result);
    }

    [Fact]
    public void Hash_LengthExceedingRemainingText_ThrowsArgumentOutOfRangeException()
    {
        var hash = new RollingHashOperations("abc");

        Assert.Throws<ArgumentOutOfRangeException>(() => hash.Hash(1, 3));
    }

    [Fact]
    public void Hash_NegativeStart_ThrowsArgumentOutOfRangeException()
    {
        var hash = new RollingHashOperations("abc");

        Assert.Throws<ArgumentOutOfRangeException>(() => hash.Hash(-1, 1));
    }

    [Fact]
    public void Hash_NegativeLength_ThrowsArgumentOutOfRangeException()
    {
        var hash = new RollingHashOperations("abc");

        Assert.Throws<ArgumentOutOfRangeException>(() => hash.Hash(0, -1));
    }

    [Fact]
    public void Hash_StartAtEndWithZeroLength_ReturnsZeroValue()
    {
        var hash = new RollingHashOperations("abc");

        var result = hash.Hash(3, 0);

        Assert.Equal(default, result);
    }

    // Two different texts sharing a common substring, each hashed via a separate
    // RollingHash instance, agree on that shared substring - the concrete test
    // for the cross-instance comparison precondition Hash's own doc comment
    // documents (needed for e.g. longest-common-substring/duplicate-across-texts
    // use cases).
    [Fact]
    public void Hash_SameSubstringAcrossTwoDefaultConstructedInstances_AreEqual()
    {
        var first = new RollingHashOperations("xxhelloyy");
        var second = new RollingHashOperations("zzhellowworld");

        var firstHash = first.Hash(2, 5);
        var secondHash = second.Hash(2, 5);

        Assert.Equal(firstHash, secondHash);
    }

    [Fact]
    public void Hash_WithCaseInsensitiveComparer_TreatsDifferentCaseCharactersAsEqual()
    {
        var caseInsensitive = EqualityComparer<char>.Create(
            (left, right) => char.ToUpperInvariant(left) == char.ToUpperInvariant(right),
            value => char.ToUpperInvariant(value).GetHashCode());

        var lower = new RollingHashOperations("hello", caseInsensitive);
        var upper = new RollingHashOperations("HELLO", caseInsensitive);
        var lowerHash = lower.Hash(0, 5);
        var upperHash = upper.Hash(0, 5);

        Assert.Equal(lowerHash, upperHash);
    }

    // Proves the double-hash MECHANISM itself, not a real risk in the production
    // defaults: two short strings are hand-picked to collide under one
    // deliberately tiny single-lane modulus, then the combined value is shown to
    // still differ because the second (different, also tiny) lane does not also
    // collide. No such case is practically constructible against
    // RollingHashLane.DefaultFirst/DefaultSecond - that is the whole point of the
    // production constants (see RollingHash.cs's collision-probability argument).
    [Fact]
    public void Hash_WithDeliberatelySmallSingleLaneModulus_SingleLaneCollidesButCombinedValueDoesNot()
    {
        var comparer = EqualityComparer<char>.Create((left, right) => left == right, value => value);
        var collidingLane = new RollingHashLane(1, 5);
        var safeLane = new RollingHashLane(1, 97);
        var first = new RollingHashOperations("a", comparer, collidingLane, safeLane);
        var second = new RollingHashOperations("f", comparer, collidingLane, safeLane);

        var firstHash = first.Hash(0, 1);
        var secondHash = second.Hash(0, 1);

        Assert.Equal(firstHash.First, secondHash.First);
        Assert.NotEqual(firstHash.Second, secondHash.Second);
        Assert.NotEqual(firstHash, secondHash);
    }
}
