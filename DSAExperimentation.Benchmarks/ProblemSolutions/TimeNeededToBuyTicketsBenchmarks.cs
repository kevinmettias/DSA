using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.TimeNeededToBuyTickets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TimeNeededToBuyTicketsSolution's, the same methods
// TimeNeededToBuyTicketsSolutionTests proves correct - this repo's Queue<(int, int)>
// replaying the line, O(total tickets sold before targetPerson finishes), against
// the O(n) closed-form sum. _tickets uses a large, uniform ticket count per person so
// the simulation is forced through its full O(n * ticketsPerPerson) worst case
// instead of finishing after a handful of turns.
public class TimeNeededToBuyTicketsBenchmarks
{
    private const int RandomSeed = 2073; // LC problem number
    private const int MinTickets = 500;
    private const int MaxTicketsExclusive = 1_000;

    private int[] _tickets = [];

    private int _targetPerson;
    [Params(50, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _tickets = SeededDraws.Values(Length, MinTickets, MaxTicketsExclusive, random);
        _targetPerson = Length / AlgorithmConstants.HalvingFactor;
    }

    [Benchmark(Baseline = true)]
    public int QueueSimulation() =>
        TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByQueueSimulation(_tickets, _targetPerson);

    [Benchmark]
    public int ClosedFormSum() =>
        TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByClosedFormSum(_tickets, _targetPerson);
}
