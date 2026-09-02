namespace DSAExperimentation.LeetCode.MinimumTimeToTransportAllIndividuals;

// One node per (mask of individuals already at the destination, current
// environmental stage) pair. Edges hold every reachable combined round -
// "send a group across, then, unless that finished everyone, send one of
// them back" - weighted by the total minutes the round takes. Answers LC
// 3594 alone; a fully generic weighted node belongs in DataStructures, not
// here, but nothing that generic exists in DSAExperimentation yet
// (DigitStepNode's own doc comment makes the same call for LC 3377's
// identical shape).
internal sealed class TransportState(int mask, int stage)
{
    public int Mask { get; } = mask;
    public int Stage { get; } = stage;
    public List<(double Time, TransportState Target)> Edges { get; } = [];

    public override string ToString() => $"(mask={Mask}, stage={Stage})";
}
