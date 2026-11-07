namespace DSAExperimentation.Domain.Computing.FloatingPoint;

// IEEE 754's binary64 format, .NET's double: a sign bit, 11 exponent bits and 52 fraction bits. An
// exponent field of all ones holds the infinities, with a zero fraction, and the NaNs, whose top
// fraction bit says quiet (set) or signaling (clear); one of all zeros holds the zeros and the
// subnormals, which give up leading bits to reach below the smallest normal number, so underflow is
// gradual. Every other exponent field is a normal number: 1.fraction times 2 to the field less the
// bias, 1023.
//
// What .NET already states is read from it, never written again: its infinities, NaN, negative zero
// and largest value are double's constants, and double.Epsilon is IEEE's smallest subnormal,
// 2^-1074, not the machine epsilon, the gap between 1 and the next double, 2^-52, which is stated
// here. The field widths are IEEE's, which .NET keeps internal, so they are stated once and the
// tests hold them to double's own encoding. They are properties over private constants, since a
// public constant of 52 or 1023 would be read as the one source of every solution's own
// (SharedValueCopyTests).
/// <summary>
/// The IEEE 754 binary64 format: double's field widths, its epsilons and its classes.
/// </summary>
public static class Binary64
{
    private const int FractionBitCount = 52;
    private const int ExponentBitCount = 11;
    private const int Bias = (1 << (ExponentBitCount - 1)) - 1;
    private const long FractionMask = (1L << FractionBitCount) - 1;
    private const long QuietBit = 1L << (FractionBitCount - 1);

    /// <summary>The gap between 1 and the next double, 2^-52: the machine epsilon.</summary>
    public const double MachineEpsilon = 1.0 / (1L << FractionBitCount);

    /// <summary>Gets the stored fraction bits, 52; the significand has one more, implied.</summary>
    public static int FractionBits => FractionBitCount;

    /// <summary>Gets the exponent field's bits, 11.</summary>
    public static int ExponentBits => ExponentBitCount;

    /// <summary>
    /// Gets the exponent bias, 1023: a normal number's exponent is its field less this.
    /// </summary>
    public static int ExponentBias => Bias;

    /// <summary>
    /// Gets the smallest positive subnormal, 2^-1074, which .NET names double.Epsilon.
    /// </summary>
    public static double SmallestSubnormal => double.Epsilon;

    /// <summary>
    /// Gets the smallest positive normal number, 2^-1022: an exponent field of one, no fraction.
    /// </summary>
    public static double SmallestNormal { get; } = BitConverter.Int64BitsToDouble(FractionMask + 1);

    /// <summary>Sorts a double into its IEEE 754 class.</summary>
    /// <param name="value">The double to classify.</param>
    /// <returns>Its class: a NaN by its quiet bit, otherwise its kind and sign.</returns>
    public static FloatingPointClass ClassOf(double value)
    {
        if (double.IsNaN(value))
        {
            return NaNClassOf(value);
        }

        if (double.IsNegative(value))
        {
            return NegativeClassOf(value);
        }

        return PositiveClassOf(value);
    }

    private static FloatingPointClass NaNClassOf(double value)
    {
        var isQuiet = (BitConverter.DoubleToInt64Bits(value) & QuietBit) != 0;
        return isQuiet ? FloatingPointClass.QuietNaN : FloatingPointClass.SignalingNaN;
    }

    private static FloatingPointClass NegativeClassOf(double value)
    {
        if (double.IsInfinity(value))
        {
            return FloatingPointClass.NegativeInfinity;
        }

        if (value == 0)
        {
            return FloatingPointClass.NegativeZero;
        }

        return double.IsSubnormal(value)
            ? FloatingPointClass.NegativeSubnormal
            : FloatingPointClass.NegativeNormal;
    }

    private static FloatingPointClass PositiveClassOf(double value)
    {
        if (double.IsInfinity(value))
        {
            return FloatingPointClass.PositiveInfinity;
        }

        if (value == 0)
        {
            return FloatingPointClass.PositiveZero;
        }

        return double.IsSubnormal(value)
            ? FloatingPointClass.PositiveSubnormal
            : FloatingPointClass.PositiveNormal;
    }
}
