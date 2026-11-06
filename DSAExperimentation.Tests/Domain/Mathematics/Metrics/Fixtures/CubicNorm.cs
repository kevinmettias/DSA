using DSAExperimentation.Domain.Mathematics.Metrics;

namespace DSAExperimentation.Tests.Domain.Mathematics.Metrics.Fixtures;

// The L3 norm, test-only: a norm of an exponent no preset fixes, written as a caller would write
// one, a struct that implements the contract and reads the general form. It holds LpNorm to the
// metric axioms at an exponent where every coordinate goes through a power.
internal readonly struct CubicNorm : ILpNorm
{
    public static double Exponent => 3.0;

    public static double Length(ReadOnlySpan<double> vector) => LpNorm.Length(vector, Exponent);

    public static double Distance(ReadOnlySpan<double> from, ReadOnlySpan<double> to) =>
        LpNorm.Distance(from, to, Exponent);
}
