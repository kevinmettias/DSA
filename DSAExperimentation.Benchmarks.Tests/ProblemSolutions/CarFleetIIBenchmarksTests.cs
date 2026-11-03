using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CarFleetIIBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms
// to each other, and this pins the baseline's answer to a value derived from the fleet's
// construction without any arm. Answers come back one per car in car order, which is part of this
// answer: collision time i belongs to car i, so AnswerGraphText.Of and not OfUnordered is the
// rendering that keeps each time scored against its own car.
public sealed partial class CarFleetIIBenchmarksTests
{
    private const int SmallestLength = 200;

    // The sentinel both strategies report for a car that never collides - and the value every car
    // in this fleet gets. Setup builds cars[i] with position (i + 1) * 10 and speed i + 1, so the
    // car at the back of each pair is always the slower of the two and nothing ever catches
    // anything: the fleet must answer with this sentinel once per car. The fleet is a closed form
    // over Length with no draw from any stream, so the same Length is the whole of what pins it.
    private const double NoCollisionTime = -1;

    [Fact]
    public void BruteForcePerCar_CarsSlowerThanTheOnesAhead_ReportsNoCollisionForEveryCar() =>
        Assert.Equal(
            AnswerGraphText.Of(Enumerable.Repeat(NoCollisionTime, SmallestLength)),
            AnswerGraphText.Of(BuildHarness().BruteForcePerCar()));

    private static CarFleetIIBenchmarks BuildHarness()
    {
        var harness = new CarFleetIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
