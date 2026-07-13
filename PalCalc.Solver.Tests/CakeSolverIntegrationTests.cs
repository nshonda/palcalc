using PalCalc.Model;
using PalCalc.Solver.PalReference;
using PalCalc.Solver.ResultPruning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PalCalc.Solver.Tests
{
    // End-to-end solver test for the probabilistic IV-cake range: two near-target parents (IV 98,
    // gap 2 below a 100 target) can reach a 100-IV child WITH a Mushroom Cake (via the +1..5 bonus
    // roll) but NOT without it. This pins the feature through the real solver, complementing the
    // unit tests of ApplyCakeBonusToTarget.
    [TestClass]
    public class CakeSolverIntegrationTests
    {
        private static readonly PalDB db = PalDB.LoadEmbedded();

        // Lamball (PalDex 1) is a self-breeder: Lamball x Lamball -> Lamball, so a 1-step solve suffices.
        private static PalInstance MakeParent(string id, PalGender gender, int ivHp) => new PalInstance
        {
            InstanceId = id,
            OwnerPlayerId = "test-player",
            Pal = "Lamball".ToPal(db),
            Gender = gender,
            PassiveSkills = new List<PassiveSkill>(),
            ActiveSkills = new List<ActiveSkill>(),
            EquippedActiveSkills = new List<ActiveSkill>(),
            Location = new PalLocation { Type = LocationType.Base, Index = 0 },
            IV_HP = ivHp,
            IV_Shot = 0,
            IV_Defense = 0,
        };

        private static List<IPalReference> SolveForHp100(BreedingCake cake)
        {
            var owned = new List<PalInstance>
            {
                MakeParent("p1", PalGender.MALE, 98),
                MakeParent("p2", PalGender.FEMALE, 98),
            };

            var settings = new BreedingSolverSettings(
                db: db,
                gameSettings: new GameSettings { ActiveCake = cake },
                ownedPals: owned,
                pruningBuilder: PruningRulesBuilder.Default,
                maxBreedingSteps: 3,
                maxSolverIterations: 3,
                maxWildPals: 0,
                allowedWildPals: new List<Pal>(),
                bannedBredPals: new List<Pal>(),
                maxInputIrrelevantPassives: 0,
                maxBredIrrelevantPassives: 0,
                maxEffort: TimeSpan.FromDays(7),
                maxThreads: 1,
                maxSurgeryCost: 0,
                allowedSurgeryPassives: new List<PassiveSkill>(),
                useGenderReversers: false
            );

            var solver = new BreedingSolver(settings);
            var spec = new PalSpecifier { Pal = "Lamball".ToPal(db), IV_HP = 100 };
            var controller = new SolverStateController { CancellationToken = CancellationToken.None };
            return solver.SolveFor(spec, controller);
        }

        [TestMethod]
        public void MushroomCake_LetsNearTargetParentsReachTargetIV()
        {
            var reaching = SolveForHp100(BreedingCake.MushroomCake)
                .OfType<BredPalReference>()
                .Where(r => r.Pal.Name == "Lamball" && r.IVs.HP.Satisfies(100))
                .ToList();

            Assert.IsTrue(reaching.Any(),
                "with a Mushroom Cake, two 98-IV parents should be able to reach a 100-IV child");

            var best = reaching.First();
            Assert.AreEqual(100, best.IVs.HP.Min);
            Assert.IsTrue(best.IVsProbability > 0f && best.IVsProbability <= 1f,
                $"expected a real reach probability in (0,1], got {best.IVsProbability}");
        }

        [TestMethod]
        public void NoCake_NearTargetParentsCannotReachTargetIV()
        {
            var reached = SolveForHp100(BreedingCake.None)
                .OfType<BredPalReference>()
                .Any(r => r.Pal.Name == "Lamball" && r.IVs.HP.Satisfies(100));

            Assert.IsFalse(reached,
                "without a cake, 98-IV parents cannot deterministically reach a 100-IV child");
        }
    }
}
