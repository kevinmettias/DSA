namespace DSAExperimentation.Domain.Mathematics.ExtendedReals;

// The operations an indeterminate form combines two limits by.
/// <summary>An operation on two limits.</summary>
public enum LimitOperation
{
    /// <summary>The first limit minus the second.</summary>
    Difference,

    /// <summary>The first limit times the second.</summary>
    Product,

    /// <summary>The first limit over the second.</summary>
    Quotient,

    /// <summary>The first limit raised to the second.</summary>
    Power,
}
