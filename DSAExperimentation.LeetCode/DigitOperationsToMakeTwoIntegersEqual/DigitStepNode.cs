namespace DSAExperimentation.LeetCode.DigitOperationsToMakeTwoIntegersEqual;

// One node per non-prime integer sharing n and m's digit count. Edges hold
// every reachable single-digit +-1 mutation that also lands on a non-prime
// value, weighted by the mutated value itself - the transformation's own
// "cost of a move is the number you land on" rule. Answers LC 3377 alone; a
// fully generic weighted node belongs in DataStructures, not here, but
// nothing that generic exists in DSAExperimentation yet (EdgeGraphNode's own
// doc comment makes the same call for LC 3123's identical shape).
internal sealed class DigitStepNode(int value)
{
    public int Value { get; } = value;

    public List<(int Weight, DigitStepNode Target)> Edges { get; } = [];

    public override string ToString() => Value.ToString();
}
