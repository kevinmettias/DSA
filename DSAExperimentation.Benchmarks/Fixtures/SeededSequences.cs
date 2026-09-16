namespace DSAExperimentation.Benchmarks.Fixtures;

// Every benchmark that measures an operation on a sequential input wants the same
// warm-up shape: the integers 0..n-1 (or 1..n) in a seeded random order, so the
// input is permutation-representative yet byte-identical from run to run. Nine
// harnesses across six problems were each writing that Range-plus-Fisher-Yates loop
// inline - the workload sizing §17.7 keeps in a harness, so this is where it lives.
//
// Both a seed and a Random are accepted because a harness that draws further values
// after the shuffle (RobotCollisions' healths and directions) must keep ONE stream:
// handing it a fresh Random would change every draw after the shuffle, and with it
// the workload a recorded benchmark artifact was measured against.
internal static class SeededSequences
{
    // Positions and array indices: 0 through count - 1.
    public static int[] ShuffledZeroTo(int count, int seed) => ShuffledZeroTo(count, new Random(seed));

    public static int[] ShuffledZeroTo(int count, Random random) => Shuffled(Enumerable.Range(0, count).ToArray(), random);

    // Node values and 1-based positions: 1 through count.
    public static int[] ShuffledOneTo(int count, int seed) => ShuffledOneTo(count, new Random(seed));

    public static int[] ShuffledOneTo(int count, Random random) => Shuffled(Enumerable.Range(1, count).ToArray(), random);

    // Fisher-Yates, drawing downward from the final index so every permutation is
    // equally likely rather than merely "shuffled".
    private static int[] Shuffled(int[] values, Random random)
    {
        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values;
    }
}
