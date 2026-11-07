namespace DSAExperimentation.Domain.Computing.FloatingPoint;

// The ten classes IEEE 754 sorts every floating-point datum into (its "class" operation, 5.7.2), in
// the standard's order. A signaling NaN raises the invalid-operation flag when an operation reads
// it, and a quiet one passes through; zeros, subnormals, normals and infinities come in both signs.
/// <summary>IEEE 754's classes of a floating-point datum.</summary>
public enum FloatingPointClass
{
    /// <summary>A NaN that signals when an operation reads it.</summary>
    SignalingNaN,

    /// <summary>A NaN that an operation passes through quietly.</summary>
    QuietNaN,

    /// <summary>Negative infinity.</summary>
    NegativeInfinity,

    /// <summary>A negative number with the full precision of the format.</summary>
    NegativeNormal,

    /// <summary>A negative number below the smallest normal, with fewer significant bits.</summary>
    NegativeSubnormal,

    /// <summary>Zero with its sign bit set.</summary>
    NegativeZero,

    /// <summary>Zero with its sign bit clear.</summary>
    PositiveZero,

    /// <summary>A positive number below the smallest normal, with fewer significant bits.</summary>
    PositiveSubnormal,

    /// <summary>A positive number with the full precision of the format.</summary>
    PositiveNormal,

    /// <summary>Positive infinity.</summary>
    PositiveInfinity,
}
