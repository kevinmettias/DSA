using System.Numerics;
using DSAExperimentation.Domain.Games.PositionalGames;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

// The four words act as one 256-bit number: every operation is held to the same operation on a
// BigInteger of the same bits, on masks drawn at random and on cells at each word's edges, and
// each shift by every count from 0 to past the end.
public sealed partial class Bitboard256Tests
{
    private const int RandomMasks = 20;
    private const int Seed = 256;
    private const int Bytes = (Bitboard256.Capacity / 8) + 1;
    private static readonly BigInteger AllCells = (BigInteger.One << Bitboard256.Capacity) - 1;

    // The first and last cell of each word, and the cells either side of a word's edge.
    public static TheoryData<int> EdgeCells =>
        new() { 0, 1, 62, 63, 64, 65, 127, 128, 191, 192, 254, 255 };

    public static TheoryData<int> CellsOffTheMask => new() { -1, Bitboard256.Capacity };

    [Fact]
    public void Empty_HoldsNoCell()
    {
        var empty = Bitboard256.Empty;

        Assert.Equal(
            (true, 0, Bitboard256.Capacity), (empty.IsEmpty, empty.Count, empty.LowestCell));
    }

    [Theory]
    [MemberData(nameof(EdgeCells))]
    public void Cell_HoldsThatCellAlone(int cell)
    {
        var mask = Bitboard256.Cell(cell);

        Assert.Equal(
            (1, cell, true, BigInteger.One << cell),
            (mask.Count, mask.LowestCell, mask.HasCell(cell), Number(mask)));
    }

    [Theory]
    [MemberData(nameof(CellsOffTheMask))]
    public void Cell_OffTheMask_IsRefused(int cell) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Bitboard256.Cell(cell));

    [Fact]
    public void Of_ListedCells_HoldsEachOnce() =>
        Assert.Equal(
            (BigInteger.One << 200) | (BigInteger.One << 64) | 1,
            Number(Bitboard256.Of([0, 64, 200, 64])));

    [Fact]
    public void Operators_RandomMasks_AreTheNumbersBitwiseOperators() =>
        Assert.All(Pairs(), pair =>
        {
            var (left, right) = (Mask(pair.Left), Mask(pair.Right));

            Assert.Equal(
                [pair.Left & pair.Right, pair.Left | pair.Right, pair.Left ^ pair.Right,
                    AllCells ^ pair.Left],
                [Number(left & right), Number(left | right), Number(left ^ right), Number(~left)]);
        });

    [Fact]
    public void ShiftUp_EveryCount_IsTheNumberShiftedLeftWithinTheMask() =>
        Assert.All(Numbers(), number => Assert.All(
            Enumerable.Range(0, Bitboard256.Capacity + 2),
            count => Assert.Equal(
                (number << count) & AllCells, Number(Mask(number).ShiftUp(count)))));

    [Fact]
    public void ShiftDown_EveryCount_IsTheNumberShiftedRight() =>
        Assert.All(Numbers(), number => Assert.All(
            Enumerable.Range(0, Bitboard256.Capacity + 2),
            count => Assert.Equal(number >> count, Number(Mask(number).ShiftDown(count)))));

    [Fact]
    public void Count_RandomMasks_IsTheNumberOfSetBits() =>
        Assert.All(Numbers(), number => Assert.Equal(Cells(number).Length, Mask(number).Count));

    [Theory]
    [MemberData(nameof(EdgeCells))]
    public void IsEmpty_AMaskOfOneCell_IsFalse(int cell) =>
        Assert.False(Bitboard256.Cell(cell).IsEmpty);

    [Fact]
    public void LowestCell_RandomMasks_IsTheLowestSetBit() =>
        Assert.All(
            Numbers().Where(number => !number.IsZero),
            number => Assert.Equal(Cells(number)[0], Mask(number).LowestCell));

    [Fact]
    public void WithoutLowestCell_RepeatedFromAnyMask_WalksTheCellsInOrder() =>
        Assert.All(Numbers(), number => Assert.Equal(Cells(number), Walked(Mask(number))));

    [Fact]
    public void NthCell_EveryRank_IsThatCell() =>
        Assert.All(Numbers(), number =>
        {
            var cells = Cells(number);

            Assert.Equal(cells, Enumerable.Range(0, cells.Length).Select(Mask(number).NthCell));
        });

    [Fact]
    public void CountBelow_EveryCell_CountsTheLowerCells() =>
        Assert.All(Numbers(), number => Assert.All(
            Enumerable.Range(0, Bitboard256.Capacity),
            cell => Assert.Equal(
                Cells(number).Count(held => held < cell), Mask(number).CountBelow(cell))));

    [Theory]
    [MemberData(nameof(EdgeCells))]
    public void HasCell_AMaskOfEveryOtherCell_IsFalseThere(int cell) =>
        Assert.False(Mask(AllCells ^ (BigInteger.One << cell)).HasCell(cell));

    [Fact]
    public void Equality_SameCells_AreEqual() =>
        Assert.Equal(
            Bitboard256.Of([3, 70, 140, 250]), Bitboard256.Cell(250) | Bitboard256.Of([3, 70, 140]));

    private static BigInteger Number(Bitboard256 mask)
    {
        var number = BigInteger.Zero;

        foreach (var cell in Walked(mask))
        {
            number |= BigInteger.One << cell;
        }

        return number;
    }

    private static int[] Walked(Bitboard256 mask)
    {
        var cells = new List<int>();

        for (var rest = mask; !rest.IsEmpty; rest = rest.WithoutLowestCell())
        {
            cells.Add(rest.LowestCell);
        }

        return [.. cells];
    }

    private static int[] Cells(BigInteger number) =>
    [
        .. Enumerable.Range(0, Bitboard256.Capacity)
            .Where(cell => !(number & (BigInteger.One << cell)).IsZero),
    ];

    private static Bitboard256 Mask(BigInteger number) => Bitboard256.Of(Cells(number));

    // Masks drawn at random, half of them thinned by a second draw, and the full and nearly full
    // masks, whose shifts carry across every word.
    private static IEnumerable<BigInteger> Numbers()
    {
        var random = new Random(Seed);

        for (var mask = 0; mask < RandomMasks; mask++)
        {
            yield return Drawn(random);
            yield return Drawn(random) & Drawn(random);
        }

        yield return AllCells;
        yield return AllCells ^ 1;
    }

    // 256 random bits, the top byte left clear so the number is not negative.
    private static BigInteger Drawn(Random random)
    {
        var bytes = new byte[Bytes];
        random.NextBytes(bytes);
        bytes[^1] = 0;

        return new BigInteger(bytes);
    }

    private static IEnumerable<(BigInteger Left, BigInteger Right)> Pairs()
    {
        var numbers = Numbers().ToArray();

        return numbers.Zip(numbers.Skip(1));
    }
}
