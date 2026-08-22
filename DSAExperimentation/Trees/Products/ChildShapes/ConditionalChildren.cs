namespace DSAExperimentation.Trees;

public readonly record struct ConditionalChildren<TChild>(
    TChild Condition,
    TChild Then,
    TChild Else);




