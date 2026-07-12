using System;

namespace PalCalc.Solver
{
    public static class BreedingEffortCalculator
    {
        // Effort (wall-clock time) to breed one target pal, given the expected number of eggs that must be
        // produced/hatched to obtain it. Each breeding cycle yields `eggsPerBreeding` eggs (Vegetable Cake = 2,
        // otherwise 1), so the number of breeding-farm cycles is `ceil(avgRequiredEggs / eggsPerBreeding)`.
        // Incubation is per-egg and unaffected by the cake.
        public static TimeSpan SelfBreedingEffort(
            int avgRequiredEggs,
            TimeSpan timePerBreed,
            TimeSpan incubationTime,
            bool multipleIncubators,
            int eggsPerBreeding)
        {
            var breedingCycles = (int)Math.Ceiling(avgRequiredEggs / (double)eggsPerBreeding);
            var totalBreedingTime = breedingCycles * timePerBreed;
            var totalIncubationTime = avgRequiredEggs * incubationTime;

            if (multipleIncubators)
            {
                // eggs incubate in parallel, so the only bottleneck is producing them: all breeding cycles
                // plus a single incubation for the final egg.
                return totalBreedingTime + incubationTime;
            }

            // single incubator: breeding and incubation happen sequentially. Whichever dominates, the other
            // still has to run at least once.
            var allIncubationWithBreeding = totalIncubationTime + timePerBreed;
            var allBreedingWithIncubation = totalBreedingTime + incubationTime;

            return allIncubationWithBreeding > allBreedingWithIncubation
                ? allIncubationWithBreeding
                : allBreedingWithIncubation;
        }
    }
}
