using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// IMaxXorSet's contract, written once and run over every set that implements it: the one trie's
// BitTrie, through its view BitTrieSet (BitTrieSetTests), and TwoFieldBitTrie, the hand-specialized
// strategy for exactly this layer (TwoFieldBitTrieTests, ARCHITECTURE §13.9). A class that derives
// from this one names only its set, as a struct type argument, and inherits every fact. The maximum
// XOR is held to a scan of every value inserted, compared as unsigned bit patterns, over chosen
// values and over a seeded script across the whole int range, negative values included; membership
// and high-bit prefixes are TrieContractTests', run over both sets in TrieTests.
public abstract partial class MaxXorSetContractTests<TSet>
    where TSet : struct, IMaxXorSet<TSet>
{
    // The canonical LeetCode 421 example: nums = [3,10,5,25,2,8], the maximum XOR pair is 5^25 =
    // 28.
    private static readonly int[] ClassicExampleValues = [3, 10, 5, 25, 2, 8];

    private const int ScriptLength = 300;
    private const int ScriptSeed = 421;
    private const int QueryValue = 5;
    private const int InsertedValue = 3;
    private const int ExpectedMaxXor = 28;
    private const int NearNeighborValue = 0b1110;
    private const int FullyOppositeQuery = 0b1111;
    private const int DuplicateValue = 7;
    private const int SelfXorValue = 9;
    private const int BitsInAnInt = 32;

    public static TheoryData<int[]> ValueSets => new()
    {
        { [0] },
        { [int.MinValue, int.MaxValue] },
        { [-1, 1, -7, 12, 0] },
        { [1 << 20, (1 << 20) + 1, 3] },
    };

    // A prefix takes 0 to 32 of an int's bits; one bit fewer or more is no prefix length.
    public static TheoryData<int> BitsOutsideAnInt => [-1, BitsInAnInt + 1];

    [Fact]
    public void Empty_HoldsNoValue()
    {
        var set = TSet.Empty();

        Assert.Equal((0, false, false), (set.Count, set.HasKey(0), set.HasPrefix(0, 0)));
    }

    [Fact]
    public void TryMaxXor_EmptySet_ReturnsFalse()
    {
        var set = TSet.Empty();

        var found = set.TryMaxXor(QueryValue, out var result);

        Assert.Equal((false, 0), (found, result));
    }

    [Fact]
    public void TryMaxXor_SingleInsertedValue_ReturnsXorAgainstIt()
    {
        var set = SetOf([InsertedValue]);

        var found = set.TryMaxXor(QueryValue, out var result);

        Assert.Equal((true, InsertedValue ^ QueryValue), (found, result));
    }

    [Fact]
    public void TryMaxXor_ClassicExample_FindsMaximumAcrossAllInsertedValues()
    {
        var set = SetOf(ClassicExampleValues);

        var best = ClassicExampleValues.Max(query => MaxXorOf(set, query));

        Assert.Equal(ExpectedMaxXor, best);
    }

    // 0b0000 and 0b1111 differ in every one of these 4 bits, so their XOR, 15, beats pairing with
    // 0b1110, which is nearer to 0b1111 in value.
    [Fact]
    public void TryMaxXor_PrefersOppositeBitAtEveryLevel_OverNearestValue()
    {
        var set = SetOf([0b0000, NearNeighborValue]);

        Assert.Equal(FullyOppositeQuery, MaxXorOf(set, FullyOppositeQuery));
    }

    [Fact]
    public void TryMaxXor_ValueXorWithItself_CanReturnZero()
    {
        var set = SetOf([SelfXorValue]);

        Assert.Equal(0, MaxXorOf(set, SelfXorValue));
    }

    // Each query's answer is the largest unsigned XOR with any held value, by a scan of them all,
    // negative values included, whose XOR with a non-negative one reads back negative.
    [Theory]
    [MemberData(nameof(ValueSets))]
    public void TryMaxXor_AnyValues_IsTheLargestUnsignedXorOfAScan(int[] values)
    {
        var set = SetOf(values);

        Assert.All(values.Append(QueryValue), query => Assert.Equal(ScannedMaxXor(values, query), MaxXorOf(set, query)));
    }

    // A seeded script of inserts over the whole int range, each followed by a query drawn afresh.
    [Fact]
    public void TryMaxXor_SeededScript_AnswersAsAScanOfTheInsertedValues()
    {
        var set = TSet.Empty();
        var inserted = new List<int>();
        var random = new Random(ScriptSeed);

        for (var step = 0; step < ScriptLength; step++)
        {
            var value = random.Next(int.MinValue, int.MaxValue);
            set.Insert(value);
            inserted.Add(value);

            var query = random.Next(int.MinValue, int.MaxValue);
            Assert.Equal(ScannedMaxXor(inserted, query), MaxXorOf(set, query));
        }
    }

    [Fact]
    public void Insert_DuplicateValue_KeepsTheCount()
    {
        var set = SetOf([DuplicateValue, DuplicateValue]);

        Assert.Equal((1, true), (set.Count, set.HasKey(DuplicateValue)));
    }

    // No mark is kept at a value's end, so presence is the whole 32-level path: 1 shares all but
    // the last of 0's bits, and is no key until it is inserted.
    [Fact]
    public void HasKey_ValueSharingAllButTheLastBit_IsNoKey()
    {
        var set = SetOf([0]);

        Assert.Equal((true, false), (set.HasKey(0), set.HasKey(1)));
    }

    // 1 shares 0's first 31 bits, so it is a prefix of a held value at 31 bits and not at 32.
    [Fact]
    public void HasPrefix_HighBitsOfAHeldValue_IsTrueUpToWhereTheyDiffer()
    {
        var set = SetOf([0]);

        Assert.Equal((true, false), (set.HasPrefix(1, BitsInAnInt - 1), set.HasPrefix(1, BitsInAnInt)));
    }

    [Theory]
    [MemberData(nameof(BitsOutsideAnInt))]
    public void HasPrefix_BitsOutsideAnInt_Throws(int bits) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SetOf([0]).HasPrefix(0, bits));

    private static TSet SetOf(int[] values)
    {
        var set = TSet.Empty();

        foreach (var value in values)
        {
            set.Insert(value);
        }

        return set;
    }

    private static int MaxXorOf(TSet set, int query)
    {
        set.TryMaxXor(query, out var result);
        return result;
    }

    private static int ScannedMaxXor(IEnumerable<int> values, int query) =>
        unchecked((int)values.Max(value => unchecked((uint)(value ^ query))));
}
