namespace DSAExperimentation.Domain.Computing.Binary;

// How many bits a machine integer holds: a bit-by-bit walk over an int, a bit trie over int keys and
// a count of set bits all run over the same 32 places.
/// <summary>The widths, in bits, of the machine integers.</summary>
public static class BitWidths
{
    /// <summary>The bits in a 32-bit integer, <see cref="int"/> or <see cref="uint"/>.</summary>
    public const int Int32 = 32;
}
