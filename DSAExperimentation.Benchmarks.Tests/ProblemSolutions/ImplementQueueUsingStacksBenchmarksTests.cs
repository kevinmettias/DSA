using DSAExperimentation.Benchmarks.ProblemSolutions;
using BclQueue = System.Collections.Generic.Queue<int>;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementQueueUsingStacksBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm - this repo's two-stack queue - so there is no second strategy to reconcile it
// against and the assertion has to come from the arm's own declared contract instead. Its
// [GlobalSetup] builds one fixed, valid script of interleaved push/pop/peek entries (a pop or
// peek only ever follows a push that has not yet been popped) and the arm replays that script
// through the two-stack queue, summing every value it returns so the JIT cannot discard the
// replay. That sum is the arm's answer, and a FIFO queue's contract fixes it: the reference
// replay below drives the identical script - same seed, same roll and value draws, same order -
// through a BCL Queue<int>, so the two sums agree only if the measured queue is genuinely FIFO.
// The script is built inside [GlobalSetup] from a seeded Random and is never shared mutable
// state, so one harness is safe to call any number of times in either order.
public sealed partial class ImplementQueueUsingStacksBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    // The seed [GlobalSetup] builds its script from, restated here because the script itself is
    // private to the harness.
    private const int ScriptSeed = 232;

    // The script's three rolls: 0 pushes the next value, 1 peeks, 2 pops.
    private const int OperationKindCount = 3;

    [Fact]
    public void Setup_SameOperationCount_RebuildsTheSameScript() =>
        Assert.Equal(BuildHarness().TwoStackTransfer(), BuildHarness().TwoStackTransfer());

    [Fact]
    public void TwoStackTransfer_InterleavedScript_MatchesBclQueueReference()
    {
        var harness = BuildHarness();

        Assert.Equal(ReferenceFifoSum(SmallestOperationCount), harness.TwoStackTransfer());
    }

    private static ImplementQueueUsingStacksBenchmarks BuildHarness()
    {
        var harness = new ImplementQueueUsingStacksBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }

    // The script the harness builds, replayed through a BCL Queue<int> instead of the two-stack
    // queue: roll 0 pushes the next value, roll 1 peeks, roll 2 pops - and while nothing is
    // pending the roll is forced to a push without drawing, exactly as the harness does.
    private static int ReferenceFifoSum(int operationCount)
    {
        var random = new Random(ScriptSeed);
        var reference = new BclQueue();
        var sum = 0;
        var pending = 0;

        for (var i = 0; i < operationCount; i++)
        {
            var roll = pending > 0 ? random.Next(0, OperationKindCount) : 0;

            if (roll == 0)
            {
                reference.Enqueue(random.Next(0, operationCount + 1));
                pending++;
            }
            else if (roll == 1)
            {
                sum += reference.Peek();
            }
            else
            {
                sum += reference.Dequeue();
                pending--;
            }
        }

        return sum;
    }
}
