using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace DSAExperimentation.Domain.Games.PositionalGames;

// A board of up to 256 cells as a bit mask, bit c for cell c, held in four 64-bit words with cell
// 0 the lowest bit of the first: what a ulong is to a board of at most 64 cells (BitboardPosition),
// for the boards beyond, Gomoku's 225 among them. The bitwise operations act word by word, and a
// shift carries bits across a word's edge, so a mask shifts as one 256-bit number and bits shifted
// past either end are lost. A shift by a constant folds to the few words it touches, which is why
// the operations ask to be inlined: a line rule shifts the same mask by the same step many times.
// A value, so a position that holds two of them is copied, never shared. Four words measured
// against Vector256<ulong> is left open: the shifts cross the vector's lanes, which Vector256 has
// no instruction for.
/// <summary>A bit mask over 256 cells, held in four 64-bit words.</summary>
public readonly record struct Bitboard256
{
    // How many cells a mask can hold. Internal, so that a solution's own 256 is not read as a copy
    // of it (ARCHITECTURE 17.9).
    internal const int Capacity = 256;

    private const int WordBits = 64;
    private const int WordShift = 6;
    private const int BitInWord = WordBits - 1;

    // The words by number, and the first cell each holds.
    private const int FirstWord = 0;
    private const int SecondWord = 1;
    private const int ThirdWord = 2;
    private const int FourthWord = 3;
    private const int SecondWordStart = SecondWord * WordBits;
    private const int ThirdWordStart = ThirdWord * WordBits;
    private const int FourthWordStart = FourthWord * WordBits;

    // Cells 0-63, 64-127, 128-191 and 192-255, each word's lowest bit its lowest cell.
    private readonly ulong _first;
    private readonly ulong _second;
    private readonly ulong _third;
    private readonly ulong _fourth;

    /// <summary>Gets the mask that holds no cell.</summary>
    public static Bitboard256 Empty => default;

    /// <summary>Gets whether the mask holds no cell.</summary>
    public bool IsEmpty => (_first | _second | _third | _fourth) == 0;

    /// <summary>Gets how many cells the mask holds.</summary>
    public int Count =>
        BitOperations.PopCount(_first) + BitOperations.PopCount(_second)
        + BitOperations.PopCount(_third) + BitOperations.PopCount(_fourth);

    /// <summary>Gets the lowest cell the mask holds, or 256 when it holds none.</summary>
    public int LowestCell
    {
        get
        {
            if (_first != 0)
            {
                return BitOperations.TrailingZeroCount(_first);
            }

            if (_second != 0)
            {
                return SecondWordStart + BitOperations.TrailingZeroCount(_second);
            }

            if (_third != 0)
            {
                return ThirdWordStart + BitOperations.TrailingZeroCount(_third);
            }

            return FourthWordStart + BitOperations.TrailingZeroCount(_fourth);
        }
    }

    // Private: a mask is made from cells or by the type's own operations, so its words are the
    // representation and never part of the contract.
    private Bitboard256(ulong first, ulong second, ulong third, ulong fourth)
    {
        _first = first;
        _second = second;
        _third = third;
        _fourth = fourth;
    }

    /// <summary>Makes the mask of one cell.</summary>
    /// <param name="cell">The cell, from 0 to 255.</param>
    /// <returns>The mask holding that cell alone.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The cell is outside the mask.</exception>
    public static Bitboard256 Cell(int cell)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
            (uint)cell, (uint)Capacity, nameof(cell));

        return OneCell(cell);
    }

    /// <summary>Makes the mask of a list of cells.</summary>
    /// <param name="cells">Cells from 0 to 255.</param>
    /// <returns>The mask holding every listed cell.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A cell is outside the mask.</exception>
    public static Bitboard256 Of(ReadOnlySpan<int> cells)
    {
        var mask = Empty;

        foreach (var cell in cells)
        {
            mask |= Cell(cell);
        }

        return mask;
    }

    /// <summary>Whether the mask holds a cell.</summary>
    /// <param name="cell">The cell, from 0 to 255.</param>
    /// <returns><see langword="true"/> when the cell's bit is set.</returns>
    public bool HasCell(int cell) => !(this & OneCell(cell)).IsEmpty;

    /// <summary>Counts the cells the mask holds below a cell.</summary>
    /// <param name="cell">The cell, from 0 to 255.</param>
    /// <returns>How many held cells are numbered lower.</returns>
    public int CountBelow(int cell)
    {
        var word = cell >> WordShift;
        var below = (1UL << (cell & BitInWord)) - 1;
        var whole = word switch
        {
            FirstWord => 0,
            SecondWord => BitOperations.PopCount(_first),
            ThirdWord => BitOperations.PopCount(_first) + BitOperations.PopCount(_second),
            _ => Count - BitOperations.PopCount(_fourth),
        };

        return whole + BitOperations.PopCount(WordAt(word) & below);
    }

    /// <summary>Finds the cell held at a rank: the index-th held cell, lowest first.</summary>
    /// <param name="index">The rank, from 0 below <see cref="Count"/>.</param>
    /// <returns>The cell.</returns>
    public int NthCell(int index)
    {
        var word = FirstWord;
        var bits = _first;
        var inWord = BitOperations.PopCount(bits);

        while (index >= inWord)
        {
            index -= inWord;
            word++;
            bits = WordAt(word);
            inWord = BitOperations.PopCount(bits);
        }

        return (word << WordShift) + NthBit(bits, index);
    }

    // The index-th set bit of a word, lowest first: one instruction where the processor deposits
    // bits (BMI2's PDEP), otherwise the lower set bits cleared one by one.
    private static int NthBit(ulong bits, int index)
    {
        if (Bmi2.X64.IsSupported)
        {
            var deposited = Bmi2.X64.ParallelBitDeposit(1UL << index, bits);

            return BitOperations.TrailingZeroCount(deposited);
        }

        for (var skipped = 0; skipped < index; skipped++)
        {
            bits &= bits - 1;
        }

        return BitOperations.TrailingZeroCount(bits);
    }

    /// <summary>The mask without its lowest cell.</summary>
    /// <returns>The same cells less the lowest; the empty mask stays empty.</returns>
    public Bitboard256 WithoutLowestCell()
    {
        if (_first != 0)
        {
            return new(_first & (_first - 1), _second, _third, _fourth);
        }

        if (_second != 0)
        {
            return new(0, _second & (_second - 1), _third, _fourth);
        }

        if (_third != 0)
        {
            return new(0, 0, _third & (_third - 1), _fourth);
        }

        return new(0, 0, 0, _fourth & (_fourth - 1));
    }

    /// <summary>Moves every cell up by a count: cell c becomes cell c + count.</summary>
    /// <param name="count">How far to move, from 0; cells moved past the last are lost.</param>
    /// <returns>The moved mask.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bitboard256 ShiftUp(int count) =>
        WholeWordsUp(count >> WordShift).BitsUp(count & BitInWord);

    // Moves every cell up by fewer than a word's bits, each word's top bits carried into the next.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Bitboard256 BitsUp(int bits)
    {
        if (bits == 0)
        {
            return this;
        }

        var back = WordBits - bits;

        return new(
            _first << bits,
            (_second << bits) | (_first >> back),
            (_third << bits) | (_second >> back),
            (_fourth << bits) | (_third >> back));
    }

    // Moves the words up by whole words, each word's cells keeping their place in it.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Bitboard256 WholeWordsUp(int words) => words switch
    {
        FirstWord => this,
        SecondWord => new(0, _first, _second, _third),
        ThirdWord => new(0, 0, _first, _second),
        FourthWord => new(0, 0, 0, _first),
        _ => Empty,
    };

    /// <summary>Moves every cell down by a count: cell c becomes cell c - count.</summary>
    /// <param name="count">How far to move, from 0; cells moved below cell 0 are lost.</param>
    /// <returns>The moved mask.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bitboard256 ShiftDown(int count) =>
        WholeWordsDown(count >> WordShift).BitsDown(count & BitInWord);

    // Moves every cell down by fewer than a word's bits, each word's low bits carried into the one
    // before.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Bitboard256 BitsDown(int bits)
    {
        if (bits == 0)
        {
            return this;
        }

        var back = WordBits - bits;

        return new(
            (_first >> bits) | (_second << back),
            (_second >> bits) | (_third << back),
            (_third >> bits) | (_fourth << back),
            _fourth >> bits);
    }

    // Moves the words down by whole words, each word's cells keeping their place in it.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Bitboard256 WholeWordsDown(int words) => words switch
    {
        FirstWord => this,
        SecondWord => new(_second, _third, _fourth, 0),
        ThirdWord => new(_third, _fourth, 0, 0),
        FourthWord => new(_fourth, 0, 0, 0),
        _ => Empty,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Bitboard256 OneCell(int cell)
    {
        var bit = 1UL << (cell & BitInWord);

        return (cell >> WordShift) switch
        {
            FirstWord => new(bit, 0, 0, 0),
            SecondWord => new(0, bit, 0, 0),
            ThirdWord => new(0, 0, bit, 0),
            _ => new(0, 0, 0, bit),
        };
    }

    private ulong WordAt(int word) => word switch
    {
        FirstWord => _first,
        SecondWord => _second,
        ThirdWord => _third,
        _ => _fourth,
    };

    /// <summary>The cells both masks hold.</summary>
    /// <param name="left">A mask.</param>
    /// <param name="right">Another mask.</param>
    /// <returns>Their intersection.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard256 operator &(Bitboard256 left, Bitboard256 right) =>
        new(left._first & right._first, left._second & right._second,
            left._third & right._third, left._fourth & right._fourth);

    /// <summary>The cells either mask holds.</summary>
    /// <param name="left">A mask.</param>
    /// <param name="right">Another mask.</param>
    /// <returns>Their union.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard256 operator |(Bitboard256 left, Bitboard256 right) =>
        new(left._first | right._first, left._second | right._second,
            left._third | right._third, left._fourth | right._fourth);

    /// <summary>The cells exactly one of the masks holds.</summary>
    /// <param name="left">A mask.</param>
    /// <param name="right">Another mask.</param>
    /// <returns>Their symmetric difference.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard256 operator ^(Bitboard256 left, Bitboard256 right) =>
        new(left._first ^ right._first, left._second ^ right._second,
            left._third ^ right._third, left._fourth ^ right._fourth);

    /// <summary>Every cell the mask does not hold.</summary>
    /// <param name="mask">A mask.</param>
    /// <returns>Its complement over all 256 cells.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bitboard256 operator ~(Bitboard256 mask) =>
        new(~mask._first, ~mask._second, ~mask._third, ~mask._fourth);
}
