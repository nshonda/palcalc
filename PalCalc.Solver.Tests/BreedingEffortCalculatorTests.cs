using System;

namespace PalCalc.Solver.Tests
{
    [TestClass]
    public class BreedingEffortCalculatorTests
    {
        // With multiple incubators, self-breeding effort = (breeding cycles) * timePerBreed + one incubation.
        // The Vegetable Cake yields 2 eggs per breeding cycle instead of 1, so the number of breeding
        // cycles (the breeding-farm time) halves, while incubation is unchanged.
        [TestMethod]
        public void VegetableCake_HalvesBreedingCycleTime_WithMultipleIncubators()
        {
            var timePerBreed = TimeSpan.FromMinutes(10);
            var incubation = TimeSpan.FromMinutes(120);
            int avgRequiredEggs = 10;

            var withoutCake = BreedingEffortCalculator.SelfBreedingEffort(
                avgRequiredEggs, timePerBreed, incubation, multipleIncubators: true, eggsPerBreeding: 1);
            var withVegetableCake = BreedingEffortCalculator.SelfBreedingEffort(
                avgRequiredEggs, timePerBreed, incubation, multipleIncubators: true, eggsPerBreeding: 2);

            // 10 eggs @ 1/cycle = 10 cycles * 10min = 100min breeding + 120min incubation
            Assert.AreEqual(TimeSpan.FromMinutes(220), withoutCake);
            // 10 eggs @ 2/cycle =  5 cycles * 10min =  50min breeding + 120min incubation
            Assert.AreEqual(TimeSpan.FromMinutes(170), withVegetableCake);
        }

        // Characterization: with no cake (1 egg/cycle) the multiple-incubator result must equal the
        // pre-existing formula (avgRequiredEggs * timePerBreed + one incubation) — the refactor of
        // BredPalReference must not change this.
        [TestMethod]
        public void NoCake_MultipleIncubators_MatchesLegacyFormula()
        {
            var result = BreedingEffortCalculator.SelfBreedingEffort(
                avgRequiredEggs: 10, timePerBreed: TimeSpan.FromMinutes(10),
                incubationTime: TimeSpan.FromMinutes(120), multipleIncubators: true, eggsPerBreeding: 1);

            Assert.AreEqual(TimeSpan.FromMinutes(10 * 10) + TimeSpan.FromMinutes(120), result);
        }

        // Characterization: with no cake and a single incubator, the result is max(all-incubation+one-breed,
        // all-breeding+one-incubation). Here incubation dominates.
        [TestMethod]
        public void NoCake_SingleIncubator_MatchesLegacyMaxFormula()
        {
            var result = BreedingEffortCalculator.SelfBreedingEffort(
                avgRequiredEggs: 10, timePerBreed: TimeSpan.FromMinutes(10),
                incubationTime: TimeSpan.FromMinutes(120), multipleIncubators: false, eggsPerBreeding: 1);

            // max(10*120 + 10, 10*10 + 120) = max(1210, 220) = 1210
            Assert.AreEqual(TimeSpan.FromMinutes(1210), result);
        }

        // In the breeding-bound single-incubator regime, the cake genuinely helps: it halves the breeding cycles.
        [TestMethod]
        public void VegetableCake_ReducesBreedingBoundEffort_WithSingleIncubator()
        {
            TimeSpan Effort(int eggsPerBreeding) => BreedingEffortCalculator.SelfBreedingEffort(
                avgRequiredEggs: 10, timePerBreed: TimeSpan.FromMinutes(100),
                incubationTime: TimeSpan.FromMinutes(5), multipleIncubators: false, eggsPerBreeding: eggsPerBreeding);

            // no cake:  max(10*5 + 100, 10*100 + 5) = max(150, 1005) = 1005
            Assert.AreEqual(TimeSpan.FromMinutes(1005), Effort(1));
            // cake:     max(10*5 + 100,  5*100 + 5) = max(150,  505) =  505
            Assert.AreEqual(TimeSpan.FromMinutes(505), Effort(2));
        }

        // Correctly a no-op when incubation is the bottleneck with a single incubator — faster egg production
        // can't beat the incubation queue.
        [TestMethod]
        public void VegetableCake_DoesNotHelp_WhenIncubationBound_WithSingleIncubator()
        {
            TimeSpan Effort(int eggsPerBreeding) => BreedingEffortCalculator.SelfBreedingEffort(
                avgRequiredEggs: 10, timePerBreed: TimeSpan.FromMinutes(10),
                incubationTime: TimeSpan.FromMinutes(120), multipleIncubators: false, eggsPerBreeding: eggsPerBreeding);

            Assert.AreEqual(Effort(1), Effort(2));
        }
    }
}
