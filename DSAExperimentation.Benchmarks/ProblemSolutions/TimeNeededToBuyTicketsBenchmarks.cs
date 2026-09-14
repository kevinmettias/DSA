using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TimeNeededToBuyTickets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TimeNeededToBuyTicketsSolution's, the same methods
// TimeNeededToBuyTicketsTests proves correct - this repo's Queue<(int, int)>
// replaying the line, O(total tickets sold before person k finishes), against the
// O(n) closed-form sum. _tickets uses a large, uniform ticket count per person so
// the simulation is forced through its full O(n * ticketsPerPerson) worst case
// instead of finishing after a handful of turns.
[MemoryDiagnoser]
public class TimeNeededToBuyTicketsBenchmarks
{
    private const int RandomSeed = 2073; // LC problem number
    private const int MinTickets = 500;
    private const int MaxTicketsExclusive = 1_000;
    private const int MidpointDivisor = 2;

    [Params(50, 300)]
    public int Length;

    private int[] _tickets = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _tickets = Enumerable.Range(0, Length).Select(_ => random.Next(MinTickets, MaxTicketsExclusive)).ToArray();
        _k = Length / MidpointDivisor;
    }

    [Benchmark(Baseline = true)]
    public int QueueSimulation() =>
        TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByQueueSimulation(_tickets, _k);

    [Benchmark]
    public int ClosedFormSum() =>
        TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByClosedFormSum(_tickets, _k);
}
