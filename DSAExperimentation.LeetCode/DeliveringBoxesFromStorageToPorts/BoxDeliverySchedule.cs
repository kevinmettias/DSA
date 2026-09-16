namespace DSAExperimentation.LeetCode.DeliveringBoxesFromStorageToPorts;

// LC 1687's box list reduced to the two prefix scans every strategy reads: how
// heavy the first k boxes are, and how many times the port changes between
// neighbouring boxes among the first k. A trip covering boxes [j, i) costs
// 2 + (port changes strictly inside it), and both of those are prefix
// subtractions once this is built, which is what turns the dp into a sliding
// window at all.
//
// Meaningless outside this problem, so it lives beside the solution
// (ARCHITECTURE.md section 17.3) rather than in Domain/. It is also the
// prepared-input shape section 17.4 calls for: the benchmark builds it once in
// [GlobalSetup] and hands it to the measured method, and since it is not an
// IEnumerable it can never be confused with the int[][] overload.
internal sealed class BoxDeliverySchedule(int[] portSwitchPrefix, long[] weightPrefix)
{
    // Each box is LeetCode's [port, weight] pair.
    private const int PortSlot = 0;
    private const int WeightSlot = 1;

    // A port change needs two boxes to compare, so nothing can change before the
    // second one.
    private const int FirstComparablePosition = 2;

    // How many boxes the storage holds, in delivery order.
    public int BoxCount => weightPrefix.Length - 1;

    public static BoxDeliverySchedule Build(int[][] boxes)
    {
        var portSwitchPrefix = new int[boxes.Length + 1];
        var weightPrefix = new long[boxes.Length + 1];

        for (var i = 1; i <= boxes.Length; i++)
        {
            weightPrefix[i] = weightPrefix[i - 1] + boxes[i - 1][WeightSlot];
            portSwitchPrefix[i] = i < FirstComparablePosition
                ? 0
                : SwitchesThrough(portSwitchPrefix, boxes, i);
        }

        return new BoxDeliverySchedule(portSwitchPrefix, weightPrefix);
    }

    // Port changes counted through the first `position` boxes: the running count
    // up to the box before it, plus whether the port changes into it.
    private static int SwitchesThrough(int[] portSwitchPrefix, int[][] boxes, int position)
        => portSwitchPrefix[position - 1] + SwitchesInto(boxes, position);

    private static int SwitchesInto(int[][] boxes, int position)
        => HasPortChangedInto(boxes, position) ? 1 : 0;

    // Whether the destination port differs between the box at `position` and the
    // one immediately before it.
    private static bool HasPortChangedInto(int[][] boxes, int position)
        => boxes[position - FirstComparablePosition][PortSlot] != boxes[position - 1][PortSlot];

    // The number of times the destination port changes between consecutive boxes
    // among the first `boxCount` of them - so the extra legs a single trip over
    // that whole prefix would pay, on top of leaving and returning.
    public int PortSwitchesAmongFirst(int boxCount) => portSwitchPrefix[boxCount];

    // Total weight of the first `boxCount` boxes; the ship's load limit is checked
    // as a difference of two of these.
    public long WeightOfFirst(int boxCount) => weightPrefix[boxCount];
}
