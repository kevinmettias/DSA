namespace DSAExperimentation.LeetCode.PartitionArrayForMaximumXorAndAnd;

// A linear basis (Gaussian elimination over GF(2)) of a multiset of integers: the
// classic "maximum XOR subset" structure, specialized here with a masked variant
// that PartitionArrayForMaximumXorAndAndSolution's subset-basis strategy needs and
// nothing else does, which is why it lives beside the solution rather than as a
// DataStructures/ primitive.
//
// nums[i] <= 1e9 < 2^30, so 30 pivot slots cover every bit this problem can set.
//
// A reference type, not a value type: Insert mutates the pivot array in place, so a
// copy of a value-typed basis would be a shallow one sharing that same array - two
// bases that look independent and are secretly the same. A mutable basis has an
// identity, and this declaration is where that is said.
internal sealed class XorBasis
{
    private const int BitWidth = 30;

    private readonly long[] _pivots = new long[BitWidth];

    // Standard reduction: walk value's bits high to low, either claiming an empty
    // pivot slot or folding value through the pivot already there. A value that
    // reduces all the way to zero was already in the span - nothing to add.
    public void Insert(long value)
    {
        for (var bit = BitWidth - 1; bit >= 0; bit--)
        {
            if (((value >> bit) & 1) == 0)
            {
                continue;
            }

            if (_pivots[bit] == 0)
            {
                _pivots[bit] = value;
                return;
            }

            value ^= _pivots[bit];
        }
    }

    // The maximum value of (x & mask) over every x reachable as the XOR of some
    // subset of the inserted vectors. Masking is GF(2)-linear (it just zeroes
    // coordinates), so the reachable set of masked values is exactly the span of
    // {pivot & mask} - re-inserting the masked pivots rebuilds a proper basis for
    // that span, and the textbook greedy (take a pivot whenever it grows the
    // running result, high bit first) then finds its maximum.
    public long MaxMaskedXor(long mask)
    {
        var masked = MaskedBasis(mask);

        return HighestGreedy(masked);
    }

    // A proper basis for the span of {pivot & mask}, rebuilt from the masked pivots.
    private XorBasis MaskedBasis(long mask)
    {
        var masked = new XorBasis();

        foreach (var pivot in _pivots)
        {
            if (pivot != 0)
            {
                masked.Insert(pivot & mask);
            }
        }

        return masked;
    }

    // The textbook greedy over a basis: walk the pivots high bit first and take each
    // one that grows the running result.
    private static long HighestGreedy(XorBasis basis)
    {
        var best = 0L;

        for (var bit = BitWidth - 1; bit >= 0; bit--)
        {
            var candidate = best ^ basis._pivots[bit];

            if (basis._pivots[bit] != 0 && candidate > best)
            {
                best = candidate;
            }
        }

        return best;
    }
}
