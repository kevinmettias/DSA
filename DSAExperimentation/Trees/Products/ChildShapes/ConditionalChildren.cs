namespace DSAExperimentation.Trees;

public readonly record struct ConditionalChildren<T>(
    T Condition,
    T Then,
    T Else);
